using MoonGate.Component.Entity;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MoonGate
{
    /// <summary>
    /// FileBrowseWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class FolderBrowseWindow : Window
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FolderBrowseWindow()
        {
            InitializeComponent();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox selected = e.Source as CheckBox;
            SelectedFolder.Items.Add(selected.Tag);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox released = e.Source as CheckBox;
            foreach (var item in SelectedFolder.Items)
            {
                if (item == released.Tag)
                {
                    SelectedFolder.Items.Remove(item);
                    break;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FolderTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            
            //CheckBox checkBox = e.Source as CheckBox;
        }
    }
}
