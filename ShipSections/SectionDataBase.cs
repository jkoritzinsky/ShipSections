namespace JKorTech.ShipSections
{
    public abstract class SectionDataBase : IPersistenceLoad, IPersistenceSave
    {
        public virtual void PersistenceLoad()
        {
        }

        public virtual void PersistenceSave()
        {
        }

        internal abstract bool Merge(SectionDataBase data);
    }
}