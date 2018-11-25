using Microsoft.Extensions.Configuration;
using System.IO;
using X.Scheduler.Core.Abstracts;

namespace X.Scheduler.Managers
{
    public class ConfigurationManager : BaseManager, IConfigurationManager
    {
        public static IConfiguration AppSetting { get; private set; }
        static ConfigurationManager()
        {

        }
        public override void Initialize()
        {
            AppSetting = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();
        }
    }
}
