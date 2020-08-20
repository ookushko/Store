using Store_MVC.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Store_MVC.Models.ViewModels.Shop
{
    public class CategoryVM
    {
        public CategoryVM()
        {

        }

        public CategoryVM(CategoryDTO row)
        {
            Id = row.Id;
            Name = row.Name;
            ShortDesc = row.ShortDesc;
            Sorting = row.Sorting;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortDesc { get; set; }
        public int Sorting { get; set; }
    }
}