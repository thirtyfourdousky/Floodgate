using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FloodgatePatcher;

public static class OtherHooks
{
    internal static readonly List<IDetour> hooks = new();
    public static void Apply()
    {
    }
}
