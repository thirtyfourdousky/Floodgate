using BepInEx;
using BepInEx.Logging;
using FloodgatePatcher;
using System;
using System.IO;

namespace Floodgate;

[BepInPlugin(GUID, Name, Version)]
public class Plugin : BaseUnityPlugin
{
    public const string GUID = "floodgate";
    public const string Name = "Floodgate";
    public const string Version = "0.1.12";

    public static Plugin? Instance { get; private set; }

    public static bool woke = false;

    public static ManualLogSource logger;

    public static RemixInterface RemixOptions;
    public void Awake()
    {
        Instance = this;
        if(woke)
        {
            return;
        }
        logger = base.Logger;

        On.RainWorld.PostModsInit += RainWorld_PostModsInit;
        On.RainWorld.OnModsInit += RainWorld_OnModsInit;

        On.Menu.EndgameMeter.NotchMeter.ctor += NotchMeter_ctor;

        FloodgatePatcher.CustomLog.Log("Floodgate plugin initialized");

        woke = true;
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
        if (NotEnums.EnabledMods.IsModActive("lb-fgf-m4r-ik.modpack"))
        {
            try
            {
                ModCompat.LBspecific.Apply();
            }
            catch (Exception e)
            {
                CustomLog.LogError("M4rblelous Entity Pack specific apply failed\n" + e.ToString());
            }
        }
        orig(self);
        if (onmodsinit)
        {
            return;
        }
        onmodsinit = true;
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
        orig(self);
        NotEnums.EnabledMods.Apply();
        NotEnums.CreatureTemplateType.PostModsInit();
        if (postmodsinit)
        {
            return;
        }
        postmodsinit = true;
        //World.CustomMerger.Apply();
        FG_Expedition.CreatureMergerTools.Apply();
        Registry.Apply();
        UI.RemixModList.Apply();
        Steam.Workshop.Apply();
        ExHooks.HookManager.ApplyHooks();
    }
}
