using Demo.Models;
using Demo.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Data;
using System.Windows;

namespace Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Users CurrentUser { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            using (var context = new ContextDB())
            {
                context.Database.Migrate();
            }

            var loginWindow = new LoginWindow();
            loginWindow.Show();
        }
    }

}
