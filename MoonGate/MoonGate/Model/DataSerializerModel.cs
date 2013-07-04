using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace MoonGate.Model
{
    /// <summary>
    /// シリアライズ・デシリアライズクラス
    /// </summary>
    public class DataSerializerModel
    {
        /// <summary>
        /// シリアライズメソッド
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool TrySerialize(string filePath, object data)
        {
            DataContractSerializer dSerializer = new DataContractSerializer(data.GetType());

            var serializeObj = data;

            using (FileStream fsWrite = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                using (XmlDictionaryWriter dicWriter = XmlDictionaryWriter.CreateBinaryWriter(fsWrite))
                {
                    try
                    {
                        dSerializer.WriteObject(dicWriter, data);
                    }
                    catch
                    {
                        return false;
                    }
                }
            }

            return true;
        }


        /// <summary>
        /// デシリアライズメソッド
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool TryDeserialize(string filePath, ref object data)
        {
            if (!File.Exists(filePath))
            {
                return false;
            }

            DataContractSerializer dSerializer = new DataContractSerializer(data.GetType());

            using (FileStream fsRead = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (XmlDictionaryReader dicReader = XmlDictionaryReader.CreateBinaryReader(fsRead, XmlDictionaryReaderQuotas.Max))
                {
                    try
                    {
                        data = dSerializer.ReadObject(dicReader);
                    }
                    catch
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
