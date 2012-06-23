using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Tarantula.MVP.View.Impl
{
    public partial class NotificationControl : UserControl, INotificationView
    {
        private readonly Storyboard _fadeIn;
        private readonly Storyboard _fadeOut;
        private readonly Storyboard _timer;
        private bool _mouseEntered, _fadingOut;

        public NotificationControl()
        {
            InitializeComponent();
            MouseEnter += new MouseEventHandler(NotificationControl_MouseEnter);
            MouseLeave += new MouseEventHandler(NotificationControl_MouseLeave);

            _fadeIn = (Storyboard)FindName("fadeIn");
            _fadeOut = (Storyboard)FindName("fadeOut");
            _timer = (Storyboard)FindName("timer");

            _mouseEntered = false;
            _fadingOut = false;
        }

        void NotificationControl_MouseLeave(object sender, MouseEventArgs e)
        {
            _mouseEntered = false;
        }

        void NotificationControl_MouseEnter(object sender, MouseEventArgs e)
        {
            _mouseEntered = true;

            if (_fadingOut)
            {
                _fadeOut.Pause();
                _fadeIn.Begin();
                _fadingOut = false;
            }
        }

        public void ShowNotification(string text)
        {
            _fadeOut.Pause();
            _timer.Pause();
            notificationText.Text = text;
            SetValue(Canvas.VisibilityProperty, Visibility.Visible);
            _fadeIn.Begin();
            _fadingOut = false;
        }

        private void FadeIn_Completed(object sender, EventArgs e)
        {
            _timer.Begin();
        }

        private void FadeOut_Completed(object sender, EventArgs e)
        {
            SetValue(VisibilityProperty, Visibility.Collapsed);
        }

        private void Timer_Completed(object sender, EventArgs e)
        {
            if (!_mouseEntered)
            {
                _fadeOut.Begin();
                _fadingOut = true;
            }
            else
            {
                _timer.Begin();
            }
        }
    }
}
