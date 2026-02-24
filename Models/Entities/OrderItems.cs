using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Models.Entities;

public class OrderItems
{
    public int Id { get; set; }
    public virtual Orders Order { get; set; }
    public int OrderId { get; set; }
    public virtual Products Product { get; set; }
    public string ProductName { get; set; }
    public int Count { get; set; }
}

