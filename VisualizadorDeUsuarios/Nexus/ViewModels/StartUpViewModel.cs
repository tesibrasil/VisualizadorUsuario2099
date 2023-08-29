using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesiNexus.Helpers;
using TesiNexus.Nexus.Models;
using TesiNexus.Nexus.Views;
using TesiNexus.ViewModels;
using TesiNexus.Views;

namespace TesiNexus.Nexus.ViewModels
{
    public class StartUpViewModel : ViewModelBase
    {
        public bool IsConfigured { get; set; }

        public StartUpViewModel()
        {            
            ShowSplashAsync();
        }

        public async Task ShowSplashAsync()
        {
            await Task.Delay(3000);
            await  CheckConnectionFile();

            var lifetime = (IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime;

            if (IsConfigured)
            {
                LoginWindowView login = new LoginWindowView
                {
                    DataContext = new LoginWindowViewModel(),
                };
                
                login.Show();
                lifetime.MainWindow.Close();

                if ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    desktop.MainWindow = login; ;
                }
            }
            else
            {
                DataBaseConfigView configView = new DataBaseConfigView
                {
                    DataContext = new DataBaseConfigViewModel(),
                };

                configView.Show();

                lifetime.MainWindow.Close();

                if ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    desktop.MainWindow = configView;
                }
            }
        }

        public async Task CheckConnectionFile()
        {
            string programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            string curFile = programData + "\\NexusConfig\\conexao.json";
            if (File.Exists(curFile))
            {
                string json = File.ReadAllText(curFile);
                json = Crypto.Decrypt(json);

                ((App)App.Current).RunningNexus = Newtonsoft.Json.JsonConvert.DeserializeObject<NexusApp>(json);

                string connStrFonte = $@"Data Source={((App)App.Current).RunningNexus.IpAdress};User ID={((App)App.Current).RunningNexus.UserLogin};Password={((App)App.Current).RunningNexus.Password};Initial Catalog=VVAND4;";
                if (DataBaseHelper.CheckingConnection(connStrFonte))
                {
                    IsConfigured = true;
                    await CheckNexusVersion(connStrFonte);
                }
                else
                {
                    IsConfigured = false;
                }
            }
            else
            {
                IsConfigured = false;
            }
        }

        public async Task CheckNexusVersion(string connectionString)
        {
            string currentVersion = DataBaseHelper.CheckingVersion(connectionString);

            string latestVersion = await GoogleAPI.CheckLatestVersionAsync(currentVersion);

            if (!latestVersion.Equals(""))
            {
                DataBaseHelper.UpdatingVersion(connectionString, latestVersion);

                string programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                string tempFolder = programData + "\\NexusConfig\\Temp";

                if (Directory.Exists(tempFolder))
                {
                    Directory.Delete(tempFolder, true);
                }
            }
        }
    }
}
