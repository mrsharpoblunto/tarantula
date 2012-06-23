using System;
using System.Collections.Generic;
using System.Text;

namespace Physics.ForceDissipationFunction
{
    internal interface IForceDissipationFunction
    {
        /// <summary>
        /// returns the multiplaction factor for a force applied from a distance, this function should return a value
        /// between 0 and 1.
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        double GetForceAtDistance(double distance);
    }
}
