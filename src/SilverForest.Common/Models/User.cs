using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverForest.Common.Models;

public record User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}
