using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JKorTech.ShipSections
{
    public static class API
    {
        public static IEnumerable<Part> CurrentVesselParts
        {
            get
            {
                if (HighLogic.LoadedSceneIsEditor)
                {
                    return EditorLogic.SortedShipList;
                }
                else if (HighLogic.LoadedSceneIsFlight)
                {
                    return FlightGlobals.ActiveVessel.Parts;
                }
                else
                {
                    return Enumerable.Empty<Part>();
                }
            }
        }

        public static void ChangeSectionName(string oldSectionName, string newSectionName)
        {
            var sectionInfos = CurrentVesselParts.SelectMany(part => part.FindModulesImplementing<SectionInfo>()).Where(section => section.section == oldSectionName);
            foreach (var sectionInfo in sectionInfos)
            {
                sectionInfo.section = newSectionName;
            }
        }

        public static event Action SectionDataUpdated = () => { };
        
        internal static void SendSectionDataUpdated()
        {
            SectionDataUpdated();
        }
    }
}
