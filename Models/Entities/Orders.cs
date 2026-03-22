using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Models.Entities;

public class Orders
{
    public int Id { get; set; }
    public DateOnly OrderDate { get; set; }
    public DateOnly DeliveryDate { get; set; }
    public virtual PickupPoints PickupPoint { get; set; }
    public int PickupPointId { get; set; }
    public virtual Users User { get; set; }
    public int UserId { get; set; }
    public int Code { get; set; }
    public virtual OrderStatuses Status { get; set; }
    public int StatusId { get; set; }

    public virtual ICollection<OrderItems> OrderItems { get; set; }

    //[NotMapped]
    //public string ProductsInfo => string.Join(",\n", OrderItems?.Select(oi => $"{oi.Product.Article} ({oi.Count} шт)") ?? new List<string>());

    [NotMapped]
    public string ProductsInfo
    {
        get
        {
            if (OrderItems == null || !OrderItems.Any())
                return "Нет товаров";
            return string.Join(", ", OrderItems.Select(oi =>
                $"{oi.Product.Article} x{oi.Count}"));
        }
    }
}

