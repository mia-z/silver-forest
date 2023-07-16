using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverForest.Common.Models;

public record User
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string Email { get; set; } = string.Empty;

    //Navigation Properties
    public ICollection<UserSkillData> Skills { get; set; } = new List<UserSkillData>();
}
