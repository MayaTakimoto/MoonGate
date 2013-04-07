using System.Windows;

namespace ConsumerSignup
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void KeyInput_PasswordChanged(object sender, RoutedEventArgs e)
        {
            this.Key.Tag = this.KeyInput.SecurePassword;
        }

        private void SecInput_PasswordChanged(object sender, RoutedEventArgs e)
        {
            this.Secret.Tag = this.SecInput.SecurePassword;
        }
    }
}
