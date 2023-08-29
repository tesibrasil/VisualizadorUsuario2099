using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Dapper;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TesiNexus.Helpers;
using TesiNexus.ViewModels;
using System.Threading;
using Avalonia.Media;

namespace TesiNexus.Nexus.ViewModels
{
    public class DataBaseConfigViewModel : ViewModelBase
    {
        public DataBaseConfigViewModel()
        {
            Testar = ReactiveCommand.CreateFromTask(async () => {ConnectionTest(); });
            Salvar = ReactiveCommand.CreateFromTask(async () => { SaveConnection(); });
            Ambiente = ReactiveCommand.CreateFromTask(async () => { PrepareEnvironmentAsync(); });
            Fechar = ReactiveCommand.CreateFromTask(async () => { CloseApp(); });
            EnabledScreen = true;
         
            SolidColorBrush myBrush = new SolidColorBrush(Colors.Red);
            ColorText = myBrush;
            TesteOk = false;
            HabAmbiente = false;
        }

        #region Properties

        private string _ip;

        public string IP
        {
            get { return _ip; }
            set { this.RaiseAndSetIfChanged(ref _ip, value); }
        }

        private string _user;

        public string User
        {
            get { return _user; }
            set { this.RaiseAndSetIfChanged(ref _user, value); }
        }

        private string _password;

        public string Password
        {
            get { return _password; }
            set { this.RaiseAndSetIfChanged(ref _password, value); }
        }

        private string _mensagem;

        public string Mensagem
        {
            get { return _mensagem; }
            set { this.RaiseAndSetIfChanged(ref _mensagem, value); }
        }

        private bool _habAmbiente;

        public bool HabAmbiente
        {
            get { return _habAmbiente; }
            set { this.RaiseAndSetIfChanged(ref _habAmbiente, value); }
        }

        private bool _enabledProgress;

        public bool EnabledProgress
        {
            get { return _enabledProgress; }
            set { this.RaiseAndSetIfChanged(ref _enabledProgress, value); }
        }

        private bool _enabledScreen;

        public bool EnabledScreen
        {
            get { return _enabledScreen; }
            set { this.RaiseAndSetIfChanged(ref _enabledScreen, value); }
        }

        private ISolidColorBrush _color;

        public ISolidColorBrush ColorText
        {
            get { return _color; }
            set { this.RaiseAndSetIfChanged(ref _color, value); }
        }

        public bool TesteOk { get; set; }
        #endregion

        #region Commands

        public ICommand Testar { get; set; }
        public ICommand Ambiente { get; set; }
        public ICommand Salvar { get; set; }
        public ICommand Fechar { get; set; }
        public IDbTransaction CurrentTransaction { get; private set; }

        #endregion

        #region Methods
        public async Task ConnectionTest()
        {
            var th = new Thread(() =>
            {
                EnabledScreen = false;
                EnabledProgress = true;
                ColorText = Brushes.Yellow;

                Mensagem = "TESTE DE CONEXÃO!";

                string connStrFonte = $@"Data Source={IP};User ID={User};Password={Password};Initial Catalog=VVAND4;";

                if (!DataBaseHelper.CheckingConnection(connStrFonte))
                {
                    connStrFonte = $@"Data Source={IP};User ID={User};Password={Password};";

                    if (!DataBaseHelper.CheckingConnection(connStrFonte))
                    {
                        ColorText = Brushes.Red;
                        Mensagem = "FALHA AO TENTAR CONECTAR O BANCO DE DADOS!";
                    }
                    else
                    {
                        ColorText = Brushes.Yellow;
                        Mensagem = "SEU AMBIENTE NÃO ESTÁ PRERADO PARA RODAR O TESI NEXUS, CLIQUE EM PREPARAR AMBIENTE E TESTE NOVAMENTE A CONEXÃO.";
                        HabAmbiente = true;
                    }

                    TesteOk = false;
                }
                else
                {
                    ColorText = Brushes.LightGreen;
                    Mensagem = "SUCESSO!";
                    TesteOk = true;
                }


                EnabledProgress = false;
                EnabledScreen = true;
            });

            th.Start();


        }

        public void SaveConnection()
        {
            if (!TesteOk)
            {
                Mensagem = "SEU ULTIMO TESTE DE CONEXÃO NÃO FOI BEM SUCEDIDO. ARQUIVO DE CONFIGURAÇÃO NÃO FOI CRIADO!";
                return;
            }

            ((App)App.Current).RunningNexus.IpAdress = IP;
            ((App)App.Current).RunningNexus.UserLogin = User;
            ((App)App.Current).RunningNexus.Password = Password;

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(((App)App.Current).RunningNexus);

            json = Crypto.Encrypt(json);

            string programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string folder = programData + "\\NexusConfig";
            string arquivo = Path.Combine(folder, "conexao.json");

            if (File.Exists(arquivo))
            {
                File.Delete(arquivo);
                File.Create(arquivo).Dispose();
            }
            else
            {
                Directory.CreateDirectory(folder);
                File.Create(arquivo).Dispose();
            }
            
            File.WriteAllText(arquivo, json);

            Mensagem = "SALVOU!";

            CloseApp();

        }

        public async Task PrepareEnvironmentAsync()
        {
            await GoogleAPI.DownloadDataBaseAsync();

            var connStrFonte = $@"Data Source={IP};User ID={User};Password={Password};";

            using (SqlConnection conn = new SqlConnection(connStrFonte))
            {
                conn.Open();

                try
                {
                    conn.Query<string>($@"DECLARE @path as varchar(500) = (SELECT substring(physical_name,0,(LEN(physical_name)-9))FROM sys.master_files WHERE name = 'master')
                                          DECLARE @mdfPath as varchar(500) = @path + 'VVAND4.mdf'
                                          DECLARE @ldfPath as varchar(500) = @path + 'VVAND4_log.ldf'

                                          RESTORE DATABASE [VVAND4] FROM  DISK = N'C:\ProgramData\NexusConfig\Temp\VVAND4.bak' WITH  FILE = 1,  
                                          MOVE N'VVAND4' TO @mdfPath,  
                                          MOVE N'VVAND4_log' TO @ldfPath,  
                                          NOUNLOAD,  REPLACE,  STATS = 5"
                    , transaction: CurrentTransaction
                    , commandType: CommandType.Text);

                    //conn.Execute($"CREATE DATABASE VVAND4"
                    //                      , transaction: CurrentTransaction
                    //                      , commandType: CommandType.Text);

                    //conn.Query<string>($@"CREATE TABLE VVAND4.[dbo].USERS (ID INT IDENTITY (1,1), 
                    //                      					   Name  VARCHAR(500), 
                    //                      					   Login  VARCHAR(50), 
                    //                      					   Password  VARCHAR (1024), 
                    //                      					   Inactive  bit DEFAULT(0)
                    //                      					   )                                          
                    //                      CREATE TABLE VVAND4.[dbo].DESTINATIONS ( ID INT IDENTITY (1,1), 
                    //                      						Name  VARCHAR(500), 
                    //                      						Source  VARCHAR(500),
                    //                      						UserName VARCHAR(500), 
                    //                      						Password  VARCHAR (1024)
                    //                      						)"
                    //, transaction: CurrentTransaction
                    //, commandType: CommandType.Text);

                    Mensagem = "SUCESSO! AMBIENTE CONFIGURADO, TESTE E SALVE A CONEXAO";
                }
                catch (Exception ex)
                {
                    Mensagem = ex.Message;
                }
            }

            string programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string tempFolder = programData + "\\NexusConfig\\Temp";

            if (Directory.Exists(tempFolder))
            {
               Directory.Delete(tempFolder, true);
            }
        }

        public void CloseApp()
        {
            var lifetime = (IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime;

            lifetime.Shutdown();
        } 
        #endregion
    }
}
