using FloodgatePatcher;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Floodgate.Optimization.CRS;

public static class RegionProperties
{
    public static readonly List<IDetour> _hooks = new List<IDetour>();
    public static void Apply()
    {
        _hooks.Add(new ILHook(typeof(global::CustomRegions.RegionProperties.RegionProperties.RawProperties).GetConstructor(Array.Empty<Type>()), IL_RawProperties_ctor));
    }
    public static void IL_RawProperties_ctor(ILContext IL)
    {
        try
        {
            ILCursor c = new ILCursor(IL);
            var sizeddictionaryctor = typeof(Dictionary<string, string>).GetConstructor([typeof(int)]);
            while(c.TryGotoNext(MoveType.Before, x=>x.MatchNewobj<Dictionary<string, string>>()))
            {
                c.Remove();
                c.Emit(OpCodes.Ldc_I4, 150); //something something resizing dictionaries hurt. this might be RAM intensive
                c.Emit(OpCodes.Newobj, IL.Import(sizeddictionaryctor));
            }
            IL.Body.OptimizeMacros();
        }
        catch (Exception ex)
        {
            CustomLog.LogError(ex.ToString());
        }
    }

}
