using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ModCompat;

public static class LastWish
{
    public static void Apply()
    {
        InlineIL.IL.Emit.Ldtoken(new InlineIL.MethodRef(new InlineIL.TypeRef("VoidTemplate", "VoidTemplate._Plugin"), "RainWorld_PostModsInit"));
        InlineIL.IL.Pop<RuntimeMethodHandle>(out RuntimeMethodHandle handle);
        HookEndpointManager.Modify(MethodBase.GetMethodFromHandle(handle), RemoveIncompats);
    }

    public static void RemoveIncompats(ILContext IL)
    {
        try
        {
            ILCursor c = new ILCursor(IL);
            InlineIL.IL.Emit.Ldtoken(new InlineIL.MethodRef(typeof(VoidTemplate.ModsCompatibilty._ModsMeta), "PostModsInit"));
            InlineIL.IL.Pop<RuntimeMethodHandle>(out RuntimeMethodHandle incompatHandle);
            MethodBase incompatMethod = MethodBase.GetMethodFromHandle(incompatHandle);
            c.GotoNext(MoveType.Before, x => x.MatchCall(incompatMethod));
            c.Remove();
        }
        catch (Exception ex)
        {
            FloodgatePatcher.CustomLog.LogError("[LastWish] failed to remove incompatibilities\n" + ex.ToString());
        }
    }
}
