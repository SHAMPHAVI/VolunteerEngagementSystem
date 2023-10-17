using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

public class OpportunityModel
{
    [Required]
    [Key]
    public string Title { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public string Location { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime Date { get; set; }
    public string? UserEmail { get; set; }
    public string Category { get; set; }
}

