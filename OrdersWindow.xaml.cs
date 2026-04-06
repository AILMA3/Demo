using Demo.Models;
using Demo.Models.Entities;
using Microsoft.EntityFrameworkCore;
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
    /// Логика взаимодействия для OrdersWindow.xaml
    /// </summary>
    public partial class OrdersWindow : Window
    {
        private List<Orders> allOrders;

        public OrdersWindow()
        {
            InitializeComponent();
            LoadOrders();

            btnAddOrder.Visibility = App.CurrentUser.RoleId == 1
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void LoadOrders()
        {
            try
            {
                using (var context = new ContextDB())
                {
                    allOrders = context.Orders
                        .Include(o => o.Status)
                        .Include(o => o.PickupPoint)
                        .Include(o => o.OrderItems)
                            .ThenInclude(oi => oi.Product)
                        .ToList();
                }

                listOrders.ItemsSource = allOrders;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заказов: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void listOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (App.CurrentUser.RoleId == 1 && listOrders.SelectedItem is Orders selectedOrder)
            {
                var editWindow = new OrderEditWindow(selectedOrder);
                if (editWindow.ShowDialog() == true)
                {
                    LoadOrders(); // Обновляем список после редактирования
                }
            }
        }

        private void btnAddOrder_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new OrderEditWindow();
            if (editWindow.ShowDialog() == true)
            {
                LoadOrders(); // Обновляем список после добавления
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
