using System;
using System.Collections.Generic;

namespace JKorTech.ShipSections
{
    public static class SectionDataDictionary
    {
        public static void ModuleManagerPostLoad()
        {
            var sectionDataDefinitions = GameDatabase.Instance.GetConfigNodes("SECTIONDATADEF");
            foreach (var def in sectionDataDefinitions)
            {
                if (!def.HasValue("type"))
                {
                    UnityEngine.Debug.LogWarning("[ShipSections] Malformed SECTIONDATADEF.");
                }
                var sectionDataType = AssemblyLoader.GetClassByName(typeof(SectionDataBase), def.GetValue("type"));
            }
        }

        public static readonly Dictionary<string, Type> sectionDataTypes = new Dictionary<string, Type>();
    }
}
