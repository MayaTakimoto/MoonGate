using MoonGate.Component.Entity;
using MoonGate.Component.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MoonGate.Component.Action
{
    /// <summary>
    /// 
    /// </summary>
    class SelectFolderAction : BaseAction
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="receiver"></param>
        public override void RegistAction(FrameworkElement receiver)
        {
            Indicator.Instance.Instruct<SelectFolderMessage>(receiver, ShowSelectFolderDialog);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        private static void ShowSelectFolderDialog(SelectFolderMessage message)
        {
            FolderBrowseWindow DialogGetFolder = new FolderBrowseWindow();
            var resDlg = DialogGetFolder.ShowDialog();

            if ((bool)resDlg == false)
            {
                message.Result = false;

                DialogGetFolder = null;
                return;
            }
            if (DialogGetFolder.SelectedFolder.Items.Count == 0)
            {
                message.FolderNames = null;
                message.Result = false;

                DialogGetFolder = null;
                return;
            }
            
            message.FolderNames = DialogGetFolder.SelectedFolder.Items.Cast<string>().ToArray();
            message.Result = true;

            DialogGetFolder = null;
        }


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="listFolder"></param>
        ///// <returns></returns>
        //private static void GetSelectedFolderList(List<TreeNodeEntity> Nodes, ref List<string> listFolder)
        //{
        //    foreach (TreeNodeEntity node in Nodes)
        //    {
        //        if (node.IsChecked)
        //        {
        //            listFolder.Add(node.FolderInfo.FilePath);
        //        }

        //        if (node.ListTreeNodes != null)
        //        {
        //            GetSelectedFolderList(node.ListTreeNodes.ToList<TreeNodeEntity>(), ref listFolder);
        //        }
        //    }
        //}
    }
}
