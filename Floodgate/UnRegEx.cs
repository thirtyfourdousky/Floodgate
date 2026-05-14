using FloodgatePatcher;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace Floodgate;

public static class UnRegEx
{
    public static void unregex(ILContext IL)
    {
        try
        {
            ILCursor c = new ILCursor(IL);
            System.Reflection.MethodInfo RESplit = typeof(System.Text.RegularExpressions.Regex).GetMethod("Split", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic, null, new Type[] { typeof(string), typeof(string) }, null);
            //List<Instruction> targets = c.Instrs.Where(i => i.MatchCall<System.Text.RegularExpressions.Regex>("Split")).ToList();
            List<Instruction> targets = c.Instrs.Where(i => i.MatchCall(RESplit)).ToList();
            if (targets.Count == 0)
            {
                CustomLog.Log("[UnRegEx] no regex calls on method " + IL.Method.FullName);
            }
            int count = 0;
            for (int i = 0; i < targets.Count; i++)
            {
                c.Goto(targets[i]);
                IEnumerable<ILLabel> labels = c.IncomingLabels;
                c.Remove();
                c.EmitDelegate(internalsplit);
                foreach(ILLabel label in labels)
                {
                    label.Target = c.Prev;
                }
                count++;
            }
            CustomLog.Log("[UnRegEx] replaced " + count + " instructions on " + IL.Method.FullName);
        }
        catch (Exception ex)
        {
            CustomLog.LogError("[UnRegEx] fucked up for "+ IL.Method.FullName +"\n" + ex.ToString());
        }
    }
    public static string[] internalsplit(string _string, string pattern)
    {
        return _string.Split(new string[] { pattern }, StringSplitOptions.None);
    }
}
