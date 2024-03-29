﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using X.Scheduler.Domain.Entities;

namespace X.Scheduler.Infrastructure.Persistence.Maps
{
    public class ScheduleHistoryMap
    {
        //private ReferenceCollectionBuilder<Staff, ScheduleHistory> referenceCollectionBuilder;

        public ScheduleHistoryMap(EntityTypeBuilder<ScheduleHistory> entityBuilder)
        {
            entityBuilder.HasKey(t => t.Id);
            entityBuilder.Property(t => t.StaffId).IsRequired();
            entityBuilder.Property(t => t.Date).IsRequired();
            entityBuilder.Property(t => t.Shift).IsRequired();
            entityBuilder.Property(t => t.PreviousId).IsRequired();
        }
    }
}
