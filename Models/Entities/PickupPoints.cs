using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Models.Entities;

public class PickupPoints
{
    public int Id { get; set; }
    public string Index { get; set; }
    public string City { get; set; }
    public string Address { get; set; }

    [NotMapped]
    public string FullAddress => $"{Index}, {City}, {Address}";
}

