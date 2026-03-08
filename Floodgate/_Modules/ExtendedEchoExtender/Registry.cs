using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Floodgate._Modules.ExtendedEchoExtender;

public static class Registry
{
    private static readonly string encryptedText = "###ENCRYPTED";
    private static readonly string encryptedHeader = RWCustom.Custom.xorEncrypt(encryptedText, 55 + (int)InGameTranslator.LanguageID.English * 7);

    public static readonly ConditionalWeakTable<global::World, Dictionary<GhostWorldPresence.GhostID, GhostWorldPresence>> ExtraGhosts = new();


    public static readonly Dictionary<GhostWorldPresence.GhostID, EchoSettings> echoesSettings = new();
    internal static readonly HashSet<GhostWorldPresence.GhostID> _extendedEchoesIDs = new();
    internal static readonly Dictionary<Conversation.ID, string> _echoConversations = new();
    internal static readonly Dictionary<string, string> _echoSongs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { "CC", "NA_32 - Else1" },
        { "SI", "NA_38 - Else7" },
        { "LF", "NA_36 - Else5" },
        { "SH", "NA_34 - Else3" },
        { "UW", "NA_35 - Else4" },
        { "SB", "NA_33 - Else2" },
        { "UNUSED", "NA_37 - Else6" }
    };
    public static Conversation.ID GetConvID(string EchoID)
    {
        return new Conversation.ID("EEEGhost_" + EchoID, false);
    }

    public static GhostWorldPresence.GhostID GetEchoID(string ID)
    {
        return new(ID, false);
    }
    public static bool EchoExists(string ID)
    {
        try
        {
            return GetEchoID(ID).index >= 0;
        }
        catch
        {
            return false;
        }
    }

    public static bool HasExtraGhost(this global::World world)
    {
        if(ExtraGhosts.TryGetValue(world, out _))
        {
            return true;
        }

        return false;
    }
    public static GhostWorldPresence ExtraGhost(this global::World world, GhostWorldPresence.GhostID ghostID)
    {
        if(ExtraGhosts.TryGetValue(world,out var extraGhosts) && extraGhosts.TryGetValue(ghostID, out var retv))
        {
            return retv;
        }
        return null;
    }

    public static void LoadAllRegions(SlugcatStats.Name campaign)
    {
        //_extendedEchoesIDs.Clear();
        foreach (string regionshort in Region.GetFullRegionOrder())
        {
            string confpath = AssetManager.ResolveFilePath(Path.Combine("world", regionshort, "eeechoesSettings.txt"));

            if(File.Exists(confpath)) 
            {
                List<EchoSettings> settingsList = FromFile(confpath, campaign, regionshort);
                foreach(EchoSettings settings in settingsList)
                {
                    if (!EchoExists(settings.ID))
                    {
                        _extendedEchoesIDs.Add(new GhostWorldPresence.GhostID(settings.ID, true));
                        _echoConversations.Add(new Conversation.ID("EEEGhost_"+ settings.ID,true),settings.ID);
                        FloodgatePatcher.CustomLog.Log("[Extended Echo] Added echo " + settings.ID);
                    }
                    else
                    {
                        FloodgatePatcher.CustomLog.Log("[Extended Echo] Echo with the ID " + settings.ID + " already exists, skipping");
                    }
                    echoesSettings[GetEchoID(settings.ID)] = settings;
                }
            }
        }
    }

    public static List<EchoSettings> FromFile(string path, SlugcatStats.Name name, string regionshort)
    {
        if (!File.Exists(path))
        {
            FloodgatePatcher.CustomLog.Log("[Extended Echo] no settings file found");
            return [];
        }
        FloodgatePatcher.CustomLog.Log("[Extended Echo] using custom settings from " + path);
        string[] lines = FGTools.ProcessTimelineConditions(File.ReadAllLines(path), name, SlugcatStats.SlugcatToTimeline(name));
        EchoSettings current = new(name);
        List<EchoSettings> echoes = new List<EchoSettings>();
        current.Region = regionshort;
        bool skiplines = false;
        int EOF = 0;
        for(int f = lines.Length - 1; f >= 0; f--)
        {
            if (!string.IsNullOrWhiteSpace(lines[f]))
            {
                EOF = f;
                break;
            }
        }
        for(int i = 0; (i < lines.Length && i <= EOF); i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
            {
                continue;
            }
            string line = lines[i].Trim().ToLowerInvariant().Replace(" :", ":");
            if(line.StartsWith("#") || line.StartsWith("//"))
            {
                continue;
            }
            if (line.StartsWith("/*"))
            {
                skiplines = true;
                continue;
            }
            if(skiplines)
            {
                if (line.EndsWith("*/"))
                {
                    skiplines = false;
                }
                continue;
            }
            if (line.StartsWith("end"))
            {
                echoes.Add(current);
                if (i < lines.Length - 1 || i < EOF)
                {
                    current = new(name);
                    current.Region = regionshort;
                }
                continue;
            }
            if (line.StartsWith("id:"))
            {
                current.ID = line.Split(':')[1].Trim();
                continue;
            }
            if (line.Contains("spawnondifficulty"))
            {
                if (line.Contains(":"))
                {
                    string[] conds = line.Split(':');
                    bool negative = conds[1].StartsWith("x-");
                    if (conds[1].Substring(negative?2:0).Split(',').Contains(name.value.ToLowerInvariant()) ^ negative)
                    {
                        current.SpawnOnDifficulty = true;
                    }
                }
                else
                {
                    current.SpawnOnDifficulty = true;
                }
                continue;
            }
            if (line.StartsWith("room:"))
            {
                current.Room = line.Split(':')[1].Trim();
                continue;
            }
            if(line.StartsWith("song:") || line.StartsWith("echosong:"))
            {
                string customsong = line.Split(':')[1].Trim();
                current.Song = string.IsNullOrWhiteSpace(customsong) ? "NA_32 - Else1" : customsong;
                continue;
            }
            if (line.StartsWith("minkarma:"))
            {
                try
                {
                    current.MinKarma = Convert.ToInt32(line.Split(':')[1].Trim());
                }
                catch (Exception e)
                {
                    FloodgatePatcher.CustomLog.LogError("[Extended Echo] Error occurred while reading Minimum Karma on " + path + "\n" + e.ToString());
                }
                continue;
            }
            if (line.StartsWith("minkarmacap:"))
            {
                try
                {
                    current.MinKarma = Convert.ToInt32(line.Split(':')[1].Trim());
                }
                catch (Exception e)
                {
                    FloodgatePatcher.CustomLog.LogError("[Extended Echo] Error occurred while reading Minimum Karma Cap on " + path + "\n" + e.ToString());
                }
                continue;
            }
            if (line.StartsWith("effectradius:") || line.StartsWith("radius:"))
            {
                try
                {
                    current.EffectRadius = float.Parse(line.Split(':')[1].Trim());
                }
                catch (Exception e)
                {
                    FloodgatePatcher.CustomLog.LogError("[Extended Echo] Error occurred while reading Effect Radius on " + path + "\n" + e.ToString());
                }
                continue;
            }
            if (line.StartsWith("sizemultiplier:"))
            {
                try
                {
                    current.SizeMultiplier = float.Parse(line.Split(':')[1].Trim());
                }
                catch (Exception e)
                {
                    FloodgatePatcher.CustomLog.LogError("[Extended Echo] Error occurred while reading Size Multiplier on " + path + "\n" + e.ToString());
                }
                continue;
            }
            if (line.StartsWith("flip:") || line.StartsWith("defaultflip:"))
            {
                try
                {
                    current.Flip = float.Parse(line.Split(':')[1].Trim());
                }
                catch (Exception e)
                {
                    FloodgatePatcher.CustomLog.LogError("[Extended Echo] Error occurred while reading " + path + " at " + i + "\n" + e.ToString());
                }
                continue;
            }
            if (line.Contains("priming:"))
            {
                string custompriming = line.Split(':')[1].Trim();
                if (custompriming == "true" || custompriming == "yes" || custompriming == "enabled")
                {
                    current.Priming = EchoSettings.PrimingType.Regular;
                }
                else if (custompriming == "false" || custompriming == "no" || custompriming == "none" || custompriming == "disabled")
                {
                    current.Priming = EchoSettings.PrimingType.None;
                }
                else if (custompriming == "saint")
                {
                    current.Priming = EchoSettings.PrimingType.Saint;
                }
                continue;
            }
            if (line.Contains("specificconv") || line.Contains("specialconv"))
            {
                if (line.Contains(":"))
                {
                    string conds = line.Split(':')[1].Trim();
                    bool negative = conds.StartsWith("x-");
                    if (conds.Substring(negative ? 2 : 0).Split(',').Contains(name.value.ToLowerInvariant()) ^ negative)
                    {
                        current.SpecificConv = true;
                    }
                }
                else
                {
                    current.SpecificConv = true;
                }
                continue;
            }
        }
        if(!echoes.Contains(current))
        {
            FloodgatePatcher.CustomLog.Log("[Extended Echo] Echo list was missing last modified echo, assuming no [end] line was found in " + path + ", adding last modified item");
            echoes.Add(current);
        }
        return echoes;
    }
    
    public static string? ManageXOREncryption(string path)
    {
        if (!File.Exists(path))
        {
            return null;
        }
        string plaintext = File.ReadAllText(path);
        if (plaintext.StartsWith(encryptedHeader))
        {
            string encrtext = RWCustom.Custom.xorEncrypt(plaintext, 55 + (int)InGameTranslator.LanguageID.English * 7);
            if(encrtext.StartsWith(encryptedText))
            {
                encrtext = encrtext.Substring(encryptedText.Length, encrtext.Length - encryptedText.Length);
            }
            File.WriteAllText(path, encrtext);
            return encrtext;
        }
        return plaintext;
    }
}
