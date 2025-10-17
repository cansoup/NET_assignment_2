﻿using DineConnect.App.Data;
using DineConnect.App.Views.Auth;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace DineConnect.App
{
    public partial class App : Application
    {
        private static bool _mainCreated = false;
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            using var db = new DineConnectContext();
            await DbSeed.EnsureCreatedAndSeedAsync(db);

            var login = new LoginWindow
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            bool? ok = login.ShowDialog(); // must be ShowDialog()

            if (ok == true && !_mainCreated)
            {
                _mainCreated = true; // prevent any accidental second creation
                var main = new MainWindow();
                Current.MainWindow = main;   // set before Show
                main.Show();

                // flip back on the next tick (prevents “already shutting down” race)
                Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    if (Current != null)
                        Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
                }));
            }
            else
            {
                Current.Shutdown();
            }
        }

    }
}