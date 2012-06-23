using System;
using System.Collections.Generic;
using System.Text;
using Tarantula.MVP.Model;

namespace Tarantula.MVP.Events
{
    public delegate void GestureEventHandler(object sender, GestureEvent gestureEvent);

    public class GestureEvent
    {
        private IGesture _gesture;

        public GestureEvent(IGesture gesture)
        {
            _gesture = gesture;
        }

        public IGesture Gesture
        {
            get { return _gesture; }
            set { _gesture = value; }
        }
    }
}
