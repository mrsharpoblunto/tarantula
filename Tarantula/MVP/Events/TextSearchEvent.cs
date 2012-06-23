using System;
using System.Collections.Generic;
using System.Text;

namespace Tarantula.MVP.Events
{
    public delegate void TextSearchEventHandler(object sender, TextSearchEvent eventArgs);

    public class TextSearchEvent
    {
        private string _searchText;

        public TextSearchEvent(string searchText)
        {
            _searchText = searchText;
        }

        public string SearchText
        {
            get { return _searchText; }
            set { _searchText = value; }
        }
    }
}
