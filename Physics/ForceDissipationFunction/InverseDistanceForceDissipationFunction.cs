using System;
using System.Collections.Generic;
using System.Text;

namespace Physics.ForceDissipationFunction
{
    internal class InverseDistanceDissipationFunction : IForceDissipationFunction
    {
        private double _scaleFactor;

        /// <summary>
        /// creates an inverse distance dissipation function, this causes the force applied to decay asymptotically toward 0
        /// </summary>
        /// <param name="scaleFactor">Applies a linear scaling effect to the inverse distance decay rate (must NOT be 0)</param>
        public InverseDistanceDissipationFunction(double scaleFactor)
        {
            this._scaleFactor = scaleFactor;
        }

        public double GetForceAtDistance(double distance)
        {
            if ((distance * _scaleFactor) < 1)
            {
                return 1;
            }
            else
            {
                return 1 / (distance * _scaleFactor);
            }
        }
    }
}
