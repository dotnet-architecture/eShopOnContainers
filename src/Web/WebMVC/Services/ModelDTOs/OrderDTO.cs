﻿namespace WebMVC.Services.ModelDTOs;

public record OrderDTO
{
    [Required]
    public string OrderNumber { get; init; }
}
