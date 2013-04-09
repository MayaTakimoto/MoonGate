using System.Windows;

namespace ConsumerSignup
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeyInput_PasswordChanged(object sender, RoutedEventArgs e)
        {
            this.Key.Tag = this.KeyInput.SecurePassword;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SecInput_PasswordChanged(object sender, RoutedEventArgs e)
        {
            this.Secret.Tag = this.SecInput.SecurePassword;
        }
    }
}
