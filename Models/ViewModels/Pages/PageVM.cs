using Store_MVC.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Store_MVC.Models.ViewModels.Pages
{
    public class PageVM // Page View Model
    {

        public PageVM()
        {

        }

        public PageVM(PagesDTO row)
        {
            Id = row.Id;
            Title = row.Title;
            ShortDesc = row.ShortDesc;
            Body = row.Body;
            Sorting = row.Sorting;
            HasSidebar = row.HasSidebar;
        }

        public int Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Title { get; set; }
        [Display(Name = "Short description")]
        public string ShortDesc { get; set; }
        [Required]
        [Display(Name = "Full description")]
        [StringLength(int.MaxValue, MinimumLength = 3)]
        public string Body { get; set; }
        public int Sorting { get; set; }
        [Display(Name = "Sidebar")]
        public bool HasSidebar { get; set; }
    }
}