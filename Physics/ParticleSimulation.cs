using System;
using System.Collections.Generic;
using System.Configuration;

namespace Physics
{
    public class ParticleSimulation<PM,CM>
    {
        /// <summary>
        /// an event that allows metadata for all particles in this simulation to be disposed when the particle is removed
        /// </summary>
        public event MetaDataDisposeHandler<PM> DisposeParticleMetaData;

        private double _gravity;
        private double _frictionConstant;
        private double _staticFriction;
        private double _connectionLength, _connectionSpringConstant;
        private double _width, _height,_top,_left;
        private ForceDissipationDelegate _forceDissipationFunction;
        private List<Particle<PM,CM>> _particles;

        public ParticleSimulation(double top,double left,double width,double height,double gravity,double frictionConstant,double staticFriction,double connectionLength,double connectionSpringConstant,ForceDissipationDelegate forceDissipationFunction)
        {
            _top = top;
            _left = left;
            _width = width;
            _height = height;
            _gravity = gravity;
            _frictionConstant = frictionConstant;
            StaticFriction = staticFriction;
            _connectionLength = connectionLength;
            _connectionSpringConstant = connectionSpringConstant;
            _forceDissipationFunction = forceDissipationFunction;
            _particles = new List<Particle<PM, CM>>();
        }

        public void AddParticle(Particle<PM, CM> p, PM metaData)
        {
            p.MetaData = metaData;
            _particles.Add(p);
        }

        /// <summary>
        /// adds a particle to the simulation and connects it with another particle
        /// </summary>
        /// <param name="p">the particle to add</param>
        /// <param name="p1">the particle to connect it to</param>
        public void AddParticleToParticle(Particle<PM, CM> p, PM m, Particle<PM, CM> p1, CM cm)
        {
            AddParticle(p,m);
            p1.AddConnection(p, cm, _connectionLength, _connectionSpringConstant);
        }

        public void RemoveParticle(Particle<PM, CM> p)
        {
            p.DisconnectParticle();
            if (DisposeParticleMetaData != null)
            {
                DisposeParticleMetaData(this, new MetaDataDisposeEventArgs<PM>(p.MetaData));
            }
            _particles.Remove(p);

        }

        public void RunSimulation(double timeStep)
        {
            AccumulateForces();
            Verlet(timeStep);
            SatisfyConstraints();
        }

        /// <summary>
        /// work out the new positions of all the particles based on the forces acting on the particles
        /// </summary>
        /// <param name="step"></param>
        private void Verlet(double step)
        {
            foreach (Particle<PM, CM> p in _particles)
            {
                Vector temp = p.Position.Clone();

                //get the force acting in the particle
                Vector acceleration = p.AccumulatedForces.Clone();

                //if the force is greater than the static friciton threshold then convert the
                //force into an acceleration
                if (Vector.Length(acceleration) >= StaticFriction)
                {
                    Vector.MultiplyLength(ref acceleration, step * step * p.InvMass);

                    //model the effects of friction
                    Vector newPos = p.Position - p.PreviousPosition;
                    Vector.MultiplyLength(ref newPos, _frictionConstant);

                    //update the positions
                    p.Position += newPos + acceleration;
                    p.PreviousPosition = temp.Clone();
                }
                else
                {
                    p.PreviousPosition = p.Position.Clone();
                }
            }
        }

        private void SatisfyConstraints()
        {
            foreach (Particle<PM, CM> p in _particles)
            {
                p.Position.X = Math.Min(Math.Max(_left, p.Position.X), _width+_left);
                p.Position.Y = Math.Min(Math.Max(_top, p.Position.Y), _height+_top);
            }
        }

        /// <summary>
        /// calculate the total force acting on each particle at the current instant
        /// </summary>
        private void AccumulateForces()
        {
            foreach (Particle<PM, CM> p in _particles)
            {
                p.AccumulatedForces = new Vector(0,Gravity);
                p.AccumulatedForces += p.AccumulateConnectorForces();

                foreach (Particle<PM, CM> op in _particles)
                {
                    if (op!=p)
                    {
                        p.AccumulatedForces += op.RestingForceAtPosition(p.Position);
                    }
                }
            }
        }

        public ForceDissipationDelegate ForceDissipationFunction
        {
            get { return _forceDissipationFunction; }
        }

        public List<Particle<PM, CM>> Particles
        {
            get { return _particles;  }
        }

        public double FrictionConstant
        {
            get { return _frictionConstant; }
            set { _frictionConstant = value; }
        }

        public double Gravity
        {
            get { return _gravity; }
            set { _gravity = value; }
        }

        public double StaticFriction
        {
            get { return _staticFriction; }
            set { _staticFriction = value; }
        }

        public double Top
        {
            get { return _top; }
            set { _top = value; }
        }

        public double Left
        {
            get { return _left; }
            set { _left = value; }
        }

        public double Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public double Height
        {
            get { return _height; }
            set { _height = value; }
        }
    }
}
