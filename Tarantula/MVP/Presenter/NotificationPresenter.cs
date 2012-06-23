using System;
using System.Collections.Generic;
using System.Text;
using Tarantula.MVP.View;
using System.Windows.Controls;

namespace Tarantula.MVP.Presenter
{
    public class NotificationPresenter: PresenterBase<INotificationView>
    {
        public NotificationPresenter(INotificationView view)
            : base(view)
        {
        }

        public void ShowNotification(string text)
        {
            View.ShowNotification(text);
        }
    }
}
