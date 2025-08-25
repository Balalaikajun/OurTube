using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace OurTube.Domain.Entities;

public class Tag : Base
{
    private const string Patern = @"^#[a-z0-9_]+$";

    public Tag()
    {
    }

    public Tag(string name)
    {
        SetName(name);
    }

    [MaxLength(50)] public string Name { get; private set; }

    private void SetName(string name)
    {
        name = name.ToLower().Trim();

        if (!Regex.IsMatch(name, Patern, RegexOptions.Compiled))
            throw new ArgumentException("Tag name is not valid");

        Name = name;
    }
}