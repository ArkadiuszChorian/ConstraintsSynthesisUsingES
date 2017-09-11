using System.Collections.Generic;
using System.Linq;
using CSUES.Engine.Models;

namespace CSUES.WinApplication
{
    public static class Extensions
    {
        public static IList<Point> Reduce(this IList<Point> points, int maximumNumberOfPoints = 10000)
        {
            if (points.Count > maximumNumberOfPoints)
                points = points.Take(maximumNumberOfPoints).ToList();

            return points;
        }
    }
}