using AssemblyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: EDW_Challenge]
namespace MonthlyReport
{
    public class Monthly : IEDWChallenge
    {
        public Guid SessionId { get; set; }
        public Monthly()
        {
            SessionId = Guid.NewGuid();
            Console.WriteLine("Monthly inestance created with id: " + SessionId.ToString());
        }
        public void Report()
        {
            Console.WriteLine("Monthly report called");
        }
    }
}
