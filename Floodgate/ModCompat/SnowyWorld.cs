using FloodgatePatcher;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.RuntimeDetour.HookGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCompat;

public static class SnowyWorld
{
    public static NativeDetour native_GetDesiredCycleLength;
    public static void Apply()
    {
        InlineIL.IL.Emit.Ldtoken(new InlineIL.MethodRef(new("SnowyWorld", "SnowyWorld.WorldModify"), "RainCycle_GetDesiredCycleLength"));
        InlineIL.IL.Pop(out RuntimeMethodHandle GetDesiredCycleLength);
        InlineIL.IL.Emit.Ldtoken(new InlineIL.MethodRef(typeof(ModCompat.SnowyWorld), "Native_RainCycle_GetDesiredCycleLength"));
        InlineIL.IL.Pop(out RuntimeMethodHandle GetDesiredCycleLengthDetour);
        native_GetDesiredCycleLength = new(System.Reflection.MethodBase.GetMethodFromHandle(GetDesiredCycleLength), System.Reflection.MethodBase.GetMethodFromHandle(GetDesiredCycleLengthDetour));
        //HookEndpointManager.Modify(System.Reflection.MethodBase.GetMethodFromHandle(GetDesiredCycleLength), IL_RainCycle_GetDesiredCycleLength);
    }
    /*
    public static void IL_RainCycle_GetDesiredCycleLength(ILContext IL)
    {
        try
        {
            ILCursor c = new(IL);
            c.GotoNext(MoveType.Before, x => x.MatchStloc(0));
            c.Emit(OpCodes.Ldarg_1);
            c.EmitDelegate(delegate (RainCycle self)
            {
                return self.world is null;
            });
            ILLabel label = c.DefineLabel();
            c.Emit(OpCodes.Brfalse_S, label);
            c.Emit(OpCodes.Ret);
            c.MarkLabel(label);
        }
        catch (Exception ex)
        {
            CustomLog.LogError("Winter's End RainCycle_GetDesiredCycleLength fix failed\n" + ex.ToString());
        }
    }*/

    public static int Native_RainCycle_GetDesiredCycleLength(On.RainCycle.orig_GetDesiredCycleLength orig, RainCycle self)
    {
        return (self.world is not null && self.world.game.IsStorySession && self.world.region.name == "Z5") ? 99999 : orig(self);
    }
}
