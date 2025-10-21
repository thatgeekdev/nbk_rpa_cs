using System;
using NBK_RPA_CS.Config;
using NBK_RPA_CS.Services;

namespace NBK_RPA_CS
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigService();
            var logger = new LoggerService(); // <- usa default ou _config.LogsPath
            var export = new ExportService(config.ExportsPath);

            logger.Info("RPA Automation starting.");

            var bot = new BotService(config, logger, export);
            bot.Run();

            logger.Info("RPA Automation finished.");
        }
    }
}
