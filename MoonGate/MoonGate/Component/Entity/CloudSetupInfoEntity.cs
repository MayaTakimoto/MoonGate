﻿using MoonGate.utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoonGate.Component.Entity
{
    /// <summary>
    /// CloudSetupWindow情報保持クラス
    /// </summary>
    class CloudSetupInfoEntity
    {
        /// <summary>
        /// マスタファイルパス
        /// </summary>
        private const string CSLIST_PATH = "./mst/CSList.xml";

        /// <summary>
        /// 使用可能クラウドストレージリストプロパティ
        /// </summary>
        public List<CloudInfoEntity> ListCloudInfo { get; private set; }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CloudSetupInfoEntity()
        {
            object list = new List<CloudInfoEntity>();
            if (DataSerializer.TryDeserialize(CSLIST_PATH, ref list))
            {
                ListCloudInfo = list as List<CloudInfoEntity>;
            }
        }
    }
}
