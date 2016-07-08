using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nameday
{
    public class NamedayModel
    {
        public int Month { get; }
        public int Day { get; }
        public IEnumerable<string> Names { get; }

        public NamedayModel(int month, int day, IEnumerable<string> names)
        {
            Month = month;
            Day = day;
            Names = names;
        }

        public string NamesAsString => string.Join(", ", Names);
 
    }
}
