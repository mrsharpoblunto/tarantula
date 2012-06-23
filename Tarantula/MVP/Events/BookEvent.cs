using System;
using System.Collections.Generic;
using System.Text;

namespace Tarantula.MVP.Events
{
    public delegate void BookEventHandler(object sender, BookEvent eventArgs);

    public class BookEvent
    {
        private string _itemID;

        public BookEvent(string itemID)
        {
            _itemID = itemID;
        }

        public string ItemID
        {
            get { return _itemID; }
            set { _itemID = value; }
        }
    }
}
