using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Security.Policy;
using UnityEngine;

namespace Floodgate;

public static class Registry
{
    public static readonly List<RegisteredMod> Mods = new();
    public static void Apply()
    {
        Rescan();
    }

    public static void Rescan()
    {
        Mods.Clear();
        foreach (ModManager.Mod mod in ModManager.ActiveMods)
        {
            if (mod == null || Mods.Any(i=>i.mod.id == mod.id)) { continue; }
            string floodgatepath = mod.TargetedPath + Path.DirectorySeparatorChar + "floodgate";
            if (mod.hasTargetedVersionFolder && Directory.Exists(floodgatepath))
            {
                Mods.Add(new(floodgatepath, mod));
                continue;
            }
            floodgatepath = mod.NewestPath + Path.DirectorySeparatorChar + "floodgate";
            if (FloodgatePatcher.ModLoader.IsLatest && mod.hasNewestFolder && Directory.Exists(floodgatepath))
            {
                Mods.Add(new(floodgatepath, mod));
                continue;
            }
            floodgatepath = mod.path + Path.DirectorySeparatorChar + "floodgate";
            if (Directory.Exists(floodgatepath))
            {
                Mods.Add(new(floodgatepath, mod));
            }
        }
    }
    public static void OpRescan()
    {
        Rescan();
        Merge();
    }
    public static void Merge()
    {
        if(FloodgatePatcher.ModLoader.FloodgateMergedInfo is null)
        {
            return;
        }
        if(!FloodgatePatcher.ModLoader.FloodgateMergedInfo.Exists || !string.Equals(FloodgatePatcher.ModLoader.FloodgateMergedInfo.Name, "FloodgateMergedMods", StringComparison.OrdinalIgnoreCase) || !string.Equals(FloodgatePatcher.ModLoader.FloodgateMergedInfo.Parent.Name, "StreamingAssets", StringComparison.OrdinalIgnoreCase))
        {
            FloodgatePatcher.CustomLog.Log("[File Merging] Floodgate's merged folder could not be found for some reason");
            FloodgatePatcher.ModLoader.FloodgateMergedInfo = null;
            return;
        }
        System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

        List<string> merged = new();
        List<string> mergedfiles = new();
        foreach(RegisteredMod mod in Mods)
        {
            string mergedpath = (mod.floodgate + Path.DirectorySeparatorChar + "merged" + Path.DirectorySeparatorChar + "merged.txt");
            if (File.Exists(mergedpath))
            {
                mergedfiles.Add(mergedpath);
            }
        }
        foreach(string file in mergedfiles)
        {
            string[] lines = File.ReadAllLines(file);
            foreach (string line in lines)
            {
                string cur = line.Trim();
                if (string.IsNullOrWhiteSpace(cur))
                {
                    continue;
                }
                int modsStart = cur.IndexOf("{{");
                int modsEnd = cur.IndexOf("}}");
                if (modsStart != -1 && modsEnd != -1)
                {
                    var mods = cur.Substring(modsStart + 2, modsEnd - modsStart - 2).Split(',');
                    if (mods.Length > 0)
                    {
                        var ActiveModIDs = ModManager.ActiveMods.Select(i => i.id).ToList();
                        List<string> pMods = mods.Where(i => !i.StartsWith("!")).ToList();
                        List<bool> pmod = new();
                        foreach (var mod in pMods)
                        {
                            pmod.Add(ActiveModIDs.Contains(mod));
                        }
                        if (!pmod.All(i => i == true))
                        {
                            continue;
                        }
                        List<string> nMods = mods.Where(i => i.StartsWith("!")).ToList();
                        bool nmod = false;
                        foreach (string mod in nMods)
                        {
                            if (ActiveModIDs.Contains(mod.trimStart('!')))
                            {
                                nmod = true;
                                break;
                            }
                        }
                        if (nmod)
                        {
                            continue;
                        }
                    }
                    cur = cur.Remove(modsStart, modsEnd - modsStart + 2);
                }
                else if (modsStart == 1 ^ modsEnd == 1)
                {
                    FloodgatePatcher.CustomLog.LogError("Broken line\n    " + cur + "\n    missing " + (modsStart == -1 ? "start `{{`" : "end `}}`") + " mods array pattern");
                    continue;
                }
                if (string.IsNullOrWhiteSpace(cur))
                {
                    continue;
                }
                if (merged.Contains(cur.ToLowerInvariant()))
                {
                    continue;
                }
                merged.Add(cur.ToLowerInvariant());
            }
        }
        foreach (string merge in merged)
        {
            foreach (RegisteredMod mod in Mods)
            {

                string FolderPath = (mod.floodgate + Path.DirectorySeparatorChar + "merged" + Path.DirectorySeparatorChar + merge);
                if (Directory.Exists(FolderPath))
                {
                    MergeCopy(FolderPath, FolderPath);
                }
            }
        }
        foreach (RegisteredMod mod in Mods)
        {
            string overridePath = (mod.floodgate + Path.DirectorySeparatorChar + "override");
            if (Directory.Exists(overridePath))
            {
                MergeCopy(overridePath, overridePath);
            }
        }

        foreach(string file in Directory.GetFiles(FloodgatePatcher.ModLoader.FloodgateMergedInfo.FullName, "*.*", SearchOption.AllDirectories))
        {
            string path = file.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            if(!handledpaths.Contains(path) && (path.EndsWith(".txt", StringComparison.OrdinalIgnoreCase) || path.EndsWith(".png", StringComparison.OrdinalIgnoreCase)))
            {
                File.Delete(path);
                //FloodgatePatcher.CustomLog.Log("[File Merging] Deleting file " + path );
            }
        }
        handledpaths.Clear();

        TurboAssetManager.accessfgmerged = true;
        sw.Stop();
        FloodgatePatcher.CustomLog.Log("[File Merging] Took " + sw.ElapsedMilliseconds + "ms");
    }
    public static readonly HashSet<string> handledpaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    public static void MergeCopy(string path, string trimm)
    {
        string dirDestination = (FloodgatePatcher.ModLoader.FloodgateMergedInfo.FullName + Path.DirectorySeparatorChar + path.Replace(trimm, "").TrimStart(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }));
        //FloodgatePatcher.CustomLog.Log("[File Merging] Checking directory " + dirDestination);
        //FloodgatePatcher.CustomLog.Log("[File Merging] Path " + path + " Trimm " + trimm);
        if (!Directory.Exists(dirDestination))
        {
            try
            {
                //FloodgatePatcher.CustomLog.Log("[File Merging] Creating directory " + dirDestination);
                Directory.CreateDirectory(dirDestination);
            }catch(System.Exception ex)
            {
                FloodgatePatcher.CustomLog.LogError("[File Merging] Error creating directory " + dirDestination + "\n" + ex.ToString());
                return;
            }
        }
        foreach(string dir  in Directory.GetDirectories(path))
        {
            MergeCopy(dir,trimm);
        }
        foreach(string file in Directory.GetFiles(path))
        {
            string destination = (FloodgatePatcher.ModLoader.FloodgateMergedPath + Path.DirectorySeparatorChar + file.Replace(trimm, "").TrimStart(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar })).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            if (!File.Exists(destination))
            {
                try
                {
                    //FloodgatePatcher.CustomLog.Log("[File Merging] Copying file " + file + " to " + destination);
                    File.Copy(file, destination, false);
                    handledpaths.Add(destination);
                }
                catch (System.Exception ex)
                {
                    FloodgatePatcher.CustomLog.LogError("[File Merging] Error copying file " + file + " to " + destination + "\n" + ex.ToString());
                }
            }
            else if(!handledpaths.Contains(destination))
            {
                try
                {
                    if (ShouldReplace(file, destination))
                    {
                        //FloodgatePatcher.CustomLog.Log("[File Merging] Replacing file " + destination + " with " + file);
                        File.Copy(file, destination, true);
                    }
                    handledpaths.Add(destination);
                }
                catch (System.Exception ex)
                {
                    FloodgatePatcher.CustomLog.LogError("[File Merging] Error overriding file " + file + " to " + destination + "\n" + ex.ToString());
                }
            }
        }
    }
    public static bool ShouldReplace(string path, string target)
    {
        FileInfo source = new FileInfo(path);
        FileInfo destination = new FileInfo(target);

        if (!destination.Exists)
        {
            return true;
        }

        if(source.Length == destination.Length && source.LastWriteTime == destination.LastWriteTime)
        {
            return false;
        }
        string sourcehash = null;
        string desthash = null;
        using (FileStream sourcefs = File.OpenRead(path))
        using (SHA512 sourcesha = SHA512.Create())
        {
            sourcehash = string.Join("", sourcesha.ComputeHash(sourcefs).Select(x => x.ToString("x2")));
        }
        using (FileStream destfs = File.OpenRead(target))
        using (SHA512 destsha = SHA512.Create())
        {
            desthash = string.Join("", destsha.ComputeHash(destfs).Select(x => x.ToString("x2")));
        }
        if(sourcehash == desthash)
        {
            return false;
        }

        return true;
    }

    public class RegisteredMod
    {
        public string floodgate;
        public string id;
        public ModManager.Mod mod;

        public RegisteredMod(string floodgatepath, ModManager.Mod mod)
        {
            floodgate = floodgatepath;
            id = mod.id;
            this.mod = mod;
            FloodgatePatcher.CustomLog.Log("Registered Mod " + mod.name + " (" + id + ")");
        }
    }
}
