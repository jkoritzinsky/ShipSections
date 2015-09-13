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

        public IEnumerable<SectionData> GetSectionDataForMod(string modID) => data.Where(pair => pair.Key == modID).Select(pair => pair.Value);
    }
}
