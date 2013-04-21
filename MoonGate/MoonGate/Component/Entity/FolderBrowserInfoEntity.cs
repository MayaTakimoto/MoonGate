using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MoonGate.Component.Entity
{
    /// <summary>
    /// ツリーノードリストクラス
    /// </summary>
    public class FolderBrowserInfoEntity
    {
        /// <summary>
        /// 子ノードのリストのプロパティ
        /// </summary>
        public List<TreeNodeEntity> Nodes { get; private set; }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FolderBrowserInfoEntity()
        {
            DriveInfo[] listDrv = DriveInfo.GetDrives();
            Nodes = listDrv.Select(param => new TreeNodeEntity(param.Name)).ToList<TreeNodeEntity>();
        }
    }
}
