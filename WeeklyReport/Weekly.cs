using AssemblyAttributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

[assembly: EDW_Challenge]
namespace WeeklyReport
{
    public class Weekly : IEDWChallenge
    {
        public Guid SessionId { get; set; }
        private readonly ILogger<dynamic> _logger;
        private readonly IConfigurationSection _helper;
        public Weekly(ILogger<dynamic> ilogger, IConfiguration configuration)
        {
            _logger = ilogger;
            _helper = configuration.GetSection("Helper");
            SessionId = Guid.NewGuid();
            Console.WriteLine("Weekly inestance created with id: " + SessionId.ToString());
            var methods = _helper.GetSection("Methods").GetChildren().ToList();
            Console.Write("Weekly inestance will execute :");
            foreach (var method in methods)
            {
                Console.WriteLine("\""+method.Value+"()\"");
            }

            _logger.LogInformation("Weekly inestance created with id: " + SessionId.ToString());

        }
        public void Report()
        {
            Console.WriteLine("Weekly report called");
            _logger.LogInformation("Weekly report called");
        }
    }
}
