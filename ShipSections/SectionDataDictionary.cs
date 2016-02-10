using System;
using System.Collections.Generic;
using System.Linq;

namespace JKorTech.ShipSections
{
    public static class SectionDataDictionary
    {
        public static void ModuleManagerPostLoad()
        {
            UnityEngine.Debug.Log($"[{nameof(ShipSections)}] Loading SECTIONDATADEF's into default dictionary.");
            var sectionDataDefinitions = GameDatabase.Instance.GetConfigNodes("SECTIONDATADEF");
            var loadedTypes = LoadTypes();
            foreach(var sectionDataType in loadedTypes)
            {
                SectionDataTypes.Add(sectionDataType, new ConfigNode());
                UnityEngine.Debug.Log($"[{nameof(ShipSections)}] Loading {sectionDataType.FullName} into the section dictionary");
            }
            foreach (var def in sectionDataDefinitions)
            {
                if (!def.HasValue("name"))
                {
                    UnityEngine.Debug.LogWarning($"[{nameof(ShipSections)}] Malformed SECTIONDATADEF.");
                }
                UnityEngine.Debug.Log($"[{nameof(ShipSections)}] Loading SECTIONDATADEF for \"{def.GetValue("name")}\"");
                SectionDataTypes[loadedTypes.FirstOrDefault(type => type.Name == def.GetValue("name")) ?? typeof(object)] = def;
            }
            SectionDataTypes.Remove(typeof(object));
        }

        private static List<Type> LoadTypes()
        {
            var types = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                UnityEngine.Debug.Log($"[{nameof(ShipSections)}] Searching {assembly.GetName()} for SectionData types");
                try
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        if(type.BaseType != null)
                        {
                            var baseType = type.BaseType;
                            if (baseType.IsGenericType && Equals(baseType.GetGenericTypeDefinition(), typeof(SectionData<>)))
                            {
                                UnityEngine.Debug.Log($"Found SectionData type {type.Name}");
                                types.Add(type);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogException(ex);
                }
            }
            return types;
        }

        public static Dictionary<Type, ConfigNode> SectionDataTypes { get; } = new Dictionary<Type, ConfigNode>();
    }
}
