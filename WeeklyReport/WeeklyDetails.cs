using AssemblyAttributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace WeeklyReport
{
    public class WeeklyDetails : IEDWChallenge
    {
        public Guid SessionId { get; set; }
        private readonly ILogger<dynamic> _logger;
        private readonly IConfigurationSection _helper;
        private readonly ILogger _loggerFactory;
        public WeeklyDetails(ILogger<dynamic> ilogger,ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _logger = ilogger;
            _loggerFactory = loggerFactory.CreateLogger("WeeklyReport.WeeklyDetails");
            _helper = configuration.GetSection("Helper");
            SessionId = Guid.NewGuid();
            Console.WriteLine("WeeklyDetails inestance created with id: " + SessionId.ToString());
            var methods = _helper.GetSection("Methods").GetChildren().ToList();
            Console.Write("WeeklyDetails inestance will execute :");
            foreach (var method in methods)
            {
                Console.WriteLine("\""+method.Value+"()\"");
            }
            _logger.LogInformation("WeeklyDetails inestance created with id: " + SessionId.ToString());
        }
        public void Report()
        {
            Console.WriteLine("WeeklyDetails report called");
        }
    }
}
