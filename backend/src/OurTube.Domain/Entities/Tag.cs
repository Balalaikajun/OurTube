using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace OurTube.Domain.Entities;

public class Tag
{
    [Key]
    public int Id { get; set; } 
    [MaxLength(50)]
    public string Name { get; set; }
    
}