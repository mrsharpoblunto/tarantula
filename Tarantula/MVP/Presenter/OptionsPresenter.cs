using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using Tarantula.MVP.View;

namespace Tarantula.MVP.Presenter
{
    public class OptionsPresenter : PresenterBase<IOptionsView>
    {
        public event EventHandler TextBookSearch;
        public event EventHandler ClearBookWeb;
        public event EventHandler Help;

        public OptionsPresenter(IOptionsView view)
            : base(view)
        {
            view.ClearBookWeb += new EventHandler(OnViewClearBookWeb);
            view.TextBookSearch += new EventHandler(OnViewTextBookSearch);
            view.ShowHelp += new EventHandler(OnViewHelp);
        }

        private void OnViewTextBookSearch(object sender, EventArgs e)
        {
            TextBookSearch(this, new EventArgs());
        }

        private void OnViewClearBookWeb(object sender, EventArgs e)
        {
            ClearBookWeb(this, new EventArgs());
        }

        private void OnViewHelp(object sender, EventArgs e)
        {
            Help(this, new EventArgs());
        }
    }
}
