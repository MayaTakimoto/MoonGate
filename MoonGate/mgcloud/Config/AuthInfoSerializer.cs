//-----------------------------------------------------------------------
// <summary>OAuth認証情報の読み書きを行うクラス</summary>
// <author>MayaTakimoto</author> 
// <date>$Date: 2013-02-13 14:00:00  +9:00 $</date>
// <copyright file="$Name: AuthInfoSerializer.cs $" > 
//     Copyright(c) 2013 MayaTakimoto All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace mgcloud.Config
{
    /// <summary>
    /// 認証情報の読込・書込を行う
    /// </summary>
    class AuthInfoSerializer
    {
        /// <summary>
        /// 認証情報を書き込むファイルのパス
        /// </summary>
        private string authInfPath;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="authInfoPath"></param>
        public AuthInfoSerializer(string authInfoPath)
        {
            this.authInfPath = authInfoPath;
        }


        /// <summary>
        /// 認証情報をシリアライズして保存する
        /// </summary>
        /// <param name="entAuthInfo">OAuth認証情報エンティティ</param>
        /// <returns></returns>
        public bool TrySerialize(AuthInfoEntity entAuthInfo)
        {
            DataContractSerializer aiSerializer = new DataContractSerializer(typeof(AuthInfoEntity));

            using (FileStream fsWrite = new FileStream(authInfPath, FileMode.Create, FileAccess.Write))
            {
                using (XmlDictionaryWriter xDicWrite = XmlDictionaryWriter.CreateBinaryWriter(fsWrite))
                {
                    try
                    {
                        aiSerializer.WriteObject(xDicWrite, entAuthInfo);
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
        /// 認証情報をデシリアライズして渡す
        /// </summary>
        /// <returns></returns>
        public bool TryDeserialize(out AuthInfoEntity entAuthInfo)
        {
            DataContractSerializer aiDeserializer = new DataContractSerializer(typeof(AuthInfoEntity));

            using (FileStream fsRead = new FileStream(authInfPath, FileMode.Open, FileAccess.Read))
            {
                using (XmlDictionaryReader xDicRead = XmlDictionaryReader.CreateBinaryReader(fsRead, XmlDictionaryReaderQuotas.Max))
                {
                    try
                    {
                        entAuthInfo = (AuthInfoEntity)aiDeserializer.ReadObject(xDicRead);
                    }
                    catch
                    {
                        // デシリアライズ失敗時は初期化したエンティティを渡す
                        entAuthInfo = new AuthInfoEntity();
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
