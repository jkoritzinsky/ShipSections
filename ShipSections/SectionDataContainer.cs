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
        public Dictionary<string, List<SectionData>> data = new Dictionary<string, List<SectionData>>();

        public IEnumerable<SectionData> GetSectionDataForMod(string modID) => data.Where(pair => pair.Key == modID).SelectMany(pair => pair.Value);

        public void AddOrUpdateSectionDataForMod(string modID, SectionData sectionData)
        {
            if(data.Any(pair => pair.Key == modID && pair.Value.Any(section => section.sectionName == sectionData.sectionName)))
            {
                data[modID].RemoveAll(section => section.sectionName == sectionData.sectionName);
                data[modID].Add(sectionData);
            }
        }
    }
}
