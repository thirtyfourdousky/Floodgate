using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Floodgate.NotEnums;

public static class EnabledMods
{
    public static bool LBmergedMods { get; internal set; } = false;
    public static bool ShroudedAssemblySpecific {  get; internal set; } = false;
    public static bool LuminousCode {  get; internal set; } = false;
    public static bool SnootShootNoot { get; internal set; } = false;
    public static bool Scroungers { get; internal set; } = false;
    public static bool Vanguard { get; internal set; } = false;

    public static void Apply()
    {
        LuminousCode = IsModActive("sequoia.luminouscode");
        SnootShootNoot = IsModActive("myr.moss_fields");
        Scroungers = IsModActive("shrimb.scroungers");
        LBmergedMods = IsModActive("lb-fgf-m4r-ik.modpack");
        ShroudedAssemblySpecific = IsModActive("com.rainworldgame.shroudedassembly.plugin");
        Vanguard = IsModActive("pkuyo.thevanguard");
    }

    public static bool IsModActive(string ID)
    {
        return (ModManager.ActiveMods.Any(i => i.id == ID));
    }
}
