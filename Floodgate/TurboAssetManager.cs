using FloodgatePatcher;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.Utils;
using RWCustom;
using System.Collections.Generic;
using System.IO;

namespace Floodgate;
//i still don't rember why it's called that
public static class TurboAssetManager
{
    public static void Apply()
    {
        if (!OtherHooks.ResolveFilePathApplied)
        {
            On.AssetManager.ResolveFilePath_string_bool_bool += AssetManager_ResolveFilePath_string_bool_bool;
            OtherHooks.ResolveFilePathApplied = true;
        }
        On.AssetManager.ResolveAllFilePaths += AssetManager_ResolveAllFilePaths;
        On.AssetManager.ResolveDirectory += AssetManager_ResolveDirectory;
        IL.AssetManager.ListDirectory_string_bool_bool_bool += AssetManager_ListDirectory_string_bool_bool_bool;
    }

    internal static string AssetManager_ResolveFilePath_string_bool_bool(On.AssetManager.orig_ResolveFilePath_string_bool_bool orig, string path, bool skipMergedMods, bool skipConsoleFiles)
    {
        if (!accessfgmerged)
        {
            return orig(path, skipMergedMods, skipConsoleFiles);
        }
        path = path.Replace('/', Path.DirectorySeparatorChar);
        if (!skipMergedMods)
        {
            string text = Custom.RootFolderDirectory() + Path.DirectorySeparatorChar + "floodgatemergedmods" + Path.DirectorySeparatorChar + path.ToLowerInvariant();
            if (File.Exists(text))
            {
                return text;
            }
        }
        return orig(path, skipMergedMods, skipConsoleFiles);
    }

    internal static List<string> AssetManager_ResolveAllFilePaths(On.AssetManager.orig_ResolveAllFilePaths orig, string path)
    {
        if (!accessfgmerged)
        {
            return orig(path);
        }
        path = path.Replace('/', Path.DirectorySeparatorChar);
        List<string> list = new List<string>();
        string text = Custom.RootFolderDirectory() + Path.DirectorySeparatorChar + "floodgatemergedmods" + Path.DirectorySeparatorChar + path.ToLowerInvariant();
        if (File.Exists(text))
        {
            list.Add(text);
        }
        list.AddRange(orig(path));
        return list;
    }

    internal static string AssetManager_ResolveDirectory(On.AssetManager.orig_ResolveDirectory orig, string path)
    {
        if (!accessfgmerged)
        {
            return orig(path);
        }
        string text = Custom.RootFolderDirectory() + Path.DirectorySeparatorChar + "floodgatemergedmods" + Path.DirectorySeparatorChar + path.ToLowerInvariant();
        if (Directory.Exists(text))
        {
            return text;
        }
        return orig(path);
    }

    internal static void AssetManager_ListDirectory_string_bool_bool_bool(ILContext il)
	{
		try
		{
			ILCursor c = new(il);
			if(c.TryGotoNext(MoveType.After, x => x.MatchStloc(2)))
			{
				ILLabel label = il.DefineLabel();
				object addOperand = c.Next.Next.Next.Next.Next.Operand;
				object pathcombineOperand = c.Next.Next.Next.Next.Operand;
				object RootFolderDir = c.Next.Next.Operand;
                c.EmitDelegate(() => { return accessfgmerged; });
				c.Emit(OpCodes.Brfalse_S, label);
				c.Emit(OpCodes.Ldloc_2);
				c.Emit(OpCodes.Call, RootFolderDir);
				c.Emit(OpCodes.Ldstr, "floodgatemergedmods");
				c.Emit(OpCodes.Call, pathcombineOperand);
				c.Emit(OpCodes.Callvirt, addOperand);
				c.MarkLabel(label);
			}
			else
			{
				CustomLog.LogError("AssetManager ListDirectory Hook couldn't find injection point");
			}
		}
		catch (System.Exception ex)
		{
			CustomLog.LogError("AssetManager ListDirectory Hook failed\n" + ex.ToString());
		}
	}

    public static bool accessfgmerged = false;


    //
    public static void MergeScan()
    {

    }
}
