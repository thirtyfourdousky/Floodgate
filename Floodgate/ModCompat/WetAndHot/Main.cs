using FloodgatePatcher;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using WetandHot;

namespace ModCompat.WetAndHot;

public static class Main
{
    public static void Apply()
    {
        IL.LizardSpit.Update += IL_LizardSpitMoisture;

        if (FGTools.IsModActive("lb-fgf-m4r-ik.modpack"))
        {
            try
            {
                M4rblelous.Apply();
            }
            catch (Exception e)
            {
                CustomLog.LogError("Wet and Hot - M4rblelous Entity Pack specific apply failed\n" + e.ToString());
            }
        }
    }

    public static readonly Dictionary<Type, float> LizardSpits = new Dictionary<Type, float>
    {
        { typeof(LizardSpit), .01f }
    };

    public static void IL_LizardSpitMoisture(ILContext il)
    {
        try
        {
            ILCursor c = new(il);
            int loc = -1;
            c.GotoNext(MoveType.After, x => x.MatchStloc(out loc), x => x.MatchLdloc(loc), x => x.MatchLdfld<SharedPhysics.CollisionResult>("chunk"), x => x.MatchBrfalse(out _));
            if (loc == -1) throw new InvalidProgramException("BodyChunk local not found");
            c.Emit(OpCodes.Ldarg_0);
            c.EmitOptimized(OpCodes.Ldloc, loc);
            InlineIL.IL.Emit.Ldtoken(new InlineIL.MethodRef(typeof(Main), "LizardSpitMoisture"));
            InlineIL.IL.Pop(out RuntimeMethodHandle handle);
            c.Emit(OpCodes.Call, System.Reflection.MethodBase.GetMethodFromHandle(handle));
        }
        catch (Exception ex)
        {
            CustomLog.LogError(ex.ToString());
        }
    }

    public static void LizardSpitMoisture(LizardSpit spit, SharedPhysics.CollisionResult collisionResult)
    {
        // NOTE: Neither should EVER be null here, if they are ill ascend myself
        if (LizardSpits.TryGetValue(spit.GetType(), out var m))
        {
            collisionResult.chunk.owner.GetWaHData().Damp += m;
        }
    }
}
