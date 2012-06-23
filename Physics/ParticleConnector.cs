using System;
using System.Collections.Generic;
using System.Text;

namespace Physics
{
    public class ParticleConnector<PM,CM>
    {
        private Particle<PM,CM> particle1;
        private Particle<PM,CM> particle2;
        private double springConstant;
        private double restingLength;
        private CM _metaData;

        public ParticleConnector(Particle<PM, CM> particle1, Particle<PM, CM> particle2,double restingLength, double springConstant)
        {
            Particle1 = particle1;
            Particle2 = particle2;
            RestingLength = restingLength;
            this.springConstant = springConstant;
        }

        public CM MetaData
        {
            get { return _metaData; }
            set { _metaData = value; }
        }

        public Particle<PM, CM> Particle1
        {
            get { return particle1; }
            set { particle1 = value; }
        }

        public Particle<PM, CM> Particle2
        {
            get { return particle2; }
            set { particle2 = value; }
        }

        public double SpringConstant
        {
            get { return springConstant; }
            set { springConstant = value; }
        }

        public double RestingLength
        {
            get { return restingLength; }
            set { restingLength = value; }
        }

        /// <summary>
        /// calculate the force applied by the spring to one of its connected particles at a point in time
        /// </summary>
        /// <returns>the force applied by the spring to one of its connected particles at a point in time</returns>
        public Vector ForceOnParticle1()
        {
            Vector v = particle1.Position - particle2.Position;
            double currentLength = Vector.Length(v);

            if (v.X != 0 || v.Y != 0)
            {
                Vector.Normalize(ref v);
            }
            else
            {
                Random r = new Random(DateTime.Now.Millisecond);
                double value = r.NextDouble();
                v = new Vector(value, 1 - value);
            }

            Vector.MultiplyLength(ref v,(currentLength - restingLength) * springConstant * -1);
            return v;
        }

        /// <summary>
        /// calculate the force applied by the spring to one of its connected particles at a point in time
        /// </summary>
        /// <returns>the force applied by the spring to one of its connected particles at a point in time</returns>
        public Vector ForceOnParticle2()
        {
            Vector v = ForceOnParticle1();
            Vector.MultiplyLength(ref v,-1);
            return v;
        }
    }
}
