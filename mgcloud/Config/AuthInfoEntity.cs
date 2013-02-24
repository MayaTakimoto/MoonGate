//-----------------------------------------------------------------------
// <summary>OAuth認証情報の保持クラス</summary>
// <author>MayaTakimoto</author> 
// <date>$Date: 2013-02-13 14:00:00  GMT $</date>
// <copyright file="$Name: AuthInfoEntity.cs $" > 
//     Copyright(c) 2013 MayaTakimoto All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace mgcloud.Config
{
    /// <summary>
    /// OAuth認証情報のエンティティ
    /// </summary>
    class AuthInfoEntity
    {
        /// <summary>
        /// アクセストークン
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// リフレッシュトークン
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// アクセストークンの有効期限
        /// </summary>
        public DateTime TokenLimit { get; set; }
    }
}
