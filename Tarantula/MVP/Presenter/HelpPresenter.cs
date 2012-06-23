using System;
using System.Collections.Generic;
using System.Text;
using Tarantula.MVP.View;
using System.Windows.Controls;
using Tarantula.MVP.View.Impl;
using Tarantula.MVP.View.Navigation;

namespace Tarantula.MVP.Presenter
{
    public class HelpPresenter : PresenterBase<IHelpView>
    {
        public HelpPresenter(IHelpView view)
            : base(view)
        {
            view.OnOk += new EventHandler(view_OnOk);
        }

        void view_OnOk(object sender, EventArgs e)
        {
            NavigationHelper.PopPage();
        }

        public void Show()
        {
            NavigationHelper.PushPage(View);
        }
    }
}
