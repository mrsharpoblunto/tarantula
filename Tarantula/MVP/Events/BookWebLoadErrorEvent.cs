using System;
using System.Collections.Generic;
using System.Text;

namespace Tarantula.MVP.Events
{
    public delegate void BookWebNotificationHandler(object sender, BookWebNotificationEvent eventArgs);

    public class BookWebNotificationEvent
    {
        private string _notification;

        public BookWebNotificationEvent(string notification)
        {
            _notification = notification;
        }

        public string Notification
        {
            get { return _notification; }
            set { _notification = value; }
        }
    }
}
