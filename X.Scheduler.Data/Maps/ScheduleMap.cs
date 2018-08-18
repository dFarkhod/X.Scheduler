﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using X.Scheduler.Data.Entitites;

namespace X.Scheduler.Data
{
    public class ScheduleMap
    {
        private ReferenceCollectionBuilder<Staff, Schedule> referenceCollectionBuilder;

        public ScheduleMap(EntityTypeBuilder<Schedule> entityBuilder)
        {
            entityBuilder.HasKey(t => t.Id);
            entityBuilder.Property(t => t.StaffId).IsRequired();
            entityBuilder.Property(t => t.Date).IsRequired();
            entityBuilder.Property(t => t.Shift).IsRequired();
            entityBuilder.Property(t => t.Staff).IsRequired();
        }

        public ScheduleMap(ReferenceCollectionBuilder<Staff, Schedule> referenceCollectionBuilder)
        {
            this.referenceCollectionBuilder = referenceCollectionBuilder;
        }
    }
}
