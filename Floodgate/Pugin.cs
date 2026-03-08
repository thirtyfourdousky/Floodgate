using BepInEx;
using BepInEx.Logging;
using FloodgatePatcher;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Floodgate;

[BepInPlugin(GUID, Name, Version)]
public partial class Plugin : BaseUnityPlugin
{
    public const string GUID = "floodgate";
    public const string Name = "Floodgate";
    public const string Version = "0.1.16";

    public static Plugin? Instance { get; private set; }

    public static bool woke = false;

    public static ManualLogSource logger;

    public static int ictCount = 0;

    public static RemixInterface RemixOptions;
    public void Awake()
    {
        Instance = this;
        ictCount = 0;
        if(woke)
        {
            return;
        }
        logger = base.Logger;
        
        On.RainWorld.PostModsInit += RainWorld_PostModsInit;
        On.RainWorld.OnModsInit += RainWorld_OnModsInit;
        On.ModManager.ModApplyer.Start += ModApplyer_Start;
        On.StaticWorld.InitCustomTemplates += (On.StaticWorld.orig_InitCustomTemplates orig) =>
        {
            orig();
            if (ictCount > 10 && ictCount < 20)
            {
                logger.LogInfo("Custom Templates Stack Below:\n" + new System.Diagnostics.StackTrace().ToString());
            }
            if (ictCount > 100)
            {
                throw new Exceptions.LoopException("Unexpected loop of the current method");
            }
            ictCount++;
        };

        On.Menu.EndgameMeter.NotchMeter.ctor += NotchMeter_ctor;

        FloodgatePatcher.CustomLog.Log("Floodgate plugin initialized");

        woke = true;
    }

    private void ModApplyer_Start(On.ModManager.ModApplyer.orig_Start orig, ModManager.ModApplyer self, bool filesInBadState)
    {
        //FloodgatePatcher.ModLoader.ResetMergedMods();
        TurboAssetManager.accessfgmerged = false;
        orig(self,filesInBadState);
    }

    private void NotchMeter_ctor(On.Menu.EndgameMeter.NotchMeter.orig_ctor orig, Menu.EndgameMeter.NotchMeter self, Menu.EndgameMeter owner)
    {
        if (self == null || owner == null) return;

        WinState.ListTracker tracker = owner.tracker as WinState.ListTracker;

        if (ModManager.MSC)
        {
            if(owner.tracker.ID == MoreSlugcats.MoreSlugcatsEnums.EndgameID.Nomad)
            {
                for(int i = 0; i < tracker.myList.Count; i++)
                {
                    if (tracker.myList[i] >= Region.GetFullRegionOrder().Count)
                    {
                        tracker.myList[i] = 0;
                    }
                }
            }
        }

        orig(self, owner);
    }

    bool onmodsinit = false;
    private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        TurboAssetManager.accessfgmerged = false;
        Registry.Apply();
        //TurboAssetManager.Map();
        if (onmodsinit)
        {
            orig(self);
            Registry.Merge();
            return;
        }
        onmodsinit = true;
        TurboAssetManager.Apply();
        _Modules.DevTools.Objects.ObjectsMenu.Enable();

        if (FGTools.IsModActive("yeliah.slugpupFieldtrip") && FGTools.IsModActive("sprobgik.desecratinggraves"))
        {
            try
            {
                ModCompat.NotScugPlaySpupSafari.Apply();
            }catch(Exception e)
            {
                CustomLog.LogError("Not Slugcat Playables + Slugpup Safari compat failed\n" + e.ToString());
            }
        }
        if (FGTools.IsModActive("javadog.gateNames"))
        {
            try
            {
                ModCompat.WDTGGcompat.Apply();
            }catch(Exception e)
            {
                CustomLog.LogError("WDTGG apply failed\n" + e.ToString());
            }
        }
        if (FGTools.IsModActive("lb-fgf-m4r-ik.modpack"))
        {
            try
            {
                ModCompat.LBspecific.Apply();
            }
            catch(Exception e)
            {
                CustomLog.LogError("M4rblelous Entity Pack specific apply failed\n" + e.ToString());
            }
        }
        if (FGTools.IsModActive("regionkit"))
        {
            try
            {
                ModCompat._RegionKit.BackgroundBuilder_Data.Apply();
            }catch(Exception e)
            {
                CustomLog.LogError("RegionKit specific apply failed\n" + e.ToString());
            }
        }
        if (FGTools.IsModActive("nacu.lunacy"))
        {
            try
            {
                ModCompat.Lunacy.CustomTokens.LunacyTokens.Apply();
            }
            catch (Exception e)
            {
                CustomLog.LogError("Lunacy specific apply failed\n" + e.ToString());
            }
        }
        if (FGTools.IsModActive("crs"))
        {
            try
            {
                ModCompat.CRS.IndexedEntranceClass.Apply();
            }
            catch (Exception e)
            {
                CustomLog.LogError("CRS specific apply failed\n" + e.ToString());
            }
        }
        //before orig
        orig(self); //yes im blind
        //after orig
        try
        {
            MachineConnector.SetRegisteredOI(GUID, RemixOptions = new());
            RemixOptions._LoadConfigFile();
        }
        catch(System.Exception e)
        {
            FloodgatePatcher.CustomLog.LogError(e.ToString());
        }
    }

    bool postmodsinit = false;
    private void RainWorld_PostModsInit(On.RainWorld.orig_PostModsInit orig, RainWorld self)
    {
        //var mapTask = System.Threading.Tasks.Task.Run(TurboAssetManager.preload);
        orig(self);
        Registry.Merge();
        //mapTask.GetAwaiter().GetResult();
        NotEnums.EnabledMods.Apply();
        NotEnums.CreatureTemplateType.PostModsInit();
        if (postmodsinit)
        {
            return;
        }
        postmodsinit = true;
        World.CustomMerger.Apply();
        World.Optimization.Apply();
        FG_Expedition.CreatureMergerTools.Apply();
        UI.RemixModList.Apply();
        Steam.Workshop.Apply();
        ExHooks.HookManager.ApplyHooks();
        _Modules.ExtendedEchoExtender.Hooks.Apply();
    }
}
