//-----------------------------------------------------------------------
// <summary>鍵情報入力ダイアログ</summary>
// <author>MayaTakimoto</author> 
// <date>$Date: 2013-02-13 14:00:00  +9:00 $</date>
// <copyright file="$Name: TargetListEntity.cs $" > 
//     Copyright(c) 2013 MayaTakimoto All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;

namespace MoonGate
{
    /// <summary>
    /// InputPassWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class InputPassWindow : Window
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InputPassWindow()
        {
            InitializeComponent();

            // ドライブ情報取得
            this.DriveList.ItemsSource = Directory.GetLogicalDrives().ToList<string>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (IsPassInfOK())
            {
                this.DialogResult = true;
            }
            else
            {
                this.DialogResult = false;
            }
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnKeyFile_Click(object sender, RoutedEventArgs e)
        {

        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool IsPassInfOK()
        {
            switch (this.TabMain.SelectedIndex)
            {
                case 0:
                    if (pswdInput.SecurePassword == null || pswdInput.SecurePassword.Length == 0)
                    {
                        MessageBox.Show(Properties.Resources.mesErrValisNull, Properties.Resources.titleErr, MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                    if (pswdConfirm.SecurePassword == null || pswdConfirm.SecurePassword.Length == 0)
                    {
                        MessageBox.Show(Properties.Resources.mesErrValisNull, Properties.Resources.titleErr, MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }

                    IntPtr ptrInput = Marshal.SecureStringToBSTR(pswdInput.SecurePassword);
                    IntPtr ptrConfirm = Marshal.SecureStringToBSTR(pswdConfirm.SecurePassword);
                    try
                    {
                        foreach (var iCnt in Enumerable.Range(0, pswdInput.SecurePassword.Length * 2))
                        {
                            var bytea = Marshal.ReadByte(ptrInput, iCnt);
                            var byteb = Marshal.ReadByte(ptrConfirm, iCnt);
                            if (bytea != byteb)
                            {
                                MessageBox.Show(Properties.Resources.mesErrInputMiss, Properties.Resources.titleErr, MessageBoxButton.OK, MessageBoxImage.Error);
                                return false;
                            }
                        }
                    }
                    catch
                    {
                        MessageBox.Show(Properties.Resources.mesExp, Properties.Resources.titleErr, MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                    finally
                    {
                        Marshal.ZeroFreeBSTR(ptrInput);
                        Marshal.ZeroFreeBSTR(ptrConfirm);
                    }
                    break;

                case 1:
                    if (ConKeyFile.Tag == null || string.IsNullOrEmpty(ConKeyFile.Tag.ToString()))
                    {
                        MessageBox.Show(Properties.Resources.mesErrValisNull, Properties.Resources.titleErr, MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                    break;

                case 2:
                    if (this.DriveList.SelectedItem == null)
                    {
                        return false;
                    }
                    break;

                default:
                    return false;
            }

            return true;
        }
    }
}
