using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nameday
{
    public class NameDayModel
    {
        public int Day { get; set; }
        public int Month { get; set; }
        public IEnumerable<string> Names { get; set; }

        //constructor
        public NameDayModel(int month, int day, IEnumerable<string> names)
        {
            Month = month;
            Day = day;
            Names = names;
        }

        /* public string NameAsString
            {
                get { return string.Join(", ", Names); }
            } */

        public string NameAsString => string.Join(", ", Names); // => is get { return }

        public NameDayModel() { }
    }
}
