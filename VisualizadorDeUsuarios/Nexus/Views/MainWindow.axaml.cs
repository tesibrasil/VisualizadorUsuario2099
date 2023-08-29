using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.LogicalTree;
using System.Linq;
using MessageBox.Avalonia;

namespace TesiNexus.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SetMinimumLenghtForm();
           
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

        }

        //forma simples de simular o evento click de um botão (usar somente em testes)
        void InitializeButtons()
        {
            var b = (this.Find<Button>("btnTeste"));
            b.Click += delegate
            {
                var msg = MessageBoxManager.GetMessageBoxStandardWindow("Teste", "Working");
                msg.Show();
            };

        }

        private void SetMinimumLenghtForm()
        {
            //var window = this.GetSelfAndLogicalAncestors().OfType<Window>().First();
            //var screen = window.Screens.ScreenFromPoint(Position);
            //this.MinWidth = screen.WorkingArea.Width;
            //this.MinHeight = screen.WorkingArea.Height;     
        }


    }
}
