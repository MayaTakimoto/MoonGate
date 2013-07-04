using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MoonGate.Component.ViewModel
{
    /// <summary>
    /// ツリーノードリストクラス
    /// </summary>
    public class FolderBrowserInfoViewModel
    {
        /// <summary>
        /// 子ノードのリストのプロパティ
        /// </summary>
        public List<TreeNodeViewModel> Nodes { get; private set; }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FolderBrowserInfoViewModel()
        {
            DriveInfo[] listDrv = DriveInfo.GetDrives();
            Nodes = listDrv.Select(param => new TreeNodeViewModel(param.Name)).ToList<TreeNodeViewModel>();
        }
    }
}
