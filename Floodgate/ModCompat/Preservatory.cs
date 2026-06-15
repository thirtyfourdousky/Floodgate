using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCompat;

public static class Preservatory
{
    public static NativeDetour extendDetour;
    public static void Apply()
    {
        InlineIL.IL.Emit.Ldtoken(new InlineIL.MethodRef(new("PVStuff", "PVStuff.Logic.SaveManager"), "extendSaveIfNecessary"));
        InlineIL.IL.Pop<RuntimeMethodHandle>(out RuntimeMethodHandle handle);
        InlineIL.IL.Emit.Ldtoken(new InlineIL.MethodRef(typeof(Preservatory), "On_PVSaveManager_extendSaveIfNecessary"));
        InlineIL.IL.Pop<RuntimeMethodHandle>(out RuntimeMethodHandle hook);
        extendDetour = new(System.Reflection.MethodBase.GetMethodFromHandle(handle), System.Reflection.MethodBase.GetMethodFromHandle(hook));
    }

    public static void On_PVSaveManager_extendSaveIfNecessary(int accessingNumber)
    {
        InlineIL.IL.Emit.Call(new(new("PVStuff", "PVStuff.Logic.SaveManager"), "get_EscapismEnding"));
        InlineIL.IL.Pop(out HashSet<SlugcatStats.Name>[] escapismEnding);
        if (accessingNumber > escapismEnding.Length - 1)
        {
            Array.Resize(ref escapismEnding, accessingNumber + 1);
            for(int i = 0; i < escapismEnding.Length; i++)
            {
                if (escapismEnding[i] == null)
                {
                    escapismEnding[i] = new HashSet<SlugcatStats.Name>();
                }
            }
        }
        InlineIL.IL.Push(escapismEnding);
        InlineIL.IL.Emit.Call(new(new("PVStuff", "PVStuff.Logic.SaveManager"), "set_EscapismEnding"));
    }
}
