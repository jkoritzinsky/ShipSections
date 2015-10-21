using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JKorTech.ShipSections
{
    public abstract class SectionData<T> : SectionDataBase
        where T: SectionDataBase
    {
        protected abstract void Merge(T dataToMerge);

        internal override bool Merge(SectionDataBase data)
        {
            var dataAsT = data as T;
            if (dataAsT != null)
            {
                Merge(dataAsT);
                return true;
            }
            return false;
        }
    }
}
