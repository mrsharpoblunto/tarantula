using System;
using System.Collections.Generic;
using System.Text;
using Tarantula.MVP.Events;
using System.Windows.Input;

namespace Tarantula.MVP.View
{
    public interface ITextSearchView: IView
    {
        event TextSearchEventHandler StartSearch;
        event EventHandler CancelSearch;
    }
}
