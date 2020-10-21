using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Store_MVC.Areas.Admin.Models.ViewModels.Shop
{
    public class OrdersForAdminVM
    {
        [DisplayName("Order number")]
        public int OrderNumber { get; set; }
        [DisplayName("User name")]
        public string UserName { get; set; }
        public decimal Total { get; set; }
        [DisplayName("Odrer details")]
        public Dictionary<string, int> ProductsAndQuantity { get; set; }
        [DisplayName("Created At")]
        public DateTime CreatedAt { get; set; }

    }
}