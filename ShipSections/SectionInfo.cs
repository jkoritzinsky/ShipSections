﻿using System;
using System.Linq;

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

        [System.Diagnostics.Conditional("DEBUG")]
        private void EnableDebugInDebugMode()
        {
            Fields[nameof(isSectionRoot)].guiActiveEditor = true;
        }

        public override void OnStart(StartState state)
        {
            EnableDebugInDebugMode();
            if(state != StartState.None)
            {
                TrySetSectionBasedOnParent();
            }
            base.OnStart(state);
            try
            {
                API.PartSectionInitialized.Fire(part);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"[ShipSections] {nameof(API.PartSectionInitialized)} threw an exception. If this is not during opening a ship never opened with ShipSections, report this error.");
                UnityEngine.Debug.LogException(ex);
            }
        }

        internal void TrySetSectionBasedOnParent()
        {
            if (section == DefaultSection)
            {
                section = part.parent?.FindModuleImplementing<SectionInfo>()?.section ?? DefaultSection;
                UnityEngine.Debug.Log("Set section name to: " + section); 
            }
        }

        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);
            var dataContainerNode = node.GetNode("CONTAINER");
            if (dataContainerNode != null)
            {
                AddDefaultSectionData(); // This adds support for adding more recently installed mod data into a section
                dataContainer.OnLoad(dataContainerNode);
            }
        }

        public override void OnSave(ConfigNode node)
        {
            base.OnSave(node);
            node.AddNode("CONTAINER", dataContainer.OnSave());
        }

        internal void InitializeAsNewSection()
        {
            AddDefaultSectionData();
        }

        private static SectionDataBase CreateSectionDataDefault(Type type, ConfigNode defaultDataNode)
        {
            var data = (SectionDataBase)Activator.CreateInstance(type);
            ConfigNode.LoadObjectFromConfig(data, defaultDataNode);
            return data;
        }

        private void AddDefaultSectionData()
        {
            foreach (SectionDataBase data in SectionDataDictionary.SectionDataTypes
                            .Select(type => CreateSectionDataDefault(type.Key, type.Value)))
            {
                dataContainer.AddOrUpdateSectionDataForMod(data);
            }

        }
    }
}
