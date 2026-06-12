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
[BepInPlugin(GUID, Name, Version)]
public class FloodgateExtra : BaseUnityPlugin
{
    const string Name = "FloodgateExtra";
    const string GUID = "floodgateextra";
    const string Version = "0.0.1";

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
    }
}
