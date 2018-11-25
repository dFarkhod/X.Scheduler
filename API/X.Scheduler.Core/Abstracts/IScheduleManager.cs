namespace X.Scheduler.Core.Abstracts
{
    public interface IScheduleManager
    {
        void Initialize();
        void GenerateNewSchedule();
    }
}