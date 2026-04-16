using BepInEx;
using System;
using System.Collections.Generic;
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
        catch (System.IO.FileNotFoundException)
        {

        }
        catch (Exception e)
        {
            FloodgatePatcher.CustomLog.LogError("MergeFix apply failed\nIf MergeFix is not present, just ignore this\n" + e.ToString());
        }
        try
        {
            ModCompat.beecat.Apply();
        }
        catch (System.IO.FileNotFoundException)
        {
            
        }
        catch (Exception e)
        {
            FloodgatePatcher.CustomLog.LogError("Beecat apply failed\nIf Beecat is not present, just ignore this\n" + e.ToString());
        }
    }

}
