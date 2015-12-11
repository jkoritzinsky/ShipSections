namespace JKorTech.ShipSections
{
    public class EmptySectionData : SectionData<EmptySectionData>
    {
        protected override void Merge(EmptySectionData dataToMerge)
        {
            
        }
    }

    public class SectionDataWithInfo : SectionData<SectionDataWithInfo>
    {
        [Persistent]
        public int value;

        protected override void Merge(SectionDataWithInfo dataToMerge)
        {
            value = dataToMerge.value;
        }
    }

    public class SectionDataWithInfoNoTemplate : SectionData<SectionDataWithInfoNoTemplate>
    {
        [Persistent]
        public int value;

        protected override void Merge(SectionDataWithInfoNoTemplate dataToMerge)
        {
            value = dataToMerge.value;
        }
    }
}
