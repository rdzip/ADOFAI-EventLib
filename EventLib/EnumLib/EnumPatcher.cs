using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using HarmonyLib;

/*
 * Special thanks to C##
 */

namespace EventLib.EnumLib {
    public static class EnumPatcher<T> where T : Enum {
        private static readonly Func<ulong, T> _convert;

        private static readonly Type _thisType = typeof(T);
        private static readonly Dictionary<string, ulong> _addedFields = new Dictionary<string, ulong>();
        public static T AddField(string name, ulong value)
        {
            _addedFields[name] = value;
            return _convert(value);
        }
        static EnumPatcher()
        {
            Main.harmony.Patch(EnumPatchManager.GCVAN, postfix: new HarmonyMethod(typeof(EnumPatcher<T>).GetMethod(nameof(GCVAN_Patch), (BindingFlags)15420)));
            
            var parameter = Expression.Parameter(typeof(ulong));
            var dynamic_method = Expression.Lambda<Func<ulong, T>>(Expression.Convert(parameter, typeof(T)), parameter);
            _convert = dynamic_method.Compile();
        }
        static void GCVAN_Patch(Type enumType, object __result)
        {
            if (enumType != _thisType) return;
            var names = EnumPatchManager.VAN_Names(__result).Concat(_addedFields.Keys);
            var values = EnumPatchManager.VAN_Values(__result).Concat(_addedFields.Values);
            EnumPatchManager.VAN_Names(__result) = names.ToArray();
            EnumPatchManager.VAN_Values(__result) = values.ToArray();
        }
    }

    internal static class EnumPatchManager {
        public static readonly MethodInfo GCVAN = typeof(Enum).GetMethod("GetCachedValuesAndNames", (BindingFlags)15420);
        public static readonly AccessTools.FieldRef<object, string[]> VAN_Names = AccessTools.FieldRefAccess<string[]>(typeof(Enum).GetNestedType("ValuesAndNames", (BindingFlags)15420), "Names");
        public static readonly AccessTools.FieldRef<object, ulong[]> VAN_Values = AccessTools.FieldRefAccess<ulong[]>(typeof(Enum).GetNestedType("ValuesAndNames", (BindingFlags)15420), "Values");
    }
}