using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace JKorTech.ShipSections
{
    public class SectionDataContainer
    {
        private List<SectionDataBase> data = new List<SectionDataBase>();

        internal void AddOrUpdateSectionDataForMod(SectionDataBase sectionData)
        {
            if (sectionData == null) throw new ArgumentNullException(nameof(sectionData));
            var currentData = data.FirstOrDefault(data => Equals(data.GetType(), sectionData.GetType()));
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

        internal ConfigNode OnSave()
        {
            return ListToConfigNode(data);
        }

        internal void OnLoad(ConfigNode node)
        {
            data = ConfigNodeToList<SectionDataBase>(node);
        }


        // The following two methods I took from my work on BROKE, another mod for KSP.

        private static ConfigNode ListToConfigNode<T>(List<T> list)
        {
            var retNode = new ConfigNode();
            foreach (var item in list)
                retNode.AddNode(ConfigNode.CreateConfigFromObject(item));
            return retNode;
        }

        private static List<T> ConfigNodeToList<T>(ConfigNode node)
        {
            var retList = new List<T>();
            foreach (var element in node.GetNodes())
            {
                var dataType = SectionDataDictionary.SectionDataTypes.Keys.FirstOrDefault(key => key.FullName == element.name);
                if (dataType == null)
                {
                    UnityEngine.Debug.LogWarning($"Could not find class {element.name}.  Not re-loading from persistance.");
                    continue;
                }
                var obj = Activator.CreateInstance(dataType);
                ConfigNode.LoadObjectFromConfig(obj, element);
                retList.Add((T)obj);
            }
            return retList;
        }
    }
}
