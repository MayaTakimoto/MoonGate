//-----------------------------------------------------------------------
// <summary>OAuth認証情報の保持クラス</summary>
// <author>MayaTakimoto</author> 
// <date>$Date: 2013-02-13 14:00:00  +9:00 $</date>
// <copyright file="$Name: AuthInfoEntity.cs $" > 
//     Copyright(c) 2013 MayaTakimoto All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace mgcloud.Config
{
    /// <summary>
    /// OAuth認証情報のエンティティ
    /// </summary>
    [DataContract]
    class AuthInfoEntity
    {
        /// <summary>
        /// アクセストークン
        /// </summary>
        [DataMember]
        public char[] AccessToken { get; set; }

        /// <summary>
        /// リフレッシュトークン
        /// </summary>
        [DataMember]
        public char[] RefreshToken { get; set; }

        /// <summary>
        /// アクセストークンの有効期限
        /// </summary>
        [DataMember]
        public DateTime TokenLimit { get; set; }
    }
}
