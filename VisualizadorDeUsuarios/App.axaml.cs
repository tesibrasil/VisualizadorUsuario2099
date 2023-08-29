using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System;
using System.Data.SqlClient;
using System.IO;
using TesiNexus.Helpers;
using TesiNexus.Nexus.Models;
using TesiNexus.Nexus.ViewModels;
using TesiNexus.Nexus.Views;
using TesiNexus.ViewModels;
using TesiNexus.Views;

namespace TesiNexus
{
    public partial class App : Application
    {
        public NexusApp RunningNexus = new NexusApp();

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {

                desktop.MainWindow = new StartUpView
                {
                    DataContext = new StartUpViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();

        }
    }
}
