
using FloodgatePatcher;
using MonoMod.Cil;
using System;
using System.Linq;

public static class FGTools
{
    public static bool IsModActive(string id)
    {
        for(int i = 0; i < ModManager.ActiveMods.Count; i++)
        {
            if(ModManager.ActiveMods[i].id == id) return true;
        }
        return false;
    }
    public static string[] ProcessTimelineConditions(string[] lines, SlugcatStats.Timeline slug)
    {
        string remove = "___";
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Length >= 1 && lines[i][0] == '(' && lines[i].Contains(')'))
            {
                string text = lines[i].Substring(1, lines[i].IndexOf(")") - 1);
                lines[i] = ((!StringMatchesTimeline(text, slug)) ? remove : lines[i].Substring(lines[i].IndexOf(")") + 1));
            }
        }
        return lines.Where((string x) => x != remove).ToArray();
    }
    public static string[] ProcessTimelineConditions(string[] lines, SlugcatStats.Name slug)
    {
        string remove = "___";
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Length >= 1 && lines[i][0] == '(' && lines[i].Contains(')'))
            {
                string text = lines[i].Substring(1, lines[i].IndexOf(")") - 1);
                lines[i] = ((!StringMatchesTimeline(text, slug)) ? remove : lines[i].Substring(lines[i].IndexOf(")") + 1));
            }
        }
        return lines.Where((string x) => x != remove).ToArray();
    }
    public static string[] ProcessTimelineConditions(string[] lines, SlugcatStats.Name slug, SlugcatStats.Timeline timeline)
    {
        string remove = "___";
        for (int i = 0; i < lines.Length; i++)
        {
            string trimmedline = lines[i].Trim();
            if (trimmedline.Length >= 1 && trimmedline[0] == '(' && trimmedline.Contains(')'))
            {
                string text = trimmedline.Substring(1, trimmedline.IndexOf(")") - 1);
                lines[i] = ((!(StringMatchesTimeline(text, slug) || StringMatchesTimeline(text,timeline)) ? remove : trimmedline.Substring(trimmedline.IndexOf(")") + 1)));
            }
        }
        return lines.Where((string x) => x != remove).ToArray();
    }
    public static bool StringMatchesTimeline(string text, SlugcatStats.Timeline slug)
    {
        bool found = false;
        bool negative = false;
        if (text.StartsWith("X-"))
        {
            text = text.Substring(2);
            negative = true;
        }
        if (slug == null)
        {
            return negative;
        }
        string[] array = text.Split(',');
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].Trim() == slug.ToString())
            {
                found = true;
                break;
            }
        }
        return negative != found;
    }
    public static bool StringMatchesTimeline(string text, SlugcatStats.Name slug)
    {
        bool found = false;
        bool negative = false;
        if (text.StartsWith("X-"))
        {
            text = text.Substring(2);
            negative = true;
        }
        if (slug == null)
        {
            return negative;
        }
        string[] array = text.Split(',');
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].Trim() == slug.ToString())
            {
                found = true;
                break;
            }
        }
        return negative != found;
    }

    public static string PrintInstrs(this ILCursor cursor)
    {
        cursor.Goto(0);
        return cursor.ToString();
    }
}
