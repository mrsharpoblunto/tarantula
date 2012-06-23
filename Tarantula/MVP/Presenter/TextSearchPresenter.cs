using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using Tarantula.MVP.View;
using Tarantula.MVP.Events;
using System.Windows.Input;
using Tarantula.MVP.View.Impl;
using Tarantula.MVP.View.Navigation;

namespace Tarantula.MVP.Presenter
{
    public class TextSearchPresenter : PresenterBase<ITextSearchView> 
    {
        public event TextSearchEventHandler StartSearch;
        private bool _isVisible;

        public TextSearchPresenter(ITextSearchView view)
            : base(view)
        {
            HookEvents(view);
            _isVisible = false;
        }

        private void HookEvents(ITextSearchView view)
        {
            view.StartSearch += new TextSearchEventHandler(OnViewStartSearch);
            view.CancelSearch += new EventHandler(view_CancelSearch);
        }

        public void Show()
        {
            _isVisible = true;
            NavigationHelper.PushPage(View);
        }

        private void OnViewStartSearch(object sender, TextSearchEvent eventArgs)
        {
            Hide();
            StartSearch(this, eventArgs);
        }

        void  view_CancelSearch(object sender, EventArgs e)
        {
            Hide();
        }

        private void Hide()
        {
            if (_isVisible)
            {
                NavigationHelper.PopPage();
                View = new TextSearchControl();
                HookEvents(View);
                _isVisible = false;
            }
        }
    }
}
