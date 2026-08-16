using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Sh4RPyTweaker.Models;

namespace Sh4RPyTweaker.Views
{
    public partial class PersonalizationView : UserControl
    {
        public PersonalizationView()
        {
            InitializeComponent();
            Rows.ItemsSource = Services.TweakCatalog.Personalization;
        }

        private void ApplySelected_Click(object sender, RoutedEventArgs e)
        {
            var selected = Rows.ItemsSource.OfType<Tweak>().Where(t => t.IsSelected).ToList();
            if (selected.Count == 0)
            {
                MessageBox.Show("Не выбрано ни одного твика. Отметьте нужные твики галочками.",
                    "Применить выбранные", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (MessageBox.Show("Включить выбранные твики (" + selected.Count + " шт.)?",
                "Применить выбранные", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                foreach (Tweak t in selected) t.IsApplied = true;
            }
        }

        private void ApplyAll_Click(object sender, RoutedEventArgs e)
        {
            if (ConfirmApplyAll()) ApplyAll(true);
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Отключить все твики этого раздела?",
                "Отключить все", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                ApplyAll(false);
            }
        }

        private bool ConfirmApplyAll()
        {
            return MessageBox.Show("Включить все твики этого раздела?",
                "Применить все", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }

        private void ApplyAll(bool value)
        {
            foreach (Tweak t in Rows.ItemsSource)
            {
                t.IsApplied = value;
            }
        }
    }
}

