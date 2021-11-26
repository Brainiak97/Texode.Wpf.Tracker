using System.Windows;
using System.Windows.Controls;
using Texode.Wpf.Tracker.Models;

namespace Texode.Wpf.Tracker.Infrastructure
{
    public class CategoryHighlightStyleSelector : StyleSelector
    {
        public Style Default { get; set; }
        public Style BestCheck { get; set; }
        public Style WorstCheck { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            User user = (User)item;

            if (user.AverageSteps > 0 && user.WorstResult > 0)
            {
                if ((user.BestRezult - user.AverageSteps) / user.AverageSteps * 100 >= 20)
                {
                    return BestCheck;
                }
                else if ((user.AverageSteps - user.WorstResult) / user.WorstResult * 100 >= 20)
                {
                    return WorstCheck;
                }
                else
                {
                    return Default;
                }
            }
            else
            {
                return Default;
            }
        }
    }
}
