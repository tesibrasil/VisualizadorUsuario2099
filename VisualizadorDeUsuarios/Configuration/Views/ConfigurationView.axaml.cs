using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TesiNexus.Configuration.Views
{
    public partial class ConfigurationView : UserControl
    {
        public ConfigurationView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
