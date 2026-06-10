using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.RuntimeDetour.HookGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FloodgatePatcher;

public static class OtherHooks
{
    internal static readonly List<IDetour> hooks = new();

    public static bool ResolveFilePathApplied = false;

    public static void Apply()
    {
        HookEndpointManager.OnAdd += HookEndpointManager_OnAdd;
        HookEndpointManager.OnModify += HookEndpointManager_OnModify;
    }

    //resolve file path stuff
    public static On.AssetManager.hook_ResolveFilePath_string_bool_bool ResolveFilePathAlt = ResolveFilePath;

    public static string ResolveFilePath(On.AssetManager.orig_ResolveFilePath_string_bool_bool orig, string path, bool skipMergedMods, bool skipConsoleFiles)
    {
        return orig(path, skipMergedMods, skipConsoleFiles);
    }

    public static string ResolveFilePathHolder(On.AssetManager.orig_ResolveFilePath_string_bool_bool orig, string path, bool skipMergedMods, bool skipConsoleFiles)
    {
        return ResolveFilePathAlt(orig, path, skipMergedMods, skipConsoleFiles);
    }

    private static bool HookEndpointManager_OnModify(System.Reflection.MethodBase arg1, Delegate arg2)
    {
        if(!ResolveFilePathApplied && arg1.Name == "ResolveFilePath" && arg1.GetParameters().Length == 3 && arg2.Method.DeclaringType.FullName != "Floodgate.TurboAssetManager")
        {
            On.AssetManager.ResolveFilePath_string_bool_bool += ResolveFilePathHolder;
            ResolveFilePathApplied = true;
            CustomLog.Log("[HOOK DEBUG] " + arg1.Name + " - " + arg2.Method.Name);
        }
        return true;
    }

    private static bool HookEndpointManager_OnAdd(System.Reflection.MethodBase arg1, Delegate arg2)
    {
        if (!ResolveFilePathApplied && arg1.Name == "ResolveFilePath" && arg1.GetParameters().Length == 3 && arg2.Method.DeclaringType.FullName != "Floodgate.TurboAssetManager")
        {
            On.AssetManager.ResolveFilePath_string_bool_bool += ResolveFilePathHolder;
            ResolveFilePathApplied = true;
            CustomLog.Log("[HOOK DEBUG] " + arg1.Name + " - " + arg2.Method.Name);
        }
        return true;
    }
}
