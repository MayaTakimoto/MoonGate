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
            DialogGetFolder.ShowDialog();

            if (DialogGetFolder.ListSelected.Items.Count == 0)
            {
                message.FolderNames = null;
                message.Result = false;
            }
            else
            {
                message.FolderNames = DialogGetFolder.ListSelected.Items.Cast<string>().ToArray();
                message.Result = true;
            }
        }
    }
}
