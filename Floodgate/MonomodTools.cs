using FloodgatePatcher;
using Mono.Cecil.Cil;
using MonoMod.Cil;

public static partial class FGTools
{

    public static ILCursor EmitOptimized(this ILCursor cursor, OpCode op, int operand)
    {
        if (op == OpCodes.Ldarg || op == OpCodes.Ldarg_S)
        {
            if (operand == 0)
            {
                return cursor.Emit(OpCodes.Ldarg_0);
            }
            else if (operand == 1)
            {
                return cursor.Emit(OpCodes.Ldarg_1);

            }
            else if (operand == 2)
            {
                return cursor.Emit(OpCodes.Ldarg_2);

            }
            else if (operand == 3)
            {
                return cursor.Emit(OpCodes.Ldarg_3);

            }
            else if (operand <= 255)
            {
                return cursor.Emit(OpCodes.Ldarg_S, (byte)operand);
            }
            else
            {
                return cursor.Emit(OpCodes.Ldarg, operand);
            }
        }
        if (op == OpCodes.Ldloc || op == OpCodes.Ldloc_S)
        {
            if (operand == 0)
            {
                return cursor.Emit(OpCodes.Ldloc_0);
            }
            else if (operand == 1)
            {
                return cursor.Emit(OpCodes.Ldloc_1);

            }
            else if (operand == 2)
            {
                return cursor.Emit(OpCodes.Ldloc_2);

            }
            else if (operand == 3)
            {
                return cursor.Emit(OpCodes.Ldloc_3);

            }
            else if (operand <= 255)
            {
                return cursor.Emit(OpCodes.Ldloc_S, (byte)operand);
            }
            else
            {
                return cursor.Emit(OpCodes.Ldloc, operand);
            }
        }
        if (op == OpCodes.Stloc || op == OpCodes.Stloc_S)
        {
            if (operand == 0)
            {
                return cursor.Emit(OpCodes.Stloc_0);
            }
            else if (operand == 1)
            {
                return cursor.Emit(OpCodes.Stloc_1);

            }
            else if (operand == 2)
            {
                return cursor.Emit(OpCodes.Stloc_2);

            }
            else if (operand == 3)
            {
                return cursor.Emit(OpCodes.Stloc_3);

            }
            else if (operand <= 255)
            {
                return cursor.Emit(OpCodes.Stloc_S, (byte)operand);
            }
            else
            {
                return cursor.Emit(OpCodes.Stloc, operand);
            }
        }
        if (op == OpCodes.Ldloca || op == OpCodes.Ldloca_S)
        {
            if (operand <= 255)
            {
                return cursor.Emit(OpCodes.Ldloca_S, (byte)operand);
            }
            else
            {
                return cursor.Emit(OpCodes.Ldloca, operand);
            }
        }
        if (op == OpCodes.Ldarga || op == OpCodes.Ldarga_S)
        {
            if (operand <= 255)
            {
                return cursor.Emit(OpCodes.Ldarga_S, (byte)operand);
            }
            else
            {
                return cursor.Emit(OpCodes.Ldarga, operand);
            }
        }

        CustomLog.LogError("[Optimize Emit]...Wha?\n    " + op.Name + " - " + operand);
        return cursor.Emit(op, operand);
    }
}
