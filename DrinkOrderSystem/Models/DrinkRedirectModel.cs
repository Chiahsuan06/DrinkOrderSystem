using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrinkOrderSystem.Models
{
    public class DrinkRedirectModel
    {
        public Guid OrderDetailsID { get; set; }
        public string OrderNumber { get; set; }

        public string Account { get; set; }

        public string OrderTime { get; set; }

        public string OrderEndTime { get; set; }

        public string RequiredTime { get; set; }

        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public string Suger { get; set; }

        public string Ice { get; set; }

        public string Toppings { get; set; }

        public decimal ToppingsUnitPrice { get; set; }

        public string OtherRequest { get; set; }

        public string SupplierName { get; set; }
        
        //這筆訂單的總金額，非一整大筆的總金額
        public decimal SubtotalAmount
        {
            get
            {
                return this.Quantity * (this.UnitPrice + this.ToppingsUnitPrice);
            }
        }
    }
}
