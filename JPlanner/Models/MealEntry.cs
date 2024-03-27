using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPlanner.Models
{
    public class MealEntry
    {
        public string Entry { get; set; }
        public int Calories { get; set; }
        public DateTime TimeStamp { get; set; }

        public MealEntry() { }

        public MealEntry(string entry, int calories, DateTime timeStamp)
        {
            Entry = entry;
            Calories = calories;
            TimeStamp = timeStamp;
        }

        public override string ToString()
        {
            return $"{Entry}, {Calories} calories at {TimeStamp.ToString()}";
        }
    }
}
