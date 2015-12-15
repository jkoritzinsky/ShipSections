using System.Collections.Generic;
using System.Linq;

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
                    return EditorLogic.fetch.ship;
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

        public static bool AnyCurrentVesel
        {
            get
            {
                if (HighLogic.LoadedSceneIsEditor)
                    return EditorLogic.fetch.ship?.Any() ?? false;
                else if (HighLogic.LoadedSceneIsFlight)
                    return FlightGlobals.ActiveVessel != null;
                else
                    return false;
            }
        }

        public static void ChangeSectionName(string currentSectionName, string newSectionName)
        {
            if (currentSectionName == newSectionName) return;
            if (SectionNames.Contains(newSectionName))
                MergeSections(currentSectionName, newSectionName);
            else
                RenameSection(currentSectionName, newSectionName);
        }

        private static void MergeSections(string currentSection, string newSection)
        {
            var currentSectionParts = PartsBySection.First(section => section.Key == currentSection);
            var currentSectionRootInfo = currentSectionParts.SelectMany(part => part.FindModulesImplementing<SectionInfo>()).First(info => info.isSectionRoot);
            var newSectionParts = PartsBySection.First(section => section.Key == newSection);
            var newSectionRootInfo = newSectionParts.SelectMany(part => part.FindModulesImplementing<SectionInfo>()).First(info => info.isSectionRoot);
            foreach (var sectionData in currentSectionRootInfo.dataContainer.GetAllSectionDatas())
            {
                newSectionRootInfo.dataContainer.AddOrUpdateSectionDataForMod(sectionData);
            }
            currentSectionRootInfo.isSectionRoot = false;
            currentSectionRootInfo.dataContainer = new SectionDataContainer();
            ChangeSectionNameCore(currentSectionParts, newSection);
            SectionsMerged.Fire(currentSection, newSection);
        }


        private static void RenameSection(string oldSectionName, string newSectionName)
        {
            ChangeSectionNameCore(PartsBySection.First(group => group.Key == oldSectionName), newSectionName);
            SectionRenamed.Fire(oldSectionName, newSectionName);
        }

        private static void ChangeSectionNameCore(IEnumerable<Part> parts, string newSectionName)
        {
            foreach (var part in parts)
            {
                SectionInfo info = part.FindModuleImplementing<SectionInfo>();
                if (info != null)
                {
                    info.section = newSectionName;
                }
            }
        }

        public static readonly EventData<string, string> SectionRenamed = new EventData<string, string>(nameof(SectionRenamed));

        public static readonly EventData<string, string> SectionsMerged = new EventData<string, string>(nameof(SectionsMerged));

        public static readonly EventData<Part> PartSectionInitialized = new EventData<Part>(nameof(PartSectionInitialized));

        public static IEnumerable<string> SectionNames
            => CurrentVesselParts.Select(part => part.FindModuleImplementing<SectionInfo>().section).Distinct();
        public static IEnumerable<IGrouping<string, Part>> PartsBySection
            => EnsureSectionRootsExist(CurrentVesselParts.GroupBy(part => part.FindModuleImplementing<SectionInfo>()?.section));
        public static T GetSectionDataForMod<T>(string sectionName)
            where T : SectionDataBase
            => PartsBySection.Single(section => section.Key == sectionName).Select(part => part.FindModuleImplementing<SectionInfo>())
                    .Single(info => info.isSectionRoot).dataContainer.GetSectionData<T>();

        public static IEnumerable<IGrouping<string, Part>> EnsureSectionRootsExist(IEnumerable<IGrouping<string, Part>> sections)
        {
            UnityEngine.Debug.Log("[ShipSections] Ensuring that there is a section root");
            foreach (var section in sections)
            {
                UnityEngine.Debug.Log($"[ShipSections] Checking section {section.Key}");
                if (!section.Any(part => part.FindModuleImplementing<SectionInfo>().isSectionRoot))
                {
                    var info = section.First().FindModuleImplementing<SectionInfo>();
                    info.isSectionRoot = true;
                    info.InitializeAsNewSection();
                }
            }
            return sections;
        }
    }
}
