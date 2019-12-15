using System;
using System.ComponentModel.DataAnnotations;

namespace WebMVC.Services.ModelDTOs
{
    public class OrderDTO
    {
        [Required]
        public string OrderNumber { get; set; }
    }
}