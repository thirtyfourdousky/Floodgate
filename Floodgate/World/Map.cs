using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Floodgate.World;

public static class Map
{
    internal static void Apply()
    {
        IL.ModManager.GenerateMergedMods += ModManager_GenerateMergedMods;
    }

    private static void ModManager_GenerateMergedMods(ILContext il)
    {
        try
        {
            ILCursor c = new(il);

            //first go-to just exists to make sure it's the right local
            if (c.TryGotoNext(MoveType.After, x => x.MatchLdstr("mergedmods"), x => x.MatchCallOrCallvirt(out _), x => x.MatchStloc(0)) && c.TryGotoNext(MoveType.After, x => x.MatchStloc(2)))
            {
                IEnumerable<ILLabel> incoming = c.IncomingLabels;
                c.Emit(OpCodes.Ldloc_0);
                foreach (ILLabel label in incoming)
                {
                    label.Target = c.Prev;
                }
                c.EmitDelegate(delegate (string MergedMods)
                {
                    //literally vanilla (no DLC)
                    string VanillaWorldPath = Path.Combine(RWCustom.Custom.RootFolderDirectory(), "World");
                    RelativeCopy(VanillaWorldPath, MergedMods);

                    //dlc and mods
                    for (int i = 0; i < ModManager.ActiveMods.Count; i++)
                    {
                        List<string> searchPaths = new List<string>();

                        string targetedWorld = Path.Combine(ModManager.ActiveMods[i].TargetedPath, "World");
                        if (Directory.Exists(targetedWorld))
                        {
                            searchPaths.Add(targetedWorld);
                        }

                        string newestWorld = Path.Combine(ModManager.ActiveMods[i].NewestPath, "World");
                        if (FloodgatePatcher.ModLoader.IsLatest && Directory.Exists(newestWorld))
                        {
                            searchPaths.Add(newestWorld);
                        }

                        string regularWorld = Path.Combine(ModManager.ActiveMods[i].path, "World");
                        if (Directory.Exists(regularWorld))
                        {
                            searchPaths.Add(regularWorld);
                        }
                        foreach (string path in searchPaths)
                        {
                            RelativeCopy(path, MergedMods);
                        }

                    }

                });
            }
            else
            {
                FloodgatePatcher.CustomLog.LogError("GenerateMergedMods IL hook failed");
            }

        }
        catch (Exception ex)
        {
            FloodgatePatcher.CustomLog.LogError(ex.ToString());
        }
    }
    public static void RelativeCopy(string SourcePath, string MergedMods)
    {
        string[] Maps = Directory.GetFiles(SourcePath, "map_*.*", SearchOption.AllDirectories);

        foreach (string Map in Maps)
        {
            if(!(Map.EndsWith(".txt", StringComparison.OrdinalIgnoreCase) || Map.EndsWith(".png", StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }
            string destination = Path.Combine(MergedMods, "world", Map.Replace(SourcePath, "").TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            //FloodgatePatcher.CustomLog.Log("[Map \"merging\"] Debug, " + Map);
            if (!File.Exists(destination))
            {
                //FloodgatePatcher.CustomLog.Log("[Map \"merging\"] Debug, destination " + destination + " does not exists and should be copied");
                try
                {
                    string dir = Path.GetDirectoryName(destination);
                    if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    File.Copy(Map, destination);
                }catch (Exception ex)
                {
                    FloodgatePatcher.CustomLog.LogError("[Map \"merging\"] Copying file " + Map + " to " + destination + " failed\n" + ex.ToString());
                }
            }
            else
            {
                //FloodgatePatcher.CustomLog.Log("[Map \"merging\"] Debug, destination " + destination + " exists and should be skipped");
            }
        }
    }
}
