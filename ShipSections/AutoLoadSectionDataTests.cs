using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        private int value;

        protected override void Merge(SectionDataWithInfo dataToMerge)
        {
            value = dataToMerge.value;
        }
    }
}
