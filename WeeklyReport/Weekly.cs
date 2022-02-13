using AssemblyAttributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

[assembly: EDW_Challenge]
namespace WeeklyReport
{
    public class Weekly : IEDWChallenge
    {
        public Guid SessionId { get; set; }
        private readonly ILogger<dynamic> _logger;
        public Weekly(ILogger<dynamic> ilogger)
        {
            _logger = ilogger;
            //_configuration = configuration;
            SessionId = Guid.NewGuid();
            Console.WriteLine("Weekly inestance created with id: " + SessionId.ToString());
            _logger.LogInformation("Weekly inestance created with id: " + SessionId.ToString());
        }
        public void Report()
        {
            Console.WriteLine("Weekly report called");
            _logger.LogInformation("Weekly report called");
        }
    }
}
