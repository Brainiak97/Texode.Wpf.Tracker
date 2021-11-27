using System.Collections.Generic;
using System.Linq;
using Texode.Wpf.Tracker.Models;

namespace Texode.Wpf.Tracker.Infrastructure
{
    public static class Steps
    {
        public static int GetAverageSteps(User user, List<List<DayResult>> dayResults)
        {
            int averageSteps = 0;

            foreach (var day in dayResults)
            {
                if (day.FirstOrDefault(d => d.User == user.Name) is not null)
                {
                    averageSteps += day.FirstOrDefault(d => d.User == user.Name).Steps;
                }
            }

            return averageSteps / dayResults.Count;
        }

        public static int GetBestRezult(User user, List<List<DayResult>> dayResults)
        {
            int max = 0;

            foreach (var day in dayResults)
            {
                if (day.FirstOrDefault(d => d.User == user.Name) is not null && day.FirstOrDefault(d => d.User == user.Name).Steps > max)
                {
                    max = day.FirstOrDefault(d => d.User == user.Name).Steps;
                }
            }

            return max;
        }

        public static int GetWorstRezult(User user, List<List<DayResult>> dayResults)
        {
            int min = user.BestRezult;

            foreach (var day in dayResults)
            {
                if (day.FirstOrDefault(d => d.User == user.Name) is not null && day.FirstOrDefault(d => d.User == user.Name).Steps < min)
                {
                    min = day.FirstOrDefault(d => d.User == user.Name).Steps;
                }
            }

            return min;
        }
    }
}
