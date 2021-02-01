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
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.Commons;
using VaultSharp.V1.SecretsEngines.TOTP;

namespace VaultOTP
{
    /// <summary>
    /// Interaction logic for AddOTPSourceWindow.xaml
    /// </summary>
    public partial class AddOTPSourceWindow : Window
    {
        public AddOTPSourceWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            CancelAddButton.IsEnabled = false;
            AddKeyButton.IsEnabled = false;
            AddKeyButton.Content = "Processing...";

            IAuthMethodInfo authMethod = new TokenAuthMethodInfo(Properties.Settings.Default.VaultToken);
            var vaultClientSettings = new VaultClientSettings(Properties.Settings.Default.VaultURL, authMethod);

            IVaultClient vaultClient = new VaultClient(vaultClientSettings);
            TOTPCreateKeyRequest createKeyRequest = new TOTPCreateKeyRequest
            {
                AccountName = ProductBox.Text,
                Algorithm = "SHA1",
                Period = "30",
                Issuer = VendorBox.Text,
                KeyGenerationOption = new TOTPNonVaultBasedKeyGeneration { 
                    AccountName = ProductBox.Text,
                    Issuer = VendorBox.Text,
                    Key = SecretBox.Text,  
                },
            };

            

            

            Secret<TOTPCreateKeyResponse> createResponse = await vaultClient.V1.Secrets.TOTP.CreateKeyAsync(Guid.NewGuid().ToString(), createKeyRequest);
            
            

            this.DialogResult = true;
            this.Close();
        }
    }
}
