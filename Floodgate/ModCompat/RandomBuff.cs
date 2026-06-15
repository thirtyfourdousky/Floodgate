using FloodgatePatcher;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCompat;

public class RandomBuff
{
    public static void Apply()
    {
        InlineIL.IL.Emit.Ldtoken(new InlineIL.MethodRef(typeof(global::RandomBuff.Core.Progression.CosmeticUnlock), "Init"));
        InlineIL.IL.Pop<RuntimeMethodHandle>(out RuntimeMethodHandle handle);
        HookEndpointManager.Modify(System.Reflection.MethodBase.GetMethodFromHandle(handle), IL_CosmeticUnlock_Init);
    }

    public static void IL_CosmeticUnlock_Init(ILContext IL)
    {
        try
        {
            ILCursor c = new(IL);
            c.GotoNext(MoveType.Before, c => c.MatchLdstr("buffassets//assetbundles//futileExtend//grown.obj"));
            c.Next.Operand = "buffassets/assetbundles/futileExtend/grown.obj";
        }
        catch (Exception e)
        {
            CustomLog.LogError("Random Buff CosmeticUnlock Init hook failed\n" + e.ToString());
        }
    }
}
