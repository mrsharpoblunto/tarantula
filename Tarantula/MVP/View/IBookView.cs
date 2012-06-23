using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Tarantula.MVP.Events;

namespace Tarantula.MVP.View
{
    public interface IBookView: IView
    {
        event MouseEventHandler StartDrag;
        event EventHandler EndDrag;
        event BookEventHandler ShowBookDetails;
        event BookEventHandler DeleteBook;
        event BookEventHandler DeleteConnectedBooks;
        event BookEventHandler SpawnSimilarBooks;
        event EventHandler Dispose;

        void DisposeControl();

        string ItemID { get; set; }
        string ImageURL {get; set;}
        string Title {get; set; }

        double Left { get; set; }
        double Top { get; set; }
        double ActualWidth { get; }
        double ActualHeight { get; }
    }
}
