using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace MoonGate.Component.Entity
{
    /// <summary>
    /// ツリーノードリストクラス
    /// </summary>
    class FolderBrowserInfoEntity
    {
        /// <summary>
        /// 子ノードのリストのプロパティ
        /// </summary>
        public List<TreeNodeEntity> Nodes { get; private set; }

        /// <summary>
        /// 選択されたノードのプロパティ
        /// </summary>
        public TreeNodeEntity SelectedNode { get; set; }

        /// <summary>
        /// 選択されたノードのリストのプロパティ
        /// </summary>
        public ObservableCollection<string> SelectedNodes { get; set; }

        /// <summary>
        /// 選択ノード追加コマンドプロパティ
        /// </summary>
        public ICommand AddNodesCommand { get; private set; }

        ///// <summary>
        ///// OKボタン押下時コマンドプロパティ
        ///// </summary>
        //public ICommand ResultOKCommand { get; private set; }

        ///// <summary>
        ///// キャンセルボタン押下時コマンドプロパティ
        ///// </summary>
        //public ICommand ResultCancelCommand { get; private set; }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FolderBrowserInfoEntity()
        {
            DriveInfo[] listDrv = DriveInfo.GetDrives();
            Nodes = listDrv.Select(param => new TreeNodeEntity(param.Name)).ToList<TreeNodeEntity>();

            SelectedNodes = new ObservableCollection<string>();

            SetCommand();
        }


        /// <summary>
        /// コマンドのセット
        /// </summary>
        private void SetCommand()
        {
            AddNodesCommand = new CommandSetter(
                param => this.Add(),
                param =>
                {
                    if (SelectedNode == null)
                    {
                        return false;
                    }

                    return true;
                }
            );

            //ResultOKCommand = new CommandSetter(
            //    param => this.Close(),
            //    param =>
            //    {
            //        if (SelectedNodes.Count == 0)
            //        {
            //            return false;
            //        }

            //        return true;
            //    }
            //);

            //ResultCancelCommand = new CommandSetter(
            //    param => this.Cansel()
            //);
        }

        
        /// <summary>
        /// 選択されたノードをリストに追加
        /// </summary>
        private void Add()
        {
            SelectedNodes.Add(SelectedNode.FolderInfo.FilePath);
            SelectedNode = null;
        }


        ///// <summary>
        ///// 
        ///// </summary>
        //private void Close()
        //{
        //    AddNodesCommand = null;
        //    ResultOKCommand = null;
        //    ResultCancelCommand = null;

            
        //}


        ///// <summary>
        ///// 
        ///// </summary>
        //private void Cansel()
        //{
        //    SelectedNodes.Clear();
        //    this.Close();
        //}

    }
}
