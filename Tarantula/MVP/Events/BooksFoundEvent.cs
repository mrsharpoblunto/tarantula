using System;
using System.Collections.Generic;
using System.Text;
using Tarantula.MVP.Model;

namespace Tarantula.MVP.Events
{
    public delegate void BooksFoundEventHandler(string parentItemID, BooksFoundEvent eventArgs);

    public class BooksFoundEvent
    {
        private List<Book> _results;
        private bool _success;
        private string _failureMessage;

        public string FailureMessage
        {
            get { return _failureMessage; }
            set { _failureMessage = value; }
        }

        public bool Success
        {
            get { return _success; }
            set { _success = value; }
        }

        public BooksFoundEvent()
        {
        }

        public List<Book> Results
        {
            get { return _results; }
            set { _results = value; }
        }
    }
}
