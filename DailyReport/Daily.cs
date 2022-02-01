using AssemblyAttributes;
using System;
[assembly: EDW_Challenge]

namespace DailyReport
{
    public class Daily : IEDWChallenge
    {
        public Guid SessionId { get; set; }
        public Daily()
        {
            SessionId = Guid.NewGuid();
            Console.WriteLine("Daily inestance created with id: " + SessionId.ToString());
        }
        public void Report()
        {
            Console.WriteLine("Daily report called");
        }
    }
}

