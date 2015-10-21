using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JKorTech.ShipSections
{
    [KSPModule("Section Information")]
    public class SectionInfo : PartModule
    {
        private static readonly string DefaultSection = "New Section 1";
        [KSPField(guiActive = false, guiActiveEditor = true, guiName = "Section", isPersistant = true)]
        public string section = DefaultSection;

        [KSPField(guiActive = false, guiActiveEditor = false, isPersistant = true)]
        public bool isSectionRoot = false;

        internal SectionDataContainer dataContainer = new SectionDataContainer();

        public override void OnStart(StartState state)
        {
            if(state != StartState.None && section == DefaultSection)
            {
                section = part.parent?.FindModuleImplementing<SectionInfo>()?.section ?? DefaultSection;
            }
            base.OnStart(state);
        }

        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);
            var dataContainerNode = node.GetNode("CONTAINER");
            if (dataContainerNode != null)
                ConfigNode.LoadObjectFromConfig(dataContainer, dataContainerNode);
        }

        public override void OnSave(ConfigNode node)
        {
            base.OnSave(node);
            var dataContainerNode = node.AddNode("CONTAINER");
            ConfigNode.CreateConfigFromObject(dataContainer);
        }
    }
}
