using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JKorTech.ShipSections
{
    [KSPModule("Section Information")]
    public class SectionInfo : PartModule
    {
        private static readonly string DefaultSection = "";
        [KSPField(guiActive = false, guiActiveEditor = true, guiName = "Section", isPersistant = true)]
        public string section = DefaultSection;

        public override void OnStart(StartState state)
        {
            if(state != StartState.None && section == DefaultSection)
            {
                section = part.parent?.FindModuleImplementing<SectionInfo>()?.section ?? DefaultSection;
            }
            base.OnStart(state);
        }
    }
}
