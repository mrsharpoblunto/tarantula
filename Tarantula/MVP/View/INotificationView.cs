using System;
using System.Collections.Generic;
using System.Text;

namespace Tarantula.MVP.View
{
    public interface INotificationView: IView
    {
        void ShowNotification(string text);
    }
}
