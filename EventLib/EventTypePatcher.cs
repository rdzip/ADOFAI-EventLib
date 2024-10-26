using ADOFAI;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EventLib
{
	public static class EventTypePatcher
	{
		static Dictionary<string, ulong> enums = new Dictionary<string, ulong>();
		static string[] sortedNames;
		static ulong[] sortedValues;

		static EventTypePatcher()
		{
			foreach (var type in Enum.GetValues(typeof(LevelEventType)))
			{
				enums.Add(((LevelEventType)type).ToString(), (ulong)(int)type);
			}
			SortNamesAndValues();
		}

		static void SortNamesAndValues()
		{
			sortedNames = enums.Keys.ToArray();
			sortedValues = enums.Values.ToArray();
			Array.Sort(sortedValues.ToArray(), sortedNames.ToArray(), Comparer<ulong>.Default);
		}

		public static void AddField(string name, ulong value)
		{
			enums.Add(name, value);
			SortNamesAndValues();

			Type enumType = Assembly.GetAssembly(typeof(LevelEventType)).GetType("ADOFAI.LevelEventType");
			Type valuesAndNames = typeof(Enum).GetNestedType("ValuesAndNames", AccessTools.all);

			enumType.GetType().Field("GenericCache").SetValue(enumType, Activator.CreateInstance(valuesAndNames, new object[] { sortedValues, sortedNames }));
		}
	}
}
