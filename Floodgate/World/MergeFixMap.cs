using FloodgatePatcher;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.IO;

namespace Floodgate.World;

public static class MergeFixMap
{
    internal static readonly List<IDetour> _hooks = new List<IDetour>();
    public static void Apply()
    {
        CustomLog.Log("MergeFix apply..");
        _hooks.Add(new ILHook(typeof(global::MergeFix.MergeFixPlugin).GetMethod("ModManager_GenerateMergedMods", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic), IL_MergeFix_GenerateMergedMods));
        _hooks.Add(new ILHook(typeof(global::MergeFix.MergeFixPlugin).GetMethod("ModMerger_ExecutePendingMerge", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic), IL_MergeFix_ExecutePendingMerge));
        CustomLog.Log("Finished");
    }

    //yes this is the same as the regular one
    public static void IL_MergeFix_GenerateMergedMods(ILContext il)
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
                    string VanillaWorldPath = (RWCustom.Custom.RootFolderDirectory() + Path.DirectorySeparatorChar + "World");
                    Map.RelativeCopy(VanillaWorldPath, MergedMods);

                    //dlc and mods
                    for (int i = 0; i < ModManager.ActiveMods.Count; i++)
                    {
                        List<string> searchPaths = new List<string>();

                        string targetedWorld = (ModManager.ActiveMods[i].TargetedPath + Path.DirectorySeparatorChar + "World");
                        if (Directory.Exists(targetedWorld))
                        {
                            searchPaths.Add(targetedWorld);
                        }

                        string newestWorld = (ModManager.ActiveMods[i].NewestPath + Path.DirectorySeparatorChar + "World");
                        if (FloodgatePatcher.ModLoader.IsLatest && Directory.Exists(newestWorld))
                        {
                            searchPaths.Add(newestWorld);
                        }

                        string regularWorld = (ModManager.ActiveMods[i].path + Path.DirectorySeparatorChar + "World");
                        if (Directory.Exists(regularWorld))
                        {
                            searchPaths.Add(regularWorld);
                        }
                        foreach (string path in searchPaths)
                        {
                            Map.RelativeCopy(path, MergedMods);
                        }

                    }

                });
            }
            else
            {
                CustomLog.LogError("MergeFix GenerateMergedMods IL hook failed");
            }

        }
        catch (Exception ex)
        {
            CustomLog.LogError(ex.ToString());
        }
    }
    public static void IL_MergeFix_ExecutePendingMerge(ILContext il)
    {
        try
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.Before, x => x.MatchCall(typeof(File).GetMethod("Copy", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static| System.Reflection.BindingFlags.NonPublic, null, new Type[] {typeof(string), typeof(string), typeof(bool)}, null))))
            {
                IEnumerable<ILLabel> labels = c.IncomingLabels;
                c.EmitDelegate(SafeCopy);
                c.Remove();
                foreach(ILLabel label in labels)
                {
                    label.Target = c.Prev;
                }
            }
            else
            {
                CustomLog.LogError("MergeFix ExecutePendingMerge IL hook failed");
            }

        }
        catch (Exception ex)
        {
            CustomLog.LogError(ex.ToString());
        }
    }

    public static void SafeCopy(string sourceFileName, string destFileName, bool overwrite)
    {
        try
        {
            if (!string.Equals(sourceFileName.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar), destFileName.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase))
            {
                File.Copy(sourceFileName, destFileName, overwrite);
            }
        }
        catch (Exception ex)
        {
            CustomLog.LogError(ex.ToString());
            throw;
        }
    }
}
