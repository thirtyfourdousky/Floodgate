using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCompat
{
    public static class EmgTxIteratorCreator
    {
        internal static readonly List<IDetour> _hooks = new();
        public static void Apply()
        {
            _hooks.Add(new Hook(typeof(CustomOracleTx.OracleHoox).GetMethod("OracleGraphicHoox", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic), IL_OracleGraphicHoox));
        }
        public static void IL_OracleGraphicHoox(Action orig)
        {
            IL.OracleGraphics.InitiateSprites -= IteratorCreator.CustomOracleGraphics.OracleGraphics_InitiateSprites;
            orig();
            IL.OracleGraphics.InitiateSprites += IteratorCreator.CustomOracleGraphics.OracleGraphics_InitiateSprites;
        }
    }
}
