using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JKorTech.ShipSections
{
    public static class SectionDataDictionary
    {
        private class SectionDataLoadingSystem : LoadingSystem
        {
            private bool loaded;
            public override bool IsReady()
            {
                return loaded;
            }
            public override string ProgressTitle()
            {
                return nameof(SectionDataLoadingSystem);
            }
            public override float ProgressFraction()
            {
                return .9f;
            }
            public override void StartLoad()
            {
                Debug.Log($"[{nameof(ShipSections)}] Loading SECTIONDATADEF's into default dictionary.");
                var sectionDataDefinitions = GameDatabase.Instance.GetConfigNodes("SECTIONDATADEF");
                var loadedTypes = LoadTypes();
                foreach (var sectionDataType in loadedTypes)
                {
                    SectionDataTypes.Add(sectionDataType, new ConfigNode());
                    Debug.Log($"[{nameof(ShipSections)}] Loading {sectionDataType.FullName} into the section dictionary");
                }
                foreach (var def in sectionDataDefinitions)
                {
                    if (!def.HasValue("name"))
                    {
                        Debug.LogWarning($"[{nameof(ShipSections)}] Malformed SECTIONDATADEF.");
                    }
                    Debug.Log($"[{nameof(ShipSections)}] Loading SECTIONDATADEF for \"{def.GetValue("name")}\"");
                    SectionDataTypes[loadedTypes.FirstOrDefault(type => type.Name == def.GetValue("name")) ?? typeof(object)] = def;
                }
                SectionDataTypes.Remove(typeof(object));
                loaded = true;
            }
        }

        [KSPAddon(KSPAddon.Startup.Instantly, false)]
        public class LoadingSystemLoader : MonoBehaviour
        {
            void Awake()
            {
                var list = LoadingScreen.Instance.loaders;
                if (list != null)
                {
                    // Need to create a GameObject so that Unity will correctly initialize the LoadingSystem
                    var gameObject = new GameObject(nameof(SectionDataLoadingSystem));
                    var loadingSystem = gameObject.AddComponent<SectionDataLoadingSystem>();
                    list.Add(loadingSystem);
                }
            }
        }

        private static List<Type> LoadTypes()
        {
            var types = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Debug.Log($"[{nameof(ShipSections)}] Searching {assembly.GetName()} for SectionData types");
                try
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        if(type.BaseType != null)
                        {
                            var baseType = type.BaseType;
                            if (baseType.IsGenericType && Equals(baseType.GetGenericTypeDefinition(), typeof(SectionData<>)))
                            {
                                Debug.Log($"Found SectionData type {type.Name}");
                                types.Add(type);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
            return types;
        }

        public static Dictionary<Type, ConfigNode> SectionDataTypes { get; } = new Dictionary<Type, ConfigNode>();
    }
}
