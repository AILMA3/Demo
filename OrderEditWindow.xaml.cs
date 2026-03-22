using Demo.Models;
using Demo.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для OrderEditWindow.xaml
    /// </summary>
    public partial class OrderEditWindow : Window
    {
        private Orders currentOrder;
        private bool isEditMode;
        private ObservableCollection<OrderItems> orderItems;

        public List<Products> ProductsList { get; set; }

        public OrderEditWindow(Orders order = null)
        {
            InitializeComponent();

            currentOrder = order ?? new Orders();
            isEditMode = order != null;

            LoadComboBoxes();
            LoadProducts();

            if (isEditMode)
            {
                txtWindowTitle.Text = "Редактирование заказа";
                btnDelete.Visibility = Visibility.Visible;
                LoadOrderData();
            }
            else
            {
                txtWindowTitle.Text = "Добавление заказа";
                txtOrderNumber.Text = ("Новый");
                dpOrderDate.SelectedDate = DateTime.Today;
                orderItems = new ObservableCollection<OrderItems>();
                dgOrderItems.ItemsSource = orderItems;
            }
        }

        private void LoadComboBoxes()
        {
            try
            {
                using (var context = new ContextDB())
                {
                    cmbUsers.ItemsSource = context.Users.ToList();
                    cmbStatus.ItemsSource = context.OrderStatuses.ToList();
                    cmbPickupPoint.ItemsSource = context.PickupPoints.ToList();
                }
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void LoadProducts()
        {
            try
            {
                using (var context = new ContextDB())
                {
                    ProductsList = context.Products
                        .Select(p => new Products
                        {
                            Id = p.Id,
                            Article = p.Article
                        })
                        .ToList();
                }

                // Обновляем ComboBox колонку
                var comboBoxColumn = dgOrderItems.Columns[0] as DataGridComboBoxColumn;
                if (comboBoxColumn != null)
                {
                    comboBoxColumn.ItemsSource = ProductsList;
                }
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка загрузки товаров: {ex.Message}");
            }
        }

        private void LoadOrderData()
        {
            try
            {
                using (var context = new ContextDB())
                {
                    var order = context.Orders
                        .Include(o => o.OrderItems)
                        .FirstOrDefault(o => o.Id == currentOrder.Id);

                    if (order != null)
                    {
                        txtOrderNumber.Text = order.Id.ToString();
                        cmbUsers.SelectedValue = order.UserId;
                        cmbStatus.SelectedValue = order.StatusId;
                        cmbPickupPoint.SelectedValue = order.PickupPointId;
                        dpOrderDate.SelectedDate = order.OrderDate.ToDateTime(TimeOnly.MinValue);
                        dpDeliveryDate.SelectedDate = order.DeliveryDate.ToDateTime(TimeOnly.MinValue);

                        orderItems = new ObservableCollection<OrderItems>();
                        foreach (var item in order.OrderItems)
                        {
                            var product = ProductsList.FirstOrDefault(p => p.Id == item.ProductId);
                            orderItems.Add(new OrderItems
                            {
                                Id = item.Id,
                                ProductId = item.ProductId,
                                Count = item.Count
                            });
                        }
                        dgOrderItems.ItemsSource = orderItems;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка загрузки данных заказа: {ex.Message}");
            }
        }

        private void btnAddItem_Click(object sender, RoutedEventArgs e)
        {
            orderItems.Add(new OrderItems
            {
                ProductId = 0,
                Count = 1
            });

            dgOrderItems.Items.Refresh();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                using (var context = new ContextDB())
                {
                    currentOrder.StatusId = (int)cmbStatus.SelectedValue;
                    currentOrder.PickupPointId = (int)cmbPickupPoint.SelectedValue;
                    currentOrder.UserId = (int)cmbUsers.SelectedValue;
                    currentOrder.OrderDate = DateOnly.FromDateTime(dpOrderDate.SelectedDate ?? DateTime.Now);
                    currentOrder.DeliveryDate = DateOnly.FromDateTime((DateTime)dpDeliveryDate.SelectedDate);

                    if (isEditMode)
                    {
                        // Удаляем старые позиции заказа
                        var existingItems = context.OrderItems.Where(oi => oi.OrderId == currentOrder.Id);
                        context.OrderItems.RemoveRange(existingItems);
                    }
                    else
                    {
                        context.Orders.Add(currentOrder);
                    }
                    context.SaveChanges();

                    // Добавляем новые позиции заказа
                    foreach (var item in orderItems)
                    {
                        if (item.ProductId > 0 && item.Count > 0)
                        {
                            var product = ProductsList.FirstOrDefault(p => p.Id == item.ProductId);
                            context.OrderItems.Add(new OrderItems
                            {
                                OrderId = currentOrder.Id,
                                ProductId = item.ProductId,
                                Count = item.Count
                            });
                        }
                    }

                    context.SaveChanges();

                    DialogResult = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка сохранения: {ex.Message}");
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Удалить заказ и все связанные с ним товары?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new ContextDB())
                    {
                        // Удаляем позиции заказа
                        var orderItems = context.OrderItems.Where(oi => oi.OrderId == currentOrder.Id);
                        context.OrderItems.RemoveRange(orderItems);

                        // Удаляем заказ
                        context.Orders.Remove(currentOrder);
                        context.SaveChanges();
                    }

                    DialogResult = true;
                    this.Close();
                }
                catch (Exception ex)
                {
                    ShowError($"Ошибка удаления: {ex.Message}");
                }
            }
        }

        private bool ValidateInput()
        {
            if (cmbStatus.SelectedValue == null)
            {
                ShowError("Выберите статус заказа");
                return false;
            }

            if (cmbUsers.SelectedValue == null)
            {
                ShowError("Выберите покупателя");
                return false;
            }

            if (cmbPickupPoint.SelectedValue == null)
            {
                ShowError("Выберите пункт выдачи");
                return false;
            }

            if (dpOrderDate.SelectedDate == null)
            {
                ShowError("Укажите дату заказа");
                return false;
            }

            if (dpDeliveryDate.SelectedDate == null)
            {
                ShowError("Укажите дату доставки");
                return false;
            }

            if (orderItems.Count == 0)
            {
                ShowError("Добавьте хотя бы один товар в заказ");
                return false;
            }

            foreach (var item in orderItems)
            {
                if (item.ProductId == 0)
                {
                    ShowError("Выберите товар для каждой позиции");
                    return false;
                }
                if (item.Count <= 0)
                {
                    ShowError("Количество товара должно быть больше 0");
                    return false;
                }
            }

            return true;
        }

        private void ShowError(string message)
        {
            txtError.Text = message;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
}
