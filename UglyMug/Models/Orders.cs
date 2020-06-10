using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UglyMug.Models
{
    public class Orders
    {
        public int OrderNumber { get; set; }
        public string OrderName { get; set; }
        public DateTime OrderTime { get; set; }
        public string OrderStatus { get; set; }
        public Order Order { get; set; }
    }

    public class Order
    {
        public string OrderDetails { get; set; }
        public int OrderQuantity { get; set; }
    }
}
