namespace X.Scheduler.Domain.Entities.Interfaces
{
    public interface IScheduleManager
    {
        void Initialize();
        void GenerateNewSchedule();
    }
}