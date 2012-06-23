using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Tarantula.MVP.Events;

namespace Tarantula.MVP.Model
{
    /// <summary>
    /// manages a set of registered gestures to see if any are recognized
    /// </summary>
    public class GestureManager
    {
        private static readonly int MAX_DRAGINFO_LIST_SIZE = 40;

        private static GestureManager _instance = new GestureManager();

        private Dictionary<IGesture,GestureEventHandler> _registeredGestures;
        private List<Point> _gestureHistory;

        public static GestureManager Instance
        {
            get
            {
                return _instance;
            }
        }

        private GestureManager() 
        {
            _registeredGestures = new Dictionary<IGesture, GestureEventHandler>();
            _gestureHistory = new List<Point>();
        }

        public void RecordGestureHistory(Point p)
        {
            _gestureHistory.Add(p);

            if (_gestureHistory.Count > MAX_DRAGINFO_LIST_SIZE)
            {
                _gestureHistory.RemoveAt(0);
            }
        }

        public void ProcessGestureHistory()
        {
            foreach (IGesture gesture in _registeredGestures.Keys)
            {
                if (gesture.MadeGesture(_gestureHistory))
                {
                    _registeredGestures[gesture].Invoke(this,new GestureEvent(gesture));
                }
            }
        }

        public void RegisterGesture(IGesture gesture,GestureEventHandler callback)
        {
            if (!_registeredGestures.ContainsKey(gesture))
            {
                _registeredGestures.Add(gesture,callback);
            }
        }




    }
}
