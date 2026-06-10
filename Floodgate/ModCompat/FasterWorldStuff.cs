using Floodgate;
using gelbi_silly_lib.MonoModUtils;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.IO;

namespace ModCompat;

public static class FasterWorldStuff
{
    public static readonly List<object> _hooks = new List<object>();

    public static void GSL_Apply_Extra()
    {
        //_hooks.Add(DetourUtils.newHookRND((On.AssetManager.orig_ResolveFilePath_string_bool_bool)AssetManager.ResolveFilePath, TurboAssetManager.AssetManager_ResolveFilePath_string_bool_bool));
        _hooks.Add(gelbi_silly_lib.VersionSpecific.newNativeDetour((On.AssetManager.orig_ResolveFilePath_string_bool_bool)faster_world_extra.M_Assets.AssetManager_ResolveFilePath, (On.AssetManager.orig_ResolveFilePath_string_bool_bool)AssetManager_ResolveFilePath_Extra));
        _hooks.Add(gelbi_silly_lib.VersionSpecific.newNativeDetour((On.AssetManager.orig_ResolveFilePath_string)faster_world_extra.M_Assets.ResolveFilePath, (On.AssetManager.orig_ResolveFilePath_string)ResolveFilePath_Extra));
        _hooks.Add(DetourUtils.newHookRND((On.AssetManager.orig_ResolveAllFilePaths)AssetManager.ResolveAllFilePaths, (On.AssetManager.hook_ResolveAllFilePaths)TurboAssetManager.AssetManager_ResolveAllFilePaths));
        _hooks.Add(DetourUtils.newHookRND((On.AssetManager.orig_ResolveDirectory)AssetManager.ResolveDirectory, (On.AssetManager.hook_ResolveDirectory)TurboAssetManager.AssetManager_ResolveDirectory));
        IL.AssetManager.ListDirectory_string_bool_bool_bool += TurboAssetManager.AssetManager_ListDirectory_string_bool_bool_bool;
    }

    public static void Apply_Extra()
    {
        _hooks.Add(new NativeDetour((On.AssetManager.orig_ResolveFilePath_string_bool_bool)faster_world_extra.M_Assets.AssetManager_ResolveFilePath, (On.AssetManager.orig_ResolveFilePath_string_bool_bool)AssetManager_ResolveFilePath_Extra));
        _hooks.Add(new NativeDetour((On.AssetManager.orig_ResolveFilePath_string)faster_world_extra.M_Assets.ResolveFilePath, (On.AssetManager.orig_ResolveFilePath_string)ResolveFilePath_Extra));
        On.AssetManager.ResolveAllFilePaths += TurboAssetManager.AssetManager_ResolveAllFilePaths;
        On.AssetManager.ResolveDirectory += TurboAssetManager.AssetManager_ResolveDirectory;
        IL.AssetManager.ListDirectory_string_bool_bool_bool += TurboAssetManager.AssetManager_ListDirectory_string_bool_bool_bool;
    }

    /*public static void h(ILContext IL)
    {
        try
        {
            ILCursor c = new(IL);
            c.GotoNext(MoveType.Before, x => x.MatchLdftn(((On.AssetManager.orig_ResolveFilePath_string_bool_bool)global::faster_world_extra.M_Assets.AssetManager_ResolveFilePath).Method));
            c.Remove();
            c.Emit(OpCodes.Ldftn, IL.Import(((On.AssetManager.orig_ResolveFilePath_string_bool_bool)AssetManager_ResolveFilePath_Extra).Method));
            
        }catch (Exception ex)
        {
            FloodgatePatcher.CustomLog.LogError("Overriding Faster World hook failed\n" + ex.ToString());
        }
    }*/

    public static string AssetManager_ResolveFilePath_Extra(string path, bool skipMergedMods = false, bool skipConsoleFiles = false)
    {
        path = path.ToLowerInvariant();
        string text;
        if ((!skipMergedMods && ((TurboAssetManager.accessfgmerged && File.Exists(text = RWCustom.Custom.RootFolderDirectory() + Path.DirectorySeparatorChar + "floodgatemergedmods" + Path.DirectorySeparatorChar + path)) || File.Exists(text = faster_world_extra.M_Assets.mergedmodsPath + path))) || faster_world_extra.M_Assets.cachedPaths.TryGetValue(path.Replace('/', '\\'), out text) || (!skipConsoleFiles && faster_world_extra.M_Assets.consolefilesPath != null && File.Exists(text = faster_world_extra.M_Assets.consolefilesPath + path)))
        {
            return text;
        }
        return (RWCustom.Custom.rootFolderDirectory + Path.DirectorySeparatorChar + path);
    }

    public static string ResolveFilePath_Extra(string path)
    {
        string text;
        if (((TurboAssetManager.accessfgmerged && File.Exists(text = RWCustom.Custom.RootFolderDirectory() + Path.DirectorySeparatorChar + "floodgatemergedmods" + Path.DirectorySeparatorChar + path)) || File.Exists(text = faster_world_extra.M_Assets.mergedmodsPath + path)) || faster_world_extra.M_Assets.cachedPaths.TryGetValue(path.ToLowerInvariant().Replace('/', '\\'), out text) || (faster_world_extra.M_Assets.consolefilesPath != null && File.Exists(text = faster_world_extra.M_Assets.consolefilesPath + path)) || File.Exists(text = Path.Combine(RWCustom.Custom.rootFolderDirectory, path)))
        {
            return text;
        }
        return null;
    }



    public static void GSL_Apply()
    {
        _hooks.Add(gelbi_silly_lib.VersionSpecific.newNativeDetour((On.AssetManager.orig_ResolveFilePath_string_bool_bool)faster_world.M_Assets.AssetManager_ResolveFilePath, (On.AssetManager.orig_ResolveFilePath_string_bool_bool)AssetManager_ResolveFilePath));
        _hooks.Add(gelbi_silly_lib.VersionSpecific.newNativeDetour((On.AssetManager.orig_ResolveFilePath_string)faster_world.M_Assets.ResolveFilePath, (On.AssetManager.orig_ResolveFilePath_string)ResolveFilePath));
    }

    public static void Apply()
    {
        _hooks.Add(new NativeDetour((On.AssetManager.orig_ResolveFilePath_string_bool_bool)faster_world.M_Assets.AssetManager_ResolveFilePath, (On.AssetManager.orig_ResolveFilePath_string_bool_bool)AssetManager_ResolveFilePath));
        _hooks.Add(new NativeDetour((On.AssetManager.orig_ResolveFilePath_string)faster_world.M_Assets.ResolveFilePath, (On.AssetManager.orig_ResolveFilePath_string)ResolveFilePath));
    }

    public static string AssetManager_ResolveFilePath(string path, bool skipMergedMods = false, bool skipConsoleFiles = false)
    {
        path = path.ToLowerInvariant();
        string text;
        if (!skipMergedMods && ((TurboAssetManager.accessfgmerged && File.Exists(text = RWCustom.Custom.RootFolderDirectory() + Path.DirectorySeparatorChar + "floodgatemergedmods" + Path.DirectorySeparatorChar + path)) || File.Exists(text = faster_world.M_Assets.mergedmodsPath + path)))
        {
            return text;
        }
        for (int i = ModManager.ActiveMods.Count - 1; i >= 0; i--)
        {
            if ((ModManager.ActiveMods[i].hasTargetedVersionFolder && File.Exists(text = Path.Combine(ModManager.ActiveMods[i].TargetedPath, path))) || (ModManager.ActiveMods[i].hasNewestFolder && File.Exists(text = Path.Combine(ModManager.ActiveMods[i].NewestPath, path))) || File.Exists(text = Path.Combine(ModManager.ActiveMods[i].path, path)))
            {
                return text;
            }
        }
        if (!skipConsoleFiles && faster_world.M_Assets.consolefilesPath != null && File.Exists(text = faster_world.M_Assets.consolefilesPath + path))
        {
            return text;
        }
        return Path.Combine(RWCustom.Custom.rootFolderDirectory, path);
    }

    public static string ResolveFilePath(string path)
    {
        string text;
        if (((TurboAssetManager.accessfgmerged && File.Exists(text = FloodgateMergedMods + path)) || File.Exists(text = faster_world.M_Assets.mergedmodsPath + path)))
        {
            return text;
        }
        for (int i = ModManager.ActiveMods.Count - 1; i >= 0; i--)
        {
            if ((ModManager.ActiveMods[i].hasTargetedVersionFolder && File.Exists(text = Path.Combine(ModManager.ActiveMods[i].TargetedPath, path))) || (ModManager.ActiveMods[i].hasNewestFolder && File.Exists(text = Path.Combine(ModManager.ActiveMods[i].NewestPath, path))) || File.Exists(text = Path.Combine(ModManager.ActiveMods[i].path, path)))
            {
                return text;
            }
        }
        if ((faster_world.M_Assets.consolefilesPath != null && File.Exists(text = faster_world.M_Assets.consolefilesPath + path)) || File.Exists(text = Path.Combine(RWCustom.Custom.rootFolderDirectory, path)))
        {
            return text;
        }
        return null;
    }

    public static string FloodgateMergedMods = RWCustom.Custom.RootFolderDirectory() + Path.DirectorySeparatorChar + "floodgatemergedmods" + Path.DirectorySeparatorChar;
}
