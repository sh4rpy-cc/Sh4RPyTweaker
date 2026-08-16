using System.Windows;
using System.Windows.Controls;
using Sh4RPyTweaker.Services;

namespace Sh4RPyTweaker.Views
{
    public partial class SystemInfoView : UserControl
    {
        public SystemInfoView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await Load();
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e)
        {
            await Load();
        }

        private async System.Threading.Tasks.Task Load()
        {
            RefreshButton.IsEnabled = false;
            Status.Text = "Сбор информации...";
            var groups = await System.Threading.Tasks.Task.Run(() => SystemInfoService.Gather());
            Groups.ItemsSource = groups;
            Status.Text = "Информация собрана. Обновлено: " + System.DateTime.Now.ToString("HH:mm:ss");
            RefreshButton.IsEnabled = true;
        }
    }
}

