namespace X.Scheduler.Core.Abstracts
{
    public interface IScheduleManager
    {
        void GenerateNewSchedule();
        void Initialize();
    }
}