using Demo.Models;
using Demo.Models.Entities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для ProductEditWindow.xaml
    /// </summary>
    public partial class ProductEditWindow : Window
    {
        private Products currentProduct;
        private bool isEditMode;
        private string originalImagePath;
        private bool isImageChanged = false;


        public ProductEditWindow(Products product = null)
        {
            InitializeComponent();

            currentProduct = product ?? new Products();
            isEditMode = product != null;

            LoadComboBoxes();

            if (isEditMode)
            {
                txtWindowTitle.Text = "Редактирование товара";
                btnDelete.Visibility = Visibility.Visible;
                LoadProductData();
            }
            else
            {
                txtWindowTitle.Text = "Добавление товара";
                txtId.Text = "(новый)";
                imgProduct.Source = new BitmapImage(new Uri(currentProduct.DisplayImagePath));
            }
        }

        private void LoadComboBoxes()
        {
            try
            {
                using (var context = new ContextDB())
                {
                    cmbCategory.ItemsSource = context.ProductCategories.ToList();
                    cmbManufacturer.ItemsSource = context.ProductManufacturers.ToList();
                    cmbSupplier.ItemsSource = context.ProductSuppliers.ToList();
                    cmbMeasure.ItemsSource = context.ProductMeasures.ToList();
                    cmbName.ItemsSource = context.ProductNames.ToList();
                }
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void LoadProductData()
        {
            txtId.Text = currentProduct.Id.ToString();
            txtArticle.Text = currentProduct.Article;
            txtDescription.Text = currentProduct.Description;
            txtPrice.Text = currentProduct.Price.ToString();
            txtCount.Text = currentProduct.Count.ToString();
            txtDiscount.Text = currentProduct.Discount.ToString();

            cmbName.SelectedValue = currentProduct.ProductNameId;
            cmbCategory.SelectedValue = currentProduct.ProductCategoryId;
            cmbManufacturer.SelectedValue = currentProduct.ProductManufacturerId;
            cmbSupplier.SelectedValue = currentProduct.ProductSupplierId;
            cmbMeasure.SelectedValue = currentProduct.ProductMeasureId;

            originalImagePath = currentProduct.PhotoName;
            imgProduct.Source = new BitmapImage(new Uri(currentProduct.DisplayImagePath));
        }

        private void BtnSelectImage_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Image files|*.jpg;*.png",
                Title = "Выберите изображение товара"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    // Загружаем и ресайзим изображение
                    var image = new BitmapImage(new Uri(dialog.FileName));

                    // Проверяем размер и ресайзим если нужно
                    if (image.PixelWidth > 300 || image.PixelHeight > 200)
                    {
                        throw new Exception("Изображение слишком высокого качества");
                    }

                    imgProduct.Source = image;
                    isImageChanged = true;

                    string fileName = $"{Guid.NewGuid()}.jpg";

                    // Путь к папке Images
                    string imagesPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
                    string savePath = System.IO.Path.Combine(imagesPath, fileName);

                    // Копируем файл
                    File.Copy(dialog.FileName, savePath, true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}");
                }
            }
        }

        private void BtnDeleteImage_Click(object sender, RoutedEventArgs e)
        {
            imgProduct.Source = null;
            isImageChanged = true;
        }

        private void DeleteOriginalImage()
        {
            if (isImageChanged)
            {
                // Удаляем старое фото
                if (!string.IsNullOrEmpty(originalImagePath))
                {
                    File.Delete(originalImagePath);
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                using (var context = new ContextDB())
                {
                    // Заполняем данные
                    currentProduct.Article = txtArticle.Text.Trim();
                    currentProduct.Description = txtDescription.Text.Trim();
                    currentProduct.Price = decimal.Parse(txtPrice.Text);
                    currentProduct.Count = int.Parse(txtCount.Text);
                    currentProduct.Discount = int.Parse(txtDiscount.Text);

                    currentProduct.ProductNameId = (int)cmbName.SelectedValue;
                    currentProduct.ProductCategoryId = (int)cmbCategory.SelectedValue;
                    currentProduct.ProductManufacturerId = (int)cmbManufacturer.SelectedValue;
                    currentProduct.ProductSupplierId = (int)cmbSupplier.SelectedValue;
                    currentProduct.ProductMeasureId = (int)cmbMeasure.SelectedValue;

                    // Обнуляем навигационные свойства, чтобы EF не перезаписал ID
                    currentProduct.ProductName = null;
                    currentProduct.ProductCategory = null;
                    currentProduct.ProductManufacturer = null;
                    currentProduct.ProductSupplier = null;
                    currentProduct.ProductMeasure = null;

                    // Обработка изображения
                    if (isImageChanged)
                    {
                        currentProduct.PhotoName = null;

                        // Сохраняем новое фото
                        if (imgProduct.Source is BitmapImage bitmap)
                        {
                            string fileName = $"{Guid.NewGuid()}.jpg";
                            string savePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", fileName);

                            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(savePath));
                            File.Copy(bitmap.UriSource.LocalPath, savePath, true);

                            currentProduct.PhotoName = savePath;
                        }
                    }

                    // Сохраняем в БД
                    if (isEditMode)
                    {
                        context.Products.Update(currentProduct);
                    }
                    else
                    {
                        context.Products.Add(currentProduct);
                    }

                    context.SaveChanges();

                    DeleteOriginalImage();
                    DialogResult = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка сохранения: {ex.Message}");
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private bool ValidateInput()
        {
            // Проверка артикля
            if(txtArticle.Text.Trim().Length == 0)
            {
                ShowError("Артикул должен быть заполнен");
                txtArticle.Focus();
                return false;
            }

            // Проверка цены
            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price < 0)
            {
                ShowError("Цена должна быть положительным числом");
                txtPrice.Focus();
                return false;
            }

            // Проверка количества
            if (!int.TryParse(txtCount.Text, out int count) || count < 0)
            {
                ShowError("Количество должно быть целым положительным числом");
                txtCount.Focus();
                return false;
            }

            // Проверка скидки
            if (!int.TryParse(txtDiscount.Text, out int discount) || discount < 0 || discount > 100)
            {
                ShowError("Скидка должна быть числом от 0 до 100");
                txtDiscount.Focus();
                return false;
            }

            // Проверка выбора из списков
            if (cmbName.SelectedValue == null)
            {
                ShowError("Выберите название товара");
                return false;
            }

            if (cmbCategory.SelectedValue == null)
            {
                ShowError("Выберите категорию товара");
                return false;
            }

            if (cmbManufacturer.SelectedValue == null)
            {
                ShowError("Выберите производителя");
                return false;
            }

            if (cmbSupplier.SelectedValue == null)
            {
                ShowError("Выберите поставщика");
                return false;
            }

            if (cmbMeasure.SelectedValue == null)
            {
                ShowError("Выберите единицу измерения");
                return false;
            }

            return true;
        }

        private void ShowError(string message)
        {
            txtError.Text = message;
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show($"Удалить товар '{currentProduct.ProductName.Name}'?\n\nВНИМАНИЕ: Если товар присутствует в заказах, удаление будет невозможно!",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new ContextDB())
                    {
                        // Проверяем, есть ли товар в заказах
                        var isInOrders = context.OrderItems.Any(oi => oi.ProductId == currentProduct.Id);

                        if (isInOrders)
                        {
                            MessageBox.Show("Товар нельзя удалить, так как он присутствует в заказах",
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                            return;
                        }

                        context.Products.Remove(currentProduct);
                        context.SaveChanges();
                    }

                    DeleteOriginalImage();
                    DialogResult = true;
                    this.Close();
                }
                catch (Exception ex)
                {
                    ShowError($"Ошибка удаления: {ex.Message}");
                }
            }
        }
    }
}
