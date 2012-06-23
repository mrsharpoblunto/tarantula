using System;
using System.Collections.Generic;
using System.Text;

namespace Tarantula.MVP.View
{
    public interface ILoadingView: IView
    {
        void ShowLoading();
        void HideLoading();
    }
}
