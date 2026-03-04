using Demo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Demo
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                ShowError("Заполните логин и пароль");
                return;
            }

            try
            {
                using (var context = new ContextDB())
                {
                    var user = context.Users
                        .FirstOrDefault(u => u.Login == login && u.Password == password);

                    if (user == null)
                    {
                        ShowError("Неверный логин или пароль");
                        return;
                    }

                    App.CurrentUser = user;

                    OpenMainWindow();
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show($"Ошибка подключения к базе данных: {ex.Message}");
            }
        }

        private void BtnGuest_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentUser = new Models.Entities.Users
            {
                Id = 0,
                Name = "Гость",
                RoleId = 1
            };

            OpenMainWindow();
        }

        private void OpenMainWindow()
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void ShowError(string message)
        {
            txtError.Text = message;
        }
    }
}
