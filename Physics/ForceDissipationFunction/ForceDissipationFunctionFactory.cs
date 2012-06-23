using System;
using System.Collections.Generic;
using System.Text;

namespace Physics.ForceDissipationFunction
{
    public class ForceDissipationFunctionFactory
    {

        /// <summary>
        /// creates an inverse distance dissipation function, this causes the force applied to decay asymptotically toward 0
        /// </summary>
        /// <param name="scaleFactor">Applies a linear scaling effect to the inverse distance decay rate (must NOT be 0)</param>
        public static ForceDissipationDelegate InverseDistanceDissipationFunction(double scaleFactor)
        {
            return CreateDelegateFromInstance(new InverseDistanceDissipationFunction(scaleFactor));
        }

        /// <summary>
        /// creates a linear dissipation frunction, this causes forces to decay at a linear rate toward 0
        /// </summary>
        /// <param name="distanceOfNoEffect">the distance at which no force will be applied (must NOT be 0)</param>

        public static ForceDissipationDelegate LinearDissipationFunction(double distanceOfNoEffect)
        {
            return CreateDelegateFromInstance(new LinearForceDissipationFunction(distanceOfNoEffect));
        }

        /// <summary>
        /// creates a force dissipation delegate using the force dissipation class provided
        /// </summary>
        /// <param name="iidf">the force dissipation class provided</param>
        /// <returns>a force dissipation delegate using the force dissipation class provided</returns>
        private static ForceDissipationDelegate CreateDelegateFromInstance(IForceDissipationFunction iidf)
        {
            return new ForceDissipationDelegate(iidf.GetForceAtDistance);            
        }
    }
}
