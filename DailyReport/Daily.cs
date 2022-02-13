using AssemblyAttributes;
using Microsoft.Extensions.Logging;
using System;
[assembly: EDW_Challenge]

namespace DailyReport
{
    public class Daily : IEDWChallenge
    {
        public Guid SessionId { get; set; }
        private readonly ILogger<Daily> _logger;
        public Daily(ILogger<Daily> ilogger)
        {
            _logger = ilogger;
            SessionId = Guid.NewGuid();
            Console.WriteLine("Daily inestance created with id: " + SessionId.ToString());
            _logger.LogInformation("Daily inestance created with id: " + SessionId.ToString());
        }
        public void Report()
        {
            Console.WriteLine("Daily report called");
            _logger.LogInformation("Daily report called");
        }
    }
}

