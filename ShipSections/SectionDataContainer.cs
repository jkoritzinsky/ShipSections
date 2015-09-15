using KSPPluginFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JKorTech.ShipSections
{
    public class SectionDataContainer : ConfigNodeStorage
    {
        [Persistent]
        public Dictionary<string, SectionData> data = new Dictionary<string, SectionData>();

        public void AddOrUpdateSectionDataForMod(string modID, SectionData sectionData)
        {
            if (!data.ContainsKey(modID))
            {
                data.Add(modID, sectionData);
            }
            else
            {
                data.Remove(modID);
                data.Add(modID, sectionData);
            }
        }

        public SectionData GetSectionDataForMod(string modID) => data.First(pair => pair.Key == modID).Value;
    }
}
