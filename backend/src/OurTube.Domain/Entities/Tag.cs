using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace OurTube.Domain.Entities;

public class Tag
{
    private const string _patern = @"^#[a-z0-9_]+$";
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; private set; } 
    [MaxLength(50)]
    public string Name { get; private set; }

    public Tag()
    {
        
    }
    public Tag(string name)
    {
        SetName(name);
    }

    private void SetName(string name)
    {
        name = name.ToLower().Trim();
        
        if(!Regex.IsMatch(name, _patern, RegexOptions.Compiled))
            throw new ArgumentException("Tag name is not valid");
        
        Name = name;
    }
    
}