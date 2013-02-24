using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using mgfile;
using System.Management;

namespace MoonGate
{
    /// <summary>
    /// FileViewerControl.xaml の相互作用ロジック
    /// </summary>
    public partial class FileViewerControl : Window
    {
        private const string DRIVE_TYPE = "DriveType";

        public FileViewerControl()
        {
            InitializeComponent();
            //folViewInit();
        }

        private void folViewInit()
        {
            //ManagementObject moDrive = new ManagementObject();

            // 論理ドライブ取得
            string[] sDrives = Environment.GetLogicalDrives();

            foreach (string sDrive in sDrives)
            {
                ManagementObject moDrive = new ManagementObject("Win32_LogicalDisk.DeviceID=\"" + sDrive.TrimEnd('\\') + "\"");
                //moDrive.Path = new ManagementPath("Win32_LogicalDisk='" + sDrive.TrimEnd('\\') + "'");
                PropertyData pdDrive = moDrive.Properties["DeviceID"];

                TreeViewItem tviDir = new TreeViewItem();
                tviDir.Header = pdDrive.Value;

                folderBrowser.Items.Add(tviDir);
            }
        }

        private void folderBrowser_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            string selectedDir = "";

            MessageBox.Show(selectedDir);

            GetLocalFiles(selectedDir);
        }

        private void GetLocalFiles(string selectedDir)
        {
            if (Directory.Exists(selectedDir))
            {
                string[] fileList = Directory.GetFiles(selectedDir);
                fileView.ItemsSource = fileList;
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            foreach (object objFile in fileView.Items)
            {
                ListEntity.listEntity.ObsFileList.Add(objFile.ToString());
            }
        }

        private void folderBrowser_Initialized(object sender, EventArgs e)
        {
            folViewInit();
        }
    }
}
