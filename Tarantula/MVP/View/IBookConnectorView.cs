using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Tarantula.MVP.View
{
    public interface IBookConnectorView: IView
    {
        event EventHandler Dispose;

        void DisposeControl();

        Point[] Points { set; }
    }
}
