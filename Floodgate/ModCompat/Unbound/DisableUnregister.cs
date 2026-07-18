using FloodgatePatcher;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unbound;

namespace ModCompat._Unbound;

//GOD FUCKING DAMMIT
public static class DisableUnregister
{
    public static ILContext.Manipulator NopHolder = new(IL_Nop);
    public static void Apply()
    {
        try
        {
            InlineIL.IL.Emit.Ldtoken(new InlineIL.MethodRef(typeof(UnboundEnums), "ModoffUnregister"));
            InlineIL.IL.Pop(out RuntimeMethodHandle ModOffUnregister);
            HookEndpointManager.Modify(System.Reflection.MethodBase.GetMethodFromHandle(ModOffUnregister), NopHolder);
            InlineIL.IL.Emit.Ldtoken(new InlineIL.MethodRef(typeof(UnboundEnums), "FullUnregister"));
            InlineIL.IL.Pop(out RuntimeMethodHandle FullUnregister);
            HookEndpointManager.Modify(System.Reflection.MethodBase.GetMethodFromHandle(FullUnregister), NopHolder);

        }
        catch (Exception ex)
        {
            CustomLog.LogError(ex.ToString());
        }
    }
    public static void IL_Nop(ILContext il)
    {
        try
        {
            il.Instrs.Clear();
            ILCursor c = new(il);
            c.Goto(0);
            c.Emit(OpCodes.Nop);
            c.Emit(OpCodes.Ret);
        }
        catch (Exception ex)
        {
            CustomLog.LogError(ex.ToString());
        }
    }
}
