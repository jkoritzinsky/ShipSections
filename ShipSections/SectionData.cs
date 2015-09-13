using KSPPluginFramework;

namespace JKorTech.ShipSections
{
    public abstract class SectionData : ConfigNodeStorage
    {
        [Persistent]
        public string sectionName = string.Empty;
    }
}