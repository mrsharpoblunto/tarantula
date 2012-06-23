using System;
using System.Collections.Generic;
using System.Text;
using Tarantula.MVP.Model.Gestures;

namespace Tarantula.MVP.Model
{
    public class GestureFactory
    {
        public static IGesture CreateShakeGesture()
        {
            return new ShakeGesture();
        }
    }
}
