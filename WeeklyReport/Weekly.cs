using AssemblyAttributes;
using System;

[assembly: EDW_Challenge]
namespace WeeklyReport
{
    public class Weekly : IEDWChallenge
    {
        public Guid SessionId { get; set; }
        public Weekly()
        {
            SessionId = Guid.NewGuid();
            Console.WriteLine("Weekly inestance created with id: " + SessionId.ToString());
        }
        public void Report()
        {
            Console.WriteLine("Weekly report called");
        }
    }
}
