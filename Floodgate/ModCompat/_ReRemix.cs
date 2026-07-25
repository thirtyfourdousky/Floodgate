using FloodgatePatcher;
using InlineIL;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ModCompat;

public static class _ReRemix
{
    //doesn't works yippee
    private static void Apply()
    {
        InlineIL.IL.Emit.Ldtoken(new InlineIL.MethodRef(new("ReRemix", "ReRemix.ReRemix/<>c"), "<ShelterDoor_ctor>b__20_0"));
        InlineIL.IL.Pop(out RuntimeMethodHandle handle);
        HookEndpointManager.Modify(System.Reflection.MethodBase.GetMethodFromHandle(handle), IL_ShelterDoorLocal1);

        InlineIL.IL.Emit.Ldtoken(new InlineIL.MethodRef(new("ReRemix", "ReRemix.ReRemix/<>c"), "<ShelterDoor_ctor>b__20_1"));
        InlineIL.IL.Pop(out RuntimeMethodHandle handle2);
        HookEndpointManager.Modify(System.Reflection.MethodBase.GetMethodFromHandle(handle2), IL_ShelterDoorLocal2);
    }
    public static void IL_ShelterDoorLocal1(ILContext il)
    {
        try
        {
            ILCursor c = new(il);
            c.GotoNext(x => x.MatchLdcI4(16));
            c.Next.Operand = 19;
        }
        catch (Exception ex)
        {
            CustomLog.LogError(ex.ToString());
        }
    }
    public static void IL_ShelterDoorLocal2(ILContext il)
    {
        try
        {
            ILCursor c = new(il);
            c.GotoNext(x => x.MatchLdcI4(17));
            c.Next.Operand = 20;
        }
        catch (Exception ex)
        {
            CustomLog.LogError(ex.ToString());
        }
    }
}
