using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Models.Entities;

public class Products
{
    [Key]
    public int Id { get; set; }
    public string Article { get; set; }
    public virtual ProductNames ProductNames { get; set; }
    public int NameId { get; set; }
    public virtual ProductMeasures ProductMeasure { get; set; }
    public int ProductMeasureId { get; set; }
    public decimal Price { get; set; }
    public virtual ProductSuppliers ProductSupplier { get; set; }
    public int ProductSupplierId { get; set; }
    public virtual ProductManufacturers ProductManufacturer { get; set; }
    public int ProductManufacturerId { get; set; }
    public virtual ProductCategories ProductCategory { get; set; }
    public int ProductCategoryId { get; set; }
    public int Discount { get; set; }
    public int Count { get; set; }
    public string Description { get; set; }
    public string? PhotoName { get; set; }

    [NotMapped]
    public decimal FinalPrice => Price - Price * Discount / 100;

    [NotMapped]
    public bool HasDiscount => Discount > 0;

    [NotMapped]
    public bool IsOutOfStock => Count == 0;

    [NotMapped]
    public bool IsHighDiscount => Discount > 15;

    [NotMapped]
    public string DisplayImagePath
    {
        get
        {
            // Если есть путь и файл существует - используем его
            if (!string.IsNullOrEmpty(PhotoName) && File.Exists(PhotoName))
                return PhotoName;

            // Иначе возвращаем путь к заглушке
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "picture.png");
        }
    }
}

