using System;
using System.Collections.Generic;
using System.Text;

namespace Physics.ForceDissipationFunction
{
    internal class LinearForceDissipationFunction: IForceDissipationFunction {
        private double _distanceOfNoEffect;

        /// <summary>
        /// creates a linear dissipation function, this causes forces to decay at a linear rate toward 0
        /// </summary>
        /// <param name="distanceOfNoEffect">the distance at which no force will be applied (must NOT be 0)</param>
        public LinearForceDissipationFunction(double distanceOfNoEffect)
        {
            this._distanceOfNoEffect = distanceOfNoEffect;
        }

        public double GetForceAtDistance(double distance)
        {
            if (distance > _distanceOfNoEffect)
            {
                return 0;
            }
            else
            {
                return (_distanceOfNoEffect - distance) / _distanceOfNoEffect;
            }
        }
    }
}
