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
using System.Windows.Threading;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.Commons;
using VaultSharp.V1.SecretsEngines.TOTP;

namespace VaultOTP
{
    /// <summary>
    /// Interaction logic for OtpView.xaml
    /// </summary>
    public partial class OtpView : UserControl
    {
        public string KeyName { get; set; }
        private DispatcherTimer timer;
        public OtpView(string keyName)
        {
            this.KeyName = keyName;
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            var refreshTask = GetTokenData();
            timer.Interval = TimeSpan.FromSeconds(1);
            
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var refreshTask = GetTokenData();
        }

        async Task GetTokenData()
        {
            IAuthMethodInfo authMethod = new TokenAuthMethodInfo(Properties.Settings.Default.VaultToken);
            var vaultClientSettings = new VaultClientSettings(Properties.Settings.Default.VaultURL, authMethod);

            IVaultClient vaultClient = new VaultClient(vaultClientSettings);
            Secret<TOTPKey> key = await vaultClient.V1.Secrets.TOTP.ReadKeyAsync(this.KeyName);
            Secret<TOTPCode> totpSecret = await vaultClient.V1.Secrets.TOTP.GetCodeAsync(this.KeyName);


            ProviderName.Content = key.Data.Issuer;
            AccountName.Content = key.Data.AccountName;
            SecondsLeft.Content = "";
            OTPValue.Text = totpSecret.Data.Code;
            if (!timer.IsEnabled)
            {
                timer.Start();
            }


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(OTPValue.Text.ToString());
        }
    }
}
