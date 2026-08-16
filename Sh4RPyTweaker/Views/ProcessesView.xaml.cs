using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Sh4RPyTweaker.Models;
using Sh4RPyTweaker.Services;

namespace Sh4RPyTweaker.Views
{
    public partial class ProcessesView : UserControl
    {
        public ProcessesView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await Refresh();
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e)
        {
            await Refresh();
        }

        private async System.Threading.Tasks.Task Refresh()
        {
            RefreshBtn.IsEnabled = false;
            Status.Text = "Обновление списка...";
            var items = await System.Threading.Tasks.Task.Run(() => ProcessService.GetProcesses());
            ProcList.ItemsSource = items;
            Status.Text = "Процессов: " + items.Count + "  ·  обновлено " + System.DateTime.Now.ToString("HH:mm:ss");
            RefreshBtn.IsEnabled = true;
        }

        private void Kill_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelected();
            if (selected.Count == 0) return;

            var blocked = selected.Where(p => p.IsProtected).ToList();
            var killable = selected.Where(p => !p.IsProtected).ToList();

            if (blocked.Count > 0)
            {
                string names = string.Join(", ", blocked.Take(5).Select(p => p.Name));
                if (blocked.Count > 5) names += " и др.";
                MessageBox.Show(
                    "Нельзя завершить системные процессы:\n" + names + "\n\n" +
                    "Завершение этих процессов приведёт к нестабильности или сбою Windows.",
                    "Завершение запрещено", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (killable.Count == 0) return;

            if (MessageBox.Show("Завершить " + killable.Count + " процесс(ов)?",
                "Завершение процессов", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            {
                return;
            }

            foreach (var item in killable)
            {
                ProcessService.Kill(item);
            }
            _ = Refresh();
        }

        private void OpenLocation_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelected();
            if (selected.Count > 0)
            {
                ProcessService.OpenFileLocation(selected[0]);
            }
        }

        private List<ProcessItem> GetSelected()
        {
            var list = new List<ProcessItem>();
            if (ProcList.SelectedItems == null) return list;
            foreach (var o in ProcList.SelectedItems)
            {
                if (o is ProcessItem p) list.Add(p);
            }
            return list;
        }
    }
}

