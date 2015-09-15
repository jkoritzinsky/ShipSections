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
            var sectionInfos = CurrentVesselParts.SelectMany(part => part.FindModulesImplementing<SectionInfo>())
                .Where(section => section.section == oldSectionName);
            foreach (var sectionInfo in sectionInfos)
            {
                sectionInfo.section = newSectionName;
            }
            SectionNameChanged.Fire(oldSectionName, newSectionName);
        }

        public static readonly EventData<string, string> SectionNameChanged = new EventData<string, string>("SectionNameChanged");

        public static IEnumerable<string> SectionNames
            => CurrentVesselParts.Select(part => part.FindModuleImplementing<SectionInfo>().section).Distinct();
        public static IEnumerable<IGrouping<string, Part>> PartsBySection
            => CurrentVesselParts.GroupBy(part => part.FindModuleImplementing<SectionInfo>()?.section);
        public static SectionData GetSectionDataForMod(string modID, string sectionName)
            => PartsBySection.Single(section => section.Key == sectionName).Select(part => part.FindModuleImplementing<SectionInfo>())
                    .Single(info => info.isSectionRoot).dataContainer.GetSectionDataForMod(modID);
        public static T GetSectionDataForMod<T>(string modID, string sectionName)
            where T : SectionData => (T)GetSectionDataForMod(modID, sectionName);
    }
}
