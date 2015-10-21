using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace JKorTech.ShipSections
{
    public class SectionDataContainer
    {
        [Persistent(collectionIndex = "SECTIONDATA", isPersistant = true)]
        private List<SectionDataBase> data = new List<SectionDataBase>();
        
        internal void AddOrUpdateSectionDataForMod(SectionDataBase sectionData)
        {
            SectionDataBase currentData = data.FirstOrDefault(data => Equals(data.GetType(), sectionData.GetType()));
            if (currentData != null)
                currentData.Merge(sectionData);
            else
                data.Add(sectionData);
        }

        public T GetSectionData<T>()
            where T : SectionDataBase
        {
            return data.OfType<T>().First();
        }

        internal ReadOnlyCollection<SectionDataBase> GetAllSectionDatas() => new ReadOnlyCollection<SectionDataBase>(data);
    }
}
