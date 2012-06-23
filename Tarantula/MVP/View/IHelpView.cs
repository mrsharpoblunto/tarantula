using System;
using System.Collections.Generic;
using System.Text;

namespace Tarantula.MVP.View
{
    public interface IHelpView: IView
    {
        event EventHandler OnOk;
    }
}
