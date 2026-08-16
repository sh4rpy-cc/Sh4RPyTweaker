using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Sh4RPyTweaker.Models;
using Sh4RPyTweaker.Services;
using Sh4RPyTweaker.Views;

namespace Sh4RPyTweaker
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            NavList.ItemsSource = new List<NavPage>
            {
                new NavPage { Title = "Главная", Glyph = "\uE80F", View = typeof(DashboardView) },
                new NavPage { Title = "О системе", Glyph = "\uE946", View = typeof(SystemInfoView) },
                new NavPage { Title = "Персонализация", Glyph = "\uE790", View = typeof(PersonalizationView) },
                new NavPage { Title = "Производительность", Glyph = "\uE945", View = typeof(PerformanceView) },
                new NavPage { Title = "Конфиденциальность", Glyph = "\uE890", View = typeof(PrivacyView) },
                new NavPage { Title = "Твики системы", Glyph = "\uE713", View = typeof(TweaksView) },
                new NavPage { Title = "Процессы", Glyph = "\uE70B", View = typeof(ProcessesView) },
                new NavPage { Title = "Очистка системы", Glyph = "\uE74D", View = typeof(CleanupView) },
                new NavPage { Title = "Приложения", Glyph = "\uE7C4", View = typeof(UwpView) },
                new NavPage { Title = "Резервные копии", Glyph = "\uE74E", View = typeof(BackupView) }
            };

            NavList.SelectedIndex = 0;
        }

        private void NavList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NavList.SelectedItem is NavPage page && page.View != null)
            {
                ContentHost.Content = (FrameworkElement)System.Activator.CreateInstance(page.View);
                PageTitle.Text = page.Title;
                ContentScroll?.ScrollToTop();
            }
        }

        private async void BackupButton_Click(object sender, RoutedEventArgs e)
        {
            string dir = await System.Threading.Tasks.Task.Run(() => BackupService.CreateBackup());
            MessageBox.Show(
                "Резервная копия создана:\n" + dir,
                "Резервное копирование",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}

