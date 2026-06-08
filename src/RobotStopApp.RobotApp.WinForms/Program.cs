using System;
using System.Net.Http;
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;
using RobotStopApp.RobotApp.WinForms.Services;

namespace RobotStopApp.RobotApp.WinForms
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var settings = LoadSettings();
            using (var httpClient = new HttpClient())
            {
                var apiClient = new RobotApiClient(httpClient);
                Application.Run(new MainForm(apiClient, settings));
            }
        }

        private static RobotApiSettings LoadSettings()
        {
            try
            {
                IConfiguration config = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                    .Build();

                var section = config.GetSection("RobotApp");
                var bound = section.Get<RobotApiSettings>();
                return bound ?? new RobotApiSettings();
            }
            catch
            {
                return new RobotApiSettings();
            }
        }
    }
}
