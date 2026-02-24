using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Models.Entities;

public class Orders
{
    public int Id { get; set; }
    public DateOnly OrderDate { get; set; }
    public DateOnly Deliverydate { get; set; }
    public virtual PickupPoints PickupPoint { get; set; }
    public int PickupPointId { get; set; }
    public virtual Users User { get; set; }
    public int UserId { get; set; }
    public int Code { get; set; }
    public virtual OrderStatuses OrderStatus { get; set; }
    public int StatusId { get; set; }
}

