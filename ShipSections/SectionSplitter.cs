﻿using System.Linq;

namespace JKorTech.ShipSections
{
    [KSPModule("Section Splitter")]
    public class SectionSplitter : SectionInfo
    {
        private const string NewSectionPrefix = "New Section ";

        [KSPEvent(guiActiveEditor = true, guiName = "Create new section", guiActive = false)]
        public void CreateNewSection()
        {
            SetNewSection(part, section, GetNewSectionName());
            InitializeAsNewSection();
            isSectionRoot = true;
            foreach (var symmetryPart in part.symmetryCounterparts)
            {
                SetNewSection(symmetryPart, symmetryPart.FindModuleImplementing<SectionInfo>().section, section);
            }
            API.NewSectionCreated.Fire(section);
        }

        private static string GetNewSectionName()
        {
            var currentSections = API.SectionNames.Where(section => section.StartsWith(NewSectionPrefix))
                .Select(section => section.Substring(NewSectionPrefix.Length))
                .Select(section =>
                {
                    int id;
                    if (int.TryParse(section, out id))
                        return id;
                    return 0;
                }).OrderBy(sectionId => sectionId);
            var lastIndex = currentSections.LastOrDefault();
            return NewSectionPrefix + (lastIndex + 1);
        }

        private static void SetNewSection(Part part, string oldSectionName, string newSectionName)
        {
            var section = part.FindModuleImplementing<SectionInfo>();
            if(section?.section == oldSectionName)
            {
                section.section = newSectionName;
                foreach (var child in part.children)
                {
                    SetNewSection(child, oldSectionName, newSectionName);
                }
            }
        }
    }
}
