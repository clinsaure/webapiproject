using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiProject.Entities.DbSet;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public int Status { get; set; } = 1;

    public DateTime AddedDate { get; set; } = DateTime.UtcNow;

    public DateTime UpdateDate { get; set; }
}

