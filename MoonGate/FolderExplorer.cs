using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MoonGate
{
    /// <summary>
    /// 自作ファイル選択ダイアログのエクスプローラバー風パーツ(親部品)
    /// </summary>
    public class FolderExplorer
    {
        public IEnumerable<FolderExplorerNode> FolderTree
        {
            get
            {
                return DriveInfo.GetDrives().Select(driveInfo => new FolderExplorerNode(driveInfo.Name));
            }
        }
    }
}
