using FloodgatePatcher;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using System;

namespace ModCompat;

public static class EmeraldTweaks
{
    //doesn't works yippee
    private static void Apply()
    {
        InlineIL.IL.Emit.Ldtoken(new InlineIL.MethodRef(new("EmeraldsTweaksRemix", "EmeraldsTweaksRemix.WorldTweaks/<>c"), "<WarpPointSuckInCreaturesILHook>b__1_0"));
        InlineIL.IL.Pop(out RuntimeMethodHandle handle);
        HookEndpointManager.Modify(System.Reflection.MethodBase.GetMethodFromHandle(handle), IL_SuckInCreaturesFix);
    }
    public static void IL_SuckInCreaturesFix(ILContext il)
    {
        try
        {
            ILCursor c = new(il);
            c.GotoNext(x => x.MatchLdcI4(6));
            c.Next.Operand = 9;
        }
        catch (Exception ex)
        {
            CustomLog.LogError(ex.ToString());
        }
    }
}
