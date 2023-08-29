using ReactiveUI;
using System;
using System.Reactive;
using System.Windows.Input;
using System.Windows;
using TesiNexus.Views;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia;
using Avalonia.Controls;
using System.Threading.Tasks;
using Avalonia.Input;
using MessageBox.Avalonia;
using TesiNexus.Synchronizer.ViewModels;
using TesiNexus.Configuration.ViewModels;

namespace TesiNexus.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {

        #region Properties

        #region HamburguerMenu
        private bool _showMenu;

        public bool ShowMenu
        {
            get { return _showMenu; }
            set
            {
                this.RaiseAndSetIfChanged(ref _showMenu, value);
                ActionMenu();

            }
        }

        private int _widthButton;

        public int WidthButton
        {
            get { return _widthButton; }
            set
            {
                this.RaiseAndSetIfChanged(ref _widthButton, value);

            }
        }

        private int _widthBorder;

        public int WidthBorder
        {
            get { return _widthBorder; }
            set
            {
                this.RaiseAndSetIfChanged(ref _widthBorder, value);

            }
        }

        private Thickness _margin;

        public Thickness Margin
        {
            get { return _margin; }
            set
            {
                this.RaiseAndSetIfChanged(ref _margin, value);
            }
        }


        public ReactiveCommand<object, Unit> MouseOverCommand { get; set; }
        public ReactiveCommand<object, Unit> MouseLeaveCommand { get; set; }
        #endregion

        #region CurrentView  
        public SynchronizerViewModel SynchronizerVM { get; set; }
        public TopMenuSynchronizerViewModel TopMenuSynchronizerVM { get; set; }
        public ConfigurationViewModel ConfigurationVM { get; set; }
        public TopMenuConfigurationViewModel TopMenuConfigurationVM { get; set; }

        private object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set { this.RaiseAndSetIfChanged(ref _currentView, value); }
        }

        private object _TopMenuView;

        public object TopMenuView
        {
            get { return _TopMenuView; }
            set { this.RaiseAndSetIfChanged(ref _TopMenuView, value); }
        }

        #region command
        public ICommand ShowSynchronizerCommand { get; set; }

        public ICommand ShowConfigurationCommand { get; set; }
        #endregion

        #endregion

        #endregion


        public MainWindowViewModel()
        {
            #region Hamburger Menu Initialize


            CurrentView = null;
            ShowMenu = false;
            WidthButton = 200;
            WidthBorder = 220;
            Margin = new Thickness(255, 10, 10, 10);
            MouseOverCommand = ReactiveCommand.CreateFromTask<object, Unit>(MouseOverCommandExecuted);
            MouseLeaveCommand = ReactiveCommand.CreateFromTask<object, Unit>(MouseLeaveCommandExecuted);

            #endregion

            #region CurrentView

            ShowSynchronizerCommand = ReactiveCommand.Create(ShowSynchronizerVM);
            ShowConfigurationCommand = ReactiveCommand.Create(ShowConfigurationVM);
            #endregion
        }


        #region Methods

        #region HamburgerMenu
        public void ActionMenu()
        {
            if (ShowMenu)
            {
                WidthButton = 50;
                WidthBorder = 50;
                Margin = new Thickness(85, 10, 10, 10);
            }
            else
            {
                WidthButton = 200;
                WidthBorder = 220;
                Margin = new Thickness(255, 10, 10, 10);
            }
        }

        private async Task<Unit> MouseOverCommandExecuted(object p)
        {
            if (ShowMenu)
            {
                WidthButton = 200;
                WidthBorder = 210;
            }
            return Unit.Default;
        }

        private async Task<Unit> MouseLeaveCommandExecuted(object p)
        {
            if (ShowMenu)
            {
                WidthButton = 50;
                WidthBorder = 50;
            }
            return Unit.Default;
        }

        #endregion


        #region CurrentView
        
        public void ShowSynchronizerVM()
        {
            SynchronizerVM = new SynchronizerViewModel();
            CurrentView = SynchronizerVM;
            TopMenuSynchronizerVM = new TopMenuSynchronizerViewModel();
            TopMenuView = TopMenuSynchronizerVM;
        }

        public void ShowConfigurationVM()
        {
            ConfigurationVM = new ConfigurationViewModel();
            CurrentView = ConfigurationVM; ;
            TopMenuConfigurationVM = new TopMenuConfigurationViewModel();
            TopMenuView = TopMenuConfigurationVM;
        }


        #endregion

        #endregion

    }


}


