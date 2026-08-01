using FloodgatePatcher;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace Floodgate.Optimization;

public static class _ConfigContainer
{
    public static void Apply()
    {
        IL.Menu.Remix.ConfigContainer._GetSoundFill += ConfigContainer__GetSoundFill;
    }

    //this is to avoid the exception, it's bad for performance and ruins debugging
    private static void ConfigContainer__GetSoundFill(ILContext il)
    {
        try
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.After, x => x.MatchLdfld(typeof(ProcessManager), "menuMic"));

            ILLabel label = c.DefineLabel();

            c.Emit(OpCodes.Dup);
            c.Emit(OpCodes.Ldnull);
            c.Emit(OpCodes.Bne_Un_S, label);
            c.Emit(OpCodes.Pop);
            c.Emit(OpCodes.Ldc_I4_0);
            c.Emit(OpCodes.Ret);
            label.Target = c.Next;
        }
        catch (Exception ex)
        {
            CustomLog.LogError("ConfigContainer GetSoundFill hook failed\n" + ex.ToString());
        }
    }
}
