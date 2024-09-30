using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

public class OpportunityModel
{
    [Required]
    [Key]
    public string Title { get; set; }

    public string? Description { get; set; }
    public string? Province { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime Date { get; set; }
    public string? District { get; set; }
    public string? City { get; set; }
    public string? UserEmail { get; set; }
    public string? Category { get; set; }
    public string? Updated { get; set; }
}

