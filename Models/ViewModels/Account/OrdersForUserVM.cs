using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Store_MVC.Models.ViewModels.Account
{
    public class OrdersForUserVM
    {
        [DisplayName("Order number")]
        public int OrderNumber { get; set; }
        public decimal Total { get; set; }
        [DisplayName("Odrer details")]
        public Dictionary<string, int> ProductsAndQuantity { get; set; }
        [DisplayName("Created At")]
        public DateTime CreatedAt { get; set; }
    }
}