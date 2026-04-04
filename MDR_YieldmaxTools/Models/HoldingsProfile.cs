using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Documents;
using System.Xml.Serialization;
using MDR_YieldmaxTools.Enums;

namespace MDR_YieldmaxTools.Models
{
    [Serializable]
    public class HoldingsProfile
    {
        public string Name = "local";

        public List<HoldingsTransactionData> Items { get; set; }

        public HoldingsProfile()
        {
        }

        public void AddItem(HoldingsTransactionData _newItem)
        {
            if (Items == null) Items = new List<HoldingsTransactionData>();
            Items.Add(_newItem);
            this.Save();
        }

        public void DeleteItem(HoldingsTransactionData _delItem)
        {
            int rmIndex = -1;
            for (int i = 0; i < Items.Count; i++)
            {
                HoldingsTransactionData htd = Items[i];
                if (htd.Symbol == _delItem.Symbol && htd.Timestamp == _delItem.Timestamp &&
                    htd.PricePerShare == _delItem.PricePerShare && htd.Volume == _delItem.Volume)
                {
                    rmIndex = i;
                    break;
                }
            }

            if (rmIndex >= 0)
            {
                Items.RemoveAt(rmIndex);
            }
            this.Save();
        }

        #region IO

        public void Save()
        {
            string fName = Name + ".xml";

            File.Delete(fName);

            using (var fs = File.Create(fName))
            {
                var xw = new XmlSerializer(typeof(HoldingsProfile), new Type[] { typeof(List<HoldingsTransactionData>) });
                xw.Serialize(fs, this);
            }
        }

        public static HoldingsProfile Load(string name)
        {
            var fName = name + ".xml";
            HoldingsProfile result = new HoldingsProfile();
            result.Name = name;

            if (!File.Exists(fName))
            {
                result.Save();
            }

            using (var fs = File.OpenRead(fName))
            {
                var xr = new XmlSerializer(typeof(HoldingsProfile), new Type[] { typeof(List<HoldingsTransactionData>) });
                result = (HoldingsProfile) xr.Deserialize(fs);
            }

            return result;
        }

        #endregion
    }
}