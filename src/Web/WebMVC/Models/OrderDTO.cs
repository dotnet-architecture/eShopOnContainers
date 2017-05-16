using System;
using System.ComponentModel.DataAnnotations;

namespace WebMVC.Models
{
    public class OrderDTO
    {
        [Required]
        public string OrderNumber { get; set; }
    }
}