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

    //Faster World Extra
    public static void GSL_Apply_Extra()
    {
        _hooks.Add(DetourUtils.newHookRND((On.AssetManager.orig_ResolveFilePath_string_bool_bool)faster_world_extra.M_Assets.AssetManager_ResolveFilePath, (On.AssetManager.hook_ResolveFilePath_string_bool_bool)AssetManager_ResolveFilePath));
        _hooks.Add(DetourUtils.newHookRND((On.AssetManager.orig_ResolveFilePath_string)faster_world_extra.M_Assets.ResolveFilePath, (On.AssetManager.hook_ResolveFilePath_string)ResolveFilePath));
        _hooks.Add(DetourUtils.newHookRND((On.AssetManager.orig_ResolveAllFilePaths)AssetManager.ResolveAllFilePaths, (On.AssetManager.hook_ResolveAllFilePaths)TurboAssetManager.AssetManager_ResolveAllFilePaths));
        _hooks.Add(DetourUtils.newHookRND((On.AssetManager.orig_ResolveDirectory)AssetManager.ResolveDirectory, (On.AssetManager.hook_ResolveDirectory)TurboAssetManager.AssetManager_ResolveDirectory));
        IL.AssetManager.ListDirectory_string_bool_bool_bool += TurboAssetManager.AssetManager_ListDirectory_string_bool_bool_bool;
    }

    public static void Apply_Extra()
    {
        _hooks.Add(new Hook((On.AssetManager.orig_ResolveFilePath_string_bool_bool)faster_world_extra.M_Assets.AssetManager_ResolveFilePath, (On.AssetManager.hook_ResolveFilePath_string_bool_bool)AssetManager_ResolveFilePath));
        _hooks.Add(new Hook((On.AssetManager.orig_ResolveFilePath_string)faster_world_extra.M_Assets.ResolveFilePath, (On.AssetManager.hook_ResolveFilePath_string)ResolveFilePath));
        On.AssetManager.ResolveAllFilePaths += TurboAssetManager.AssetManager_ResolveAllFilePaths;
        On.AssetManager.ResolveDirectory += TurboAssetManager.AssetManager_ResolveDirectory;
        IL.AssetManager.ListDirectory_string_bool_bool_bool += TurboAssetManager.AssetManager_ListDirectory_string_bool_bool_bool;
    }

    //Faster World
    public static void GSL_Apply()
    {
        _hooks.Add(DetourUtils.newHookRND((On.AssetManager.orig_ResolveFilePath_string_bool_bool)faster_world.M_Assets.AssetManager_ResolveFilePath, (On.AssetManager.hook_ResolveFilePath_string_bool_bool)AssetManager_ResolveFilePath));
        _hooks.Add(DetourUtils.newHookRND((On.AssetManager.orig_ResolveFilePath_string)faster_world.M_Assets.ResolveFilePath, (On.AssetManager.hook_ResolveFilePath_string)ResolveFilePath));
    }

    public static void Apply()
    {
        _hooks.Add(new Hook((On.AssetManager.orig_ResolveFilePath_string_bool_bool)faster_world.M_Assets.AssetManager_ResolveFilePath, (On.AssetManager.hook_ResolveFilePath_string_bool_bool)AssetManager_ResolveFilePath));
        _hooks.Add(new Hook((On.AssetManager.orig_ResolveFilePath_string)faster_world.M_Assets.ResolveFilePath, (On.AssetManager.hook_ResolveFilePath_string)ResolveFilePath));
    }

    //generic hooks
    public static string AssetManager_ResolveFilePath(On.AssetManager.orig_ResolveFilePath_string_bool_bool orig, string path, bool skipMergedMods = false, bool skipConsoleFiles = false)
    {
        string text;
        if (!skipMergedMods && TurboAssetManager.accessfgmerged && File.Exists(text = FloodgateMergedMods + path))
        {
            return text;
        }
        return orig(path,skipMergedMods,skipConsoleFiles);
    }

    public static string ResolveFilePath(On.AssetManager.orig_ResolveFilePath_string orig, string path)
    {
        string text;
        if(TurboAssetManager.accessfgmerged && File.Exists(text = FloodgateMergedMods + path))
        {
            return text;
        }
        return orig(path);
    }

    public static string FloodgateMergedMods = RWCustom.Custom.RootFolderDirectory() + Path.DirectorySeparatorChar + "floodgatemergedmods" + Path.DirectorySeparatorChar;
}
