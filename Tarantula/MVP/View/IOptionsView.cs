using System;
using System.Collections.Generic;
using System.Text;

namespace Tarantula.MVP.View
{
    public interface IOptionsView: IView
    {
        event EventHandler TextBookSearch;
        event EventHandler ClearBookWeb;
        event EventHandler ShowHelp;
    }
}
