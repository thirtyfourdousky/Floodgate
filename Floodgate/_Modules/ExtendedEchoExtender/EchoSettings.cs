using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Floodgate._Modules.ExtendedEchoExtender;

public struct EchoSettings
{
    public string Room;
    public float SizeMultiplier;
    public float EffectRadius;
    public int MinKarma;
    public int MinKarmaCap;
    public PrimingType Priming;
    public string Song;
    public bool SpawnOnDifficulty = true;
    public float Flip;
    public string ID;
    public string Region;
    public bool SpecificConv = false;
    
    public enum PrimingType
    {
        None,
        Regular,
        Saint
    }

    public EchoSettings(SlugcatStats.Name name)
    {
        Room = "";
        SizeMultiplier = 1f;
        EffectRadius = 4f;
        MinKarma = -1;
        MinKarmaCap = 0;
        Priming = ((name == SlugcatStats.Name.Red) ? PrimingType.None : ((name == MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Saint) ? PrimingType.Saint : PrimingType.Regular));
        Song = "NA_32 - Else1";
        SpawnOnDifficulty = true;
        Flip = 0f;
        ID = "NoGhost";
        Region = "isurehopethisisntaregion";
        SpecificConv = false;
    }

    public EchoSettings()
    {
        throw new NotImplementedException("Do not use this one");
    }

    public bool KarmaCondition(int karma, int karmaCap)
    {
        if (MinKarma == -1)
        {
            if(karmaCap < 4)
            {
                return karma >= karmaCap;
            }
            if(karmaCap >= 4 &&  karmaCap <= 5)
            {
                return karma >= 4;
            }else if(karmaCap == 6)
            {
                return karma >= 5;
            }
            else
            {
                return karma >= 6;
            }
        }
        return karma >= MinKarma;
    }
}
