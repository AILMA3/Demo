using Demo.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Models;

public class ContextDB : DbContext
{
    public DbSet<Users> Users { get; set; }
    public DbSet<Roles> Roles { get; set; }
    public DbSet<Products> Products { get; set; }
    public DbSet<ProductNames> ProductNames { get; set; }
    public DbSet<ProductMeasures> ProductMeasures { get; set; }
    public DbSet<ProductSuppliers> ProductSuppliers { get; set; }
    public DbSet<ProductManufacturers> ProductManufacturers { get; set; }
    public DbSet<ProductCategories> ProductCategorys { get; set; }
    public DbSet<Orders> Orders { get; set; }
    public DbSet<OrderItems> OrderItems { get; set; }   
    public DbSet<OrderStatuses> OrderStatuses { get; set; }
    public DbSet<PickupPoints> PickupPoints { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=Demo;Username=user;Password=");
    }

}

