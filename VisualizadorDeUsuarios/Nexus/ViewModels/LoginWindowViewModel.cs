using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Dapper;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TesiNexus.Helpers;
using TesiNexus.Nexus.Models;
using TesiNexus.Nexus.Views;
using TesiNexus.ViewModels;
using TesiNexus.Views;

namespace TesiNexus.Nexus.ViewModels
{
    public class LoginWindowViewModel : ViewModelBase
    {
        public LoginWindowViewModel()
        {
            EntrarCommand = ReactiveCommand.CreateFromTask(async () => { Entrar(); });
            SairCommand = ReactiveCommand.CreateFromTask(async () => { Sair(); });
            Login = "admin";
            Password = "nautilus";

        }

        private string _user;
        public string Login
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

        public ICommand EntrarCommand { get; set; }
        public ICommand SairCommand { get; set; }
        public IDbTransaction CurrentTransaction { get; private set; }

        public async Task Entrar()
        {
            if (VerificarUsuario())
            {
                var newlifetime = (IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime;


                MainWindow mainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };

                
                mainWindow.Show();
                newlifetime.MainWindow.Close();

                if ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    desktop.MainWindow = mainWindow;
                }


            }
            else
            {
                Login = "";
                Password = "";
            }
        }

        public void Sair()
        {
            var lifetime = (IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime;

            lifetime.Shutdown();
        }

        public bool VerificarUsuario()
        {
            //bool user = false;
            string conectionString = $@"Data Source={((App)App.Current).RunningNexus.IpAdress};User ID={((App)App.Current).RunningNexus.UserLogin};Password={((App)App.Current).RunningNexus.Password};Initial Catalog=VVAND4;";
            User loginUser = new User();
            //string s = Crypto.Encrypt(Password);

            using (SqlConnection conn = new SqlConnection(conectionString))
            {
                
                conn.Open();

                try
                {
                    
                   var  result = conn.Query<User>(@"LoginNexusUser", param: new
                    {
                        login = Login
                    }
                     ,  transaction: CurrentTransaction
                     ,  commandType: CommandType.StoredProcedure).FirstOrDefault();

                    loginUser = result;
                }
                catch
                {
                    return false;
                }

            }

            if (Crypto.Decrypt(loginUser.Password).Equals(Password)) 
            { 
                return true; 
            }
            else
            {
                return false;
            }
            

        }
    }
}

