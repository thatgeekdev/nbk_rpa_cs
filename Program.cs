using NBK_RPA_CS.Config;
using NBK_RPA_CS.Services;
using System;
// using NBK_RPA_CS.Config;


namespace NBK_RPA_CS
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigService();
            var logger = new LoggerService(config.LogsDir);
            var export = new ExportService(config.ExportsPath);

            logger.Info("🚀 Iniciando automação RPA...");

            var bot = new BotService(config, logger, export);
            bot.Run();

            logger.Info("✅ Automação concluída!");
        }
    }
}
