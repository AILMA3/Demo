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

        private static bool isWindowOpen = false;


        public ProductEditWindow(Products product = null)
        {
            if (isWindowOpen)
            {
                MessageBox.Show("Окно редактирования товара уже открыто");
                this.Close();
                return;
            }

            InitializeComponent();

            isWindowOpen = true;
            currentProduct = product ?? new Products();
            isEditMode = product != null;

            LoadComboBoxes();

            if (isEditMode)
            {
                txtWindowTitle.Text = "Редактирование товара";
                LoadProductData();
            }
            else
            {
                txtWindowTitle.Text = "Добавление товара";
                txtId.Text = "(новый)";
                imgProduct.Source = LoadImage(null);
            }
        }

        private void LoadComboBoxes()
        {
            try
            {
                using (var context = new ContextDB())
                {
                    cmbCategory.ItemsSource = context.ProductCategorys.ToList();
                    cmbManufacturer.ItemsSource = context.ProductManufacturers.ToList();
                    cmbSupplier.ItemsSource = context.ProductSuppliers.ToList();
                    cmbMeasure.ItemsSource = context.ProductMeasures.ToList();
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
            txtName.Text = currentProduct.ProductNames.Name;
            txtDescription.Text = currentProduct.Description;
            txtPrice.Text = currentProduct.Price.ToString();
            txtCount.Text = currentProduct.Count.ToString();
            txtDiscount.Text = currentProduct.Discount.ToString();

            cmbCategory.SelectedValue = currentProduct.ProductCategoryId;
            cmbManufacturer.SelectedValue = currentProduct.ProductManufacturerId;
            cmbSupplier.SelectedValue = currentProduct.ProductSupplierId;
            cmbMeasure.SelectedValue = currentProduct.ProductMeasureId;

            originalImagePath = currentProduct.PhotoName;
            imgProduct.Source = LoadImage(currentProduct.PhotoName);
        }

        private BitmapImage LoadImage(string path)
        {
            try
            {
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
                    bitmap.EndInit();
                    return bitmap;
                }
            }
            catch { }

            // Заглушка
            return new BitmapImage(new Uri("pack://application:,,,/Resources/picture.png"));
        }

        private void BtnSelectImage_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Image files|*.jpg;*.jpeg;*.png;*.bmp",
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
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}");
                }
            }
        }

        private void BtnDeleteImage_Click(object sender, RoutedEventArgs e)
        {
            imgProduct.Source = LoadImage(null);
            isImageChanged = true;
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
                    currentProduct.ProductNames.Name = txtName.Text.Trim();
                    currentProduct.Description = txtDescription.Text.Trim();
                    currentProduct.Price = int.Parse(txtPrice.Text);
                    currentProduct.Count = int.Parse(txtCount.Text);
                    currentProduct.Discount = int.Parse(txtDiscount.Text);

                    currentProduct.ProductCategoryId = (int)cmbCategory.SelectedValue;
                    currentProduct.ProductManufacturerId = (int)cmbManufacturer.SelectedValue;
                    currentProduct.ProductSupplierId = (int)cmbSupplier.SelectedValue;
                    currentProduct.ProductMeasureId = (int)cmbMeasure.SelectedValue;

                    // Обработка изображения
                    if (isImageChanged)
                    {
                        // Удаляем старое фото если есть
                        if (!string.IsNullOrEmpty(originalImagePath) && File.Exists(originalImagePath))
                        {
                            File.Delete(originalImagePath);
                        }

                        // Сохраняем новое фото
                        if (imgProduct.Source is BitmapImage bitmap && bitmap.UriSource != null)
                        {
                            string fileName = $"{Guid.NewGuid()}.jpg";
                            string savePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", fileName);

                            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(savePath));
                            File.Copy(bitmap.UriSource.LocalPath, savePath, true);

                            currentProduct.PhotoName = savePath;
                        }
                        else
                        {
                            currentProduct.PhotoName = null;
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
            // Проверка обязательных полей
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                ShowError("Введите наименование товара");
                txtName.Focus();
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
            if (!decimal.TryParse(txtDiscount.Text, out decimal discount) || discount < 0 || discount > 100)
            {
                ShowError("Скидка должна быть числом от 0 до 100");
                txtDiscount.Focus();
                return false;
            }

            // Проверка выбора из списков
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

        private void Window_Closed(object sender, EventArgs e)
        {
            isWindowOpen = false;
        }
    }
}
