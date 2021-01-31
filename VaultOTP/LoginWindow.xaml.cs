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
using System.Windows.Shapes;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.UserPass;
using VaultSharp;
using VaultSharp.V1.SecretsEngines.TOTP;
using VaultSharp.V1.Commons;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.Core;
using System.Net.Http;

namespace VaultOTP
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private async void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            ConfirmButton.IsEnabled = false;
            CancelButton.IsEnabled = false;
            ConfirmButton.Content = "Confirming...";
            try
            {
                var authMethod = new UserPassAuthMethodInfo(Username.Text, Password.Password);
                var vaultClientSettings = new VaultClientSettings(VaultServerURL.Text, authMethod);
                var vaultClient = new VaultClient(vaultClientSettings);
                var createTokenRequest = new CreateTokenRequest();
                createTokenRequest.DisplayName = "TOTP Client token";
                try
                {
                    Secret<object> permanentToken = await vaultClient.V1.Auth.Token.CreateTokenAsync(createTokenRequest);
                    Properties.Settings.Default.VaultToken = permanentToken.AuthInfo.ClientToken;
                    Properties.Settings.Default.VaultURL = VaultServerURL.Text;
                    Properties.Settings.Default.TotpMountPoint = TOTPMountPoint.Text;
                    Properties.Settings.Default.Save();
                    this.DialogResult = true;
                    this.Close();
                }
                catch (VaultApiException exception)
                {
                    MessageBox.Show(exception.ApiErrors.FirstOrDefault(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                } 
                catch (HttpRequestException)
                {
                    MessageBox.Show("Could not connect to Vault instance", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }catch (ArgumentException exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            ConfirmButton.Content = "Confirm";
            ConfirmButton.IsEnabled = true;
            CancelButton.IsEnabled = true;
            
        }
    }
}
