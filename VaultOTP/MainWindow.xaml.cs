using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reflection;
using System.Diagnostics;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.AuthMethods;
using VaultSharp;
using VaultSharp.V1.Commons;

namespace VaultOTP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.VaultToken == "")
            {
                var loginWindow = new LoginWindow();
                if (loginWindow.ShowDialog() == false)
                {
                    MessageBox.Show("Vault TOTP client must be configured before use.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                }
            }
            VaultURL.Content = Properties.Settings.Default.VaultURL;
            var updateContentTask = UpdateContent();
        }

        async Task UpdateContent()
        {
            IAuthMethodInfo authMethod = new TokenAuthMethodInfo(Properties.Settings.Default.VaultToken);
            var vaultClientSettings = new VaultClientSettings(Properties.Settings.Default.VaultURL, authMethod);

            IVaultClient vaultClient = new VaultClient(vaultClientSettings);
               
            Secret<ListInfo> keys = await vaultClient.V1.Secrets.TOTP.ReadAllKeysAsync();

            foreach (var key in  keys.Data.Keys)
            {
                var otpView = new OtpView(key);
                OtpViewPanel.Children.Add(otpView);
            }

        }


        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Preferences Stub");
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Add secret stub");
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fileVersionInfo.ProductVersion;
            MessageBox.Show(string.Format( "Hashicorp Vault TOTP Client\n\nWritten by Yvan Janssens\nVersion {0}", version ));
        }
    }
}
