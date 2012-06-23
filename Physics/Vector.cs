using System;
using System.Collections.Generic;
using System.Text;

namespace Physics
{
    public class Vector
    {
        private double _x, _y;

        public Vector(double x, double y)
        {
            _x = x;
            _y = y;
        }

        public Vector Clone()
        {
            return new Vector(X, Y);
        }

        public double Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public double X
        {
            get { return _x; }
            set { _x = value; }
        }

        public static Vector operator+(Vector a, Vector b)
        {
            return new Vector(a._x + b._x, a._y + b._y);
        }

        public static Vector operator-(Vector a, Vector b)
        {
            return new Vector(a._x - b._x, a._y - b._y);
        }

        public static double Length(Vector a)
        {
            return Math.Sqrt(Math.Pow(a.X, 2) + Math.Pow(a.Y, 2));
        }

        public static void Normalize(ref Vector a) 
        {
            double invLength = 1/Length(a);
            a.X *= invLength;
            a.Y *= invLength;
        }

        public static void MultiplyLength(ref Vector a,double multiple)
        {
            a.X *= multiple;
            a.Y *= multiple;
        }




    }
}
