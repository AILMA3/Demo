using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Models.Entities;

public class Products
{
    [Key]
    public string Article { get; set; }
    public virtual ProductNames ProductNames { get; set; }
    public int NameId { get; set; }
    public virtual ProductMeasures ProductMeasure { get; set; }
    public int ProductMeasureId { get; set; }
    public int Price { get; set; }
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
}

