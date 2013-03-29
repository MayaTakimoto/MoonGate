using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoonGate.Component.Entity
{
    /// <summary>
    /// CloudSetupWindowの情報保持クラス
    /// </summary>
    class CloudSetupInfoEntity
    {
        /// <summary>
        /// 使用可能クラウドストレージ情報保持プロパティ
        /// </summary>
        public List<CloudInfoEntity> ListCloudInfo { get; private set; }
    }
}
