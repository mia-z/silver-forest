﻿using SilverForest.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverForest.Infrastructure.Redis.Abstraction;

public interface IRedisJobCache
{
    Task<bool> AddJob(string name, int id);
    Task<IEnumerable<SkillJob>> GetJobsById(int id);
}
