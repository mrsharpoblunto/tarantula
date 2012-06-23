using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Tarantula.MVP.Events;

namespace Tarantula.MVP.View.Navigation
{
    public class PopFadeTransition: ITransitionBase
    {
        public event TransitionCompleteHandler OnComplete;

        private UserControl _newPage;
        private UserControl _oldPage;

        public void PerformTransition(UserControl newPage, UserControl oldPage)
        {
            _newPage = newPage;
            _oldPage = oldPage;

            //fade the old page out
            Duration duration = new Duration(TimeSpan.FromSeconds(Constants.PAGE_TRANSITION_DURATION));
            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = duration;
            //animation.To = 0.0;

            Storyboard sb = new Storyboard();
            sb.Duration = duration;
            sb.Children.Add(animation);
            sb.Completed+=new EventHandler(sb_Completed);

            Storyboard.SetTarget(animation, oldPage);
            Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));
            oldPage.Resources.Add("Pop_Fade_"+Guid.NewGuid().ToString(),sb);
            sb.Begin();
        }

        public void InitNewPage(UserControl page)
        {
        }

        void sb_Completed(object sender, EventArgs e)
        {
            if (OnComplete != null)
            {
                TransitionCompleteEvent args = new TransitionCompleteEvent();
                args.NewPage = _newPage;
                args.OldPage = _oldPage;
                OnComplete(_newPage, args);
            }
        }
    }
}
