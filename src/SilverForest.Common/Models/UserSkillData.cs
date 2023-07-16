using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverForest.Common.Models;

public class UserSkillData
{
    [Key]
    public int Id { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }

    //Navigation Properties
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int SkillId { get; set; }
    public Skill Skill { get; set; } = null!;
}
