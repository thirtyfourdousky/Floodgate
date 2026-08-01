using CustomRegions.Mod;
using FloodgatePatcher;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using System;
using System.Collections.Generic;

namespace ModCompat;

public static class DroughtStuff
{
    public static void Apply()
    {
        InlineIL.IL.Emit.Ldtoken(new InlineIL.MethodRef(new("Rain World Drought", "Rain_World_Drought.OverWorld.ModOverwritePreprocessor"), "ModOverwrite"));
        InlineIL.IL.Pop(out RuntimeMethodHandle handle);
        HookEndpointManager.Modify(System.Reflection.MethodBase.GetMethodFromHandle(handle), (ILContext.Manipulator)IL_ModOverwrite);
    }

    public static void IL_ModOverwrite(ILContext il)
    {
        try
        {
            ILCursor c = new(il);
            int pathlocal = -1;
            c.GotoNext(MoveType.After, x => x.MatchCall(typeof(WorldLoader), "FindRoomFile"), x => x.MatchStloc(out pathlocal));
            if(pathlocal == -1) throw new KeyNotFoundException("Somehow the path index couldn't be found");
            c.Emit(OpCodes.Ldarg_0);
            c.EmitOptimized(OpCodes.Ldloc, pathlocal);
            c.EmitDelegate<Func<string, string, bool>>(static delegate (string condition, string path)
            {
                return path.IndexOf("FloodgateMergedMods", StringComparison.OrdinalIgnoreCase) >= 0;
            });
            ILLabel label = c.DefineLabel();
            c.Emit(OpCodes.Brfalse_S, label);
            c.Emit(OpCodes.Ldarg_0);
            c.EmitOptimized(OpCodes.Ldloc, pathlocal);
            c.EmitDelegate<Func<string, string, bool?>>(static delegate (string condition, string path)
            {
                if (WorldLoader.FindRoomFile(condition + "Drought", false, ".txt", true).IndexOf("FloodgateMergedMods", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    CustomRegionsMod.CustomLog("DroughtOverwrite for room [" + condition + "] was found in Floodgate, so its overwrite version will be used");
                    return true;
                }
                else
                {
                    CustomRegionsMod.CustomLog("DroughtOverwrite for room [" + condition + "] was found in Floodgate, so won't overwrite");
                    return false;
                }
            });
            c.Emit(OpCodes.Ret);
            label.Target = c.Next;
        }
        catch (Exception ex)
        {
            CustomLog.LogError("Drought's Conditional Mod Overwrite failed. Your regions WILL break. Please report this\n" + ex);
        }
    }
}
