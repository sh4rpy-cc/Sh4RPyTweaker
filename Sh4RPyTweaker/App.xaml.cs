using System.Security.Principal;
using System.Windows;

namespace Sh4RPyTweaker
{
    public partial class App : Application
    {
        public static bool IsAdministrator()
        {
            using (var identity = WindowsIdentity.GetCurrent())
            {
                var principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            DispatcherUnhandledException += OnDispatcherUnhandledException;
            System.AppDomain.CurrentDomain.UnhandledException += OnDomainUnhandledException;

            if (!IsAdministrator())
            {
                MessageBox.Show(
                    "Sh4RPyTweaker требует прав администратора.\n\n" +
                    "Запустите приложение от имени администратора.",
                    "Нет прав администратора",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                Shutdown();
                return;
            }

            ShutdownMode = ShutdownMode.OnMainWindowClose;
        }

        private void OnDispatcherUnhandledException(object sender,
            System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(
                "Произошла непредвиденная ошибка:\n" + e.Exception,
                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        private void OnDomainUnhandledException(object sender,
            System.UnhandledExceptionEventArgs e)
        {
            try
            {
                if (e.ExceptionObject is System.Exception ex)
                {
                    MessageBox.Show(
                        "Произошла критическая ошибка:\n" + ex,
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch
            {
            }
        }
    }
}

