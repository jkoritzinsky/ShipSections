using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace JKorTech.ShipSections
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class PartSeperateAssistEditor : PartSeperateAssist
    {
        internal override void SubscribeToEvents()
        {
            GameEvents.onEditorPartEvent.Add(EditorPartEvent);
        }

        internal override void UnsubscribeFromEvents()
        {
            GameEvents.onEditorPartEvent.Remove(EditorPartEvent);
        }


        private void EditorPartEvent(ConstructionEventType data0, Part data1)
        {
            if (data0 == ConstructionEventType.PartDetached)
            {
                var info = data1.FindModuleImplementing<SectionInfo>();
                CopySectionDataToOtherPartInSection(info, true);
            }
        }
    }

    public abstract class PartSeperateAssist : MonoBehaviour
    {
        void Start()
        {
            Debug.Log("{SS} Starting PartSeperateAssist.");
            SubscribeToEvents();
        }

        internal abstract void SubscribeToEvents();
        internal abstract void UnsubscribeFromEvents();

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
    }
}
