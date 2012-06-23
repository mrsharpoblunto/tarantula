using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using Tarantula.MVP.View;

namespace Tarantula.MVP.Presenter
{
    public class PresenterBase<T> where T: IView
    {
        private T _view;

        public PresenterBase(T view) 
        {
            _view = view;
        }

        protected T View
        {
            get { return _view; }
            set { _view = value; }
        }

    }
}
