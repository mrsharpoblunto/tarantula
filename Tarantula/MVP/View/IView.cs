using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Tarantula.MVP.View
{
    public interface IView
    {
        Dispatcher Dispatcher { get; }
    }
}
