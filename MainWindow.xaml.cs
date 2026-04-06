using Demo.Models;
using Demo.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Products> allProducts;


        public MainWindow()
        {
            InitializeComponent();

            ConfigureUIBasedOnRole();
            LoadProducts();
            LoadFilters();
            DisplayUserInfo();
        }

        private void ConfigureUIBasedOnRole()
        {
            if (App.CurrentUser.RoleId == 1 || App.CurrentUser.RoleId == 2)
            {
                panelFilters.Visibility = Visibility.Visible;
            }
            else { panelFilters.Visibility = Visibility.Collapsed;}

            btnAddProduct.Visibility = App.CurrentUser.RoleId == 1
                ? Visibility.Visible
                : Visibility.Collapsed;

            btnOpenOrders.Visibility = App.CurrentUser.RoleId == 1
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void DisplayUserInfo()
        {
            if (App.CurrentUser.RoleId == 4) // Гость
            {
                txtUserInfo.Text = "Вы вошли как: Гость";
            }
            else
            {
                txtUserInfo.Text = $"{App.CurrentUser.Name}";
            }
        }

        private void LoadProducts()
        {
            try
            {
                using (var context = new ContextDB())
                {
                    allProducts = context.Products
                        .Include(p => p.ProductCategory)
                        .Include(p => p.ProductManufacturer)
                        .Include(p => p.ProductSupplier)
                        .Include(p => p.ProductMeasure)
                        .Include(p => p.ProductName)
                        .ToList();

                    ApplyFiltersAndSort();
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show($"Ошибка загрузки товаров: {ex.Message}");
            }
        }

        private void LoadFilters()
        {
            try
            {
                using (var context = new ContextDB())
                {
                    var suppliers = context.ProductSuppliers.ToList();

                    cmbSupplierFilter.Items.Clear();
                    cmbSupplierFilter.Items.Add(new ComboBoxItem { Content = "Все поставщики", Tag = 0 });

                    foreach (var supplier in suppliers)
                    {
                        cmbSupplierFilter.Items.Add(new ComboBoxItem
                        {
                            Content = supplier.Name,
                            Tag = supplier.Id
                        });
                    }

                    cmbSupplierFilter.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки фильтров: {ex.Message}");
            }
        }

        private void ApplyFiltersAndSort()
        {
            IEnumerable<Products> filtered = allProducts;

            string searchText = txtSearch.Text?.ToLower() ?? "";
            if (!string.IsNullOrEmpty(searchText))
            {
                filtered = filtered.Where(p =>
                    (p.ProductName?.Name?.ToLower().Contains(searchText) ?? false) ||
                    (p.Description?.ToLower().Contains(searchText) ?? false) ||
                    (p.ProductCategory?.Name?.ToLower().Contains(searchText) ?? false) ||
                    (p.ProductManufacturer?.Name?.ToLower().Contains(searchText) ?? false) ||
                    (p.ProductSupplier?.Name?.ToLower().Contains(searchText) ?? false)
                    );
            }

            if (cmbSupplierFilter.SelectedItem is ComboBoxItem supplierItem &&
                supplierItem.Tag is int supplierId && supplierId > 0)
            {
                filtered = filtered.Where(p => p.ProductSupplierId == supplierId);
            }

            if (cmbSort.SelectedItem is ComboBoxItem sortItem && sortItem.Tag is string sortTag)
            {
                switch (sortTag)
                {
                    case "1": // По возрастанию
                        filtered = filtered.OrderBy(p => p.Count);
                        break;
                    case "2": // По убыванию
                        filtered = filtered.OrderByDescending(p => p.Count);
                        break;
                }
            }

            listProducts.ItemsSource = filtered.ToList();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы действительно хотите выйти?",
                "Выход", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                App.CurrentUser = null;
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFiltersAndSort();
        }

        private void cmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFiltersAndSort();
        }

        private void cmbSupplierFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFiltersAndSort();
        }

        private void listProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (App.CurrentUser.RoleId == 1 && listProducts.SelectedItem is Products selectedProduct)
            {
                var editWindow = new ProductEditWindow(selectedProduct);
                if (editWindow.ShowDialog() == true)
                {
                    // Обновляем список после редактирования
                    LoadProducts();
                }
                // Снимаем выделение
                listProducts.SelectedItem = null;
            }
        }

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new ProductEditWindow();
            if (editWindow.ShowDialog() == true)
            {
                // Обновляем список после добавления
                LoadProducts();
            }
        }

        private void btnOpenOrders_Click(object sender, RoutedEventArgs e)
        {
            var ordersWindow = new OrdersWindow();
            ordersWindow.ShowDialog();
        }
    }
}