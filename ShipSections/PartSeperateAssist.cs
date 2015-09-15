using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace JKorTech.ShipSections
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class PartSeperateAssist : MonoBehaviour
    {
        void Awake()
        {
            GameEvents.onPartJointBreak.Add(PartJointBroken);
        }

        private static void PartJointBroken(PartJoint data)
        {
            var info = data.Host.FindModuleImplementing<SectionInfo>();
            if (info.isSectionRoot)
            {
                var otherPartInSection = API.PartsBySection.First(section => section.Key == info.section).FirstOrDefault(part => part != data.Host);
                if (otherPartInSection != null)
                {
                    TransferSectionData(info, otherPartInSection.FindModuleImplementing<SectionInfo>());
                }
            }
        }

        private static void TransferSectionData(SectionInfo from, SectionInfo to)
        {
            if (from == to) return;
            to.dataContainer = from.dataContainer;
            to.isSectionRoot = true;
            from.isSectionRoot = false;
        }

        void OnDestroy()
        {
            GameEvents.onPartJointBreak.Remove(PartJointBroken);
        }
    }
}
