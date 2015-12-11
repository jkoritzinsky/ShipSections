namespace JKorTech.ShipSections
{
    public abstract class SectionData<T> : SectionDataBase
        where T: SectionData<T>, new()
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
