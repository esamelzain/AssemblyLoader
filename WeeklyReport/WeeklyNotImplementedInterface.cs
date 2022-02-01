using AssemblyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeeklyReport
{
    /// <summary>
    /// will never mapped
    /// </summary>
    public class WeeklyNotImplementedInterface
    {
        public Guid SessionId { get; set; }
        public WeeklyNotImplementedInterface()
        {
            SessionId = Guid.NewGuid();
            Console.WriteLine("WeeklyNotImplementedInterface inestance created with id: " + SessionId.ToString());
        }
        public void Report()
        {
            Console.WriteLine("WeeklyNotImplementedInterface report called");
        }
    }
}
