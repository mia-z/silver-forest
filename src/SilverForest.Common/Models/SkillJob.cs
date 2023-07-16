using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverForest.Common.Models;

public class SkillJob
{
    public Guid Guid { get; set; }
    public int PlayerId { get; set; }
    public Skill Skill { get; set; }
    public TimeSpan ExpireTime { get; set; }

}
