using BepInEx;
using FloodgatePatcher;
using ModCompat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloodgateExtra;

[BepInDependency("floodgate", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("bro.fixedmerging", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("bro.mergefix", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("beeworld", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("zombieseatflesh7.MenuFixes", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("Gamer025.RemixAutoRestart", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("NCR.theunbound", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("exist.reremix", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("emeralds_features", BepInDependency.DependencyFlags.SoftDependency)]
[BepInPlugin(GUID, Name, Version)]
public class FloodgateExtra : BaseUnityPlugin
{
    const string Name = "FloodgateExtra";
    const string GUID = "floodgateextra";
    const string Version = "0.0.3";

    public void Awake()
    {
        try
        {
            Floodgate.World.MergeFixMap.Apply();
        }
        catch (FileNotFoundException)
        {

        }
        catch (Exception e)
        {
            CustomLog.LogError("MergeFix apply failed\nIf MergeFix is not present, just ignore this\n" + e.ToString());
        }
        try
        {
            beecat.Apply();
        }
        catch (FileNotFoundException)
        {

        }
        catch (Exception e)
        {
            CustomLog.LogError("Beecat apply failed\nIf Beecat is not present, just ignore this\n" + e.ToString());
        }
        try
        {
            RemixAutoRestarter.Apply_MMF();
        }
        catch (FileNotFoundException)
        {
        }
        catch (Exception e)
        {
            CustomLog.LogError("ManyMenuFixes specific apply failed.\nIf ManyMenuFixes is not present, just ignore this\n" + e.ToString());
        }
        try
        {
            RemixAutoRestarter.Apply_AutoRestarter();
        }
        catch (FileNotFoundException)
        {
        }
        catch (Exception e)
        {
            CustomLog.LogError("Remix Auto Restarter specific apply failed.\nIf Remix Auto Restarter is not present, just ignore this\n" + e.ToString());
        }
        try
        {
            ModCompat._Unbound.DisableUnregister.Apply();
        }
        catch (FileNotFoundException)
        {
        }
        catch (Exception e)
        {
            CustomLog.LogError("Unbound specific apply failed.\nIf Unbound is not present, just ignore this\n" + e.ToString());
        }

        try
        {
            EmeraldTweaks.Apply();
        }
        catch (FileNotFoundException)
        {
        }
        catch (Exception e)
        {
            CustomLog.LogError("Emerald Tweaks apply failed\nIf Emerald Tweaks is not present, just ignore this\n" + e.ToString());
        }
        try
        {
            _ReRemix.Apply();
        }
        catch (FileNotFoundException)
        {
        }
        catch (Exception e)
        {
            CustomLog.LogError("ReRemix apply failed\nIf ReRemix is not present, just ignore this\n" + e.ToString());
        }
    }
}
