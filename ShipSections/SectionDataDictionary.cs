using System;
using System.Collections.Generic;
using System.Linq;

namespace JKorTech.ShipSections
{
    public static class SectionDataDictionary
    {
        public static void ModuleManagerPostLoad()
        {
            UnityEngine.Debug.Log("[ShipSections] Loading SECTIONDATADEF's into default dictionary.");
            var sectionDataDefinitions = GameDatabase.Instance.GetConfigNodes("SECTIONDATADEF");
            var loadedTypes = LoadTypes().ToList();
            foreach(var sectionDataType in loadedTypes)
            {
                SectionDataTypes.Add(sectionDataType, new ConfigNode());
            }
            foreach (var def in sectionDataDefinitions)
            {
                if (!def.HasValue("name"))
                {
                    UnityEngine.Debug.LogWarning("[ShipSections] Malformed SECTIONDATADEF.");
                }
                UnityEngine.Debug.Log($"[ShipSections] Loading SECTIONDATADEF for \"{def.GetValue("name")}\"");
                SectionDataTypes[loadedTypes.FirstOrDefault(type => type.Name == def.GetValue("name")) ?? typeof(object)] = def;
            }
            SectionDataTypes.Remove(typeof(object));
        }

        private static IEnumerable<Type> LoadTypes()
        {
            return from t in AppDomain.CurrentDomain.GetAssemblies().SelectMany(assem => assem.GetTypes())
                   let baseType = t.BaseType
                   where baseType != null
                   where baseType.IsGenericType && Equals(baseType.GetGenericTypeDefinition(), typeof(SectionData<>))
                   select t;
        }

        public static Dictionary<Type, ConfigNode> SectionDataTypes { get; } = new Dictionary<Type, ConfigNode>();
    }
}
