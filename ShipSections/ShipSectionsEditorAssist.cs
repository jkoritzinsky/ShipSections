using System.Linq;
using UnityEngine;

namespace JKorTech.ShipSections
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class ShipSectionsEditorAssit : MonoBehaviour
    {

        void Start()
        {
            SubscribeToEvents();
        }

        protected static void CopySectionDataToOtherPartInSection(SectionInfo info, bool unsetAsRoot)
        {
            if (info.isSectionRoot)
            {
                var otherPartInSection = API.PartsBySection.First(section => section.Key == info.section).FirstOrDefault(part => part != info.part);
                if (otherPartInSection != null)
                {
                    CopySectionData(info, otherPartInSection.FindModuleImplementing<SectionInfo>(), unsetAsRoot);
                }
            }
        }

        private static void CopySectionData(SectionInfo from, SectionInfo to, bool unsetAsRoot)
        {
            if (from == to) return;
            to.dataContainer = from.dataContainer;
            to.isSectionRoot = true;
            from.isSectionRoot = !unsetAsRoot;
        }

        void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        internal void SubscribeToEvents()
        {
            GameEvents.onEditorPartEvent.Add(EditorPartEvent);
        }

        internal void UnsubscribeFromEvents()
        {
            GameEvents.onEditorPartEvent.Remove(EditorPartEvent);
        }


        private void EditorPartEvent(ConstructionEventType data0, Part data1)
        {
            var sectionInfo = data1.FindModuleImplementing<SectionInfo>();
            if (data0 == ConstructionEventType.PartDetached)
            {
                CopySectionDataToOtherPartInSection(sectionInfo, true);
            }
            if (data0 == ConstructionEventType.PartCreated && data1 == EditorLogic.RootPart)
            {
                sectionInfo.isSectionRoot = true;
                sectionInfo.InitializeAsNewSection();
            }
            if(data0 == ConstructionEventType.PartAttached)
            {
                sectionInfo.TrySetSectionBasedOnParent();
                if (EditorLogic.fetch.symmetryMode != 1 && sectionInfo.isSectionRoot)
                    EnsureOnlyOneSectionRoot(sectionInfo);

            }
        }

        private static void EnsureOnlyOneSectionRoot(SectionInfo sectionInfo)
        {
            var sectionName = sectionInfo.section;
            var sectionRoots = API.PartsBySection.First(section => section.Key == sectionName)
                .SelectMany(part => part.FindModulesImplementing<SectionInfo>())
                .Where(info => info.isSectionRoot);
            sectionRoots.Where(info => info != sectionInfo).ToList().ForEach(info =>
            {
                info.dataContainer = new SectionDataContainer();
                info.isSectionRoot = false;
            });
        }
    }
}
