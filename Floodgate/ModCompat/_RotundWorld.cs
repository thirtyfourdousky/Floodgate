using FloodgatePatcher;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ModCompat;

public static class _RotundWorld
{
    public static void Apply() {
        InlineIL.IL.Emit.Ldtoken(new InlineIL.MethodRef(typeof(RotundWorld.BPLastWishFixes), "LastWishContent"));
        InlineIL.IL.Pop<RuntimeMethodHandle>(out RuntimeMethodHandle handle);
        HookEndpointManager.Modify(MethodBase.GetMethodFromHandle(handle), IL_Nop);
    }
    public static void IL_Nop(ILContext il)
    {
        try
        {
            il.Instrs.Clear();
            ILCursor c = new(il);
            c.Emit(OpCodes.Nop);
            c.Emit(OpCodes.Ret);
        }
        catch (Exception e)
        {
            CustomLog.LogError(e.ToString());
        }
    }
}
