﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using X.Scheduler.Core.Entitites;

namespace X.Scheduler.Persistence.Maps
{
    public class StaffMap
    {
        public StaffMap(EntityTypeBuilder<Staff> entityBuilder)
        {
            entityBuilder.HasKey(t => t.Id);
            entityBuilder.Property(t => t.FirstName).IsRequired().HasMaxLength(100);
            entityBuilder.Property(t => t.LastName).IsRequired().HasMaxLength(100);
            entityBuilder.Property(t => t.Title).HasMaxLength(100);
        }
    }
}
