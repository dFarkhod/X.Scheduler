using Microsoft.EntityFrameworkCore.Metadata.Builders;
using X.Scheduler.Entities;

namespace X.Scheduler
{
    public class StaffMap
    {
        public StaffMap(EntityTypeBuilder<Staff> entityBuilder)
        {
            entityBuilder.HasKey(t => t.Guid);
            entityBuilder.Property(t => t.FirstName).IsRequired();
            entityBuilder.Property(t => t.LastName).IsRequired();
            entityBuilder.Property(t => t.Email).IsRequired();
        }
    }
}
