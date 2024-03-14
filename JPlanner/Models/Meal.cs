using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPlanner.Models
{
    public class Meal
    {
        public string Food { get; private set; }
        public int Calories { get; private set; }
        public DateTime TimeStamp { get; private set; }

        public Meal(string food, int calories, DateTime timeStamp)
        {
            Food = food;
            Calories = calories;
            TimeStamp = timeStamp;
        }

        public override string ToString()
        {
            return $"{Food}, {Calories} calories at {TimeStamp.ToString("f")}";
        }
    }
}
