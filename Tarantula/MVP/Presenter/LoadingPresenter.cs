using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using Tarantula.MVP.View;

namespace Tarantula.MVP.Presenter
{
    public class LoadingPresenter : PresenterBase<ILoadingView>
    {
        public LoadingPresenter(ILoadingView view)
            : base(view)
        {
        }

        public void ShowLoading()
        {
            View.ShowLoading();
        }

        public void HideLoading()
        {
            View.HideLoading();
        }

    }
}
