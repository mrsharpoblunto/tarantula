using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Tarantula.MVP.Model.Gestures
{
    /// <summary>
    /// responds to rapid horizontal shaking gestures
    /// </summary>
    internal class ShakeGesture: IGesture
    {
        private static int DISCONNECTION_THRESHOLD = 2;
        private static double MIN_SHAKE_THRESHOLD = 5;

        public bool MadeGesture(List<Point> points)
        {
            if (points != null && points.Count > 1)
            {
                int xreversals = 0;
                double oldXDir = 0;
                double xdir = 0;

                for (int i = 1; i < points.Count - 1; ++i)
                {
                    xdir = points[i].X - points[i - 1].X;

                    if (Math.Sign(xdir) != Math.Sign(oldXDir) && (xdir > MIN_SHAKE_THRESHOLD))
                    {
                        ++xreversals;
                    }

                    oldXDir = xdir;
                }

                if (xreversals > DISCONNECTION_THRESHOLD)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
