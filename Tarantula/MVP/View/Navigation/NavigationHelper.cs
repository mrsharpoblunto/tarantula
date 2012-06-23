using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Tarantula.MVP.View.Navigation
{
    public class NavigationHelper
    {
        private static Grid _root;

        static NavigationHelper()
        {
            _root = Application.Current.RootVisual as Grid;
        } 

        public static void PushPage(IView page)
        {
            UserControl oldPage = _root.Children[_root.Children.Count-1] as UserControl;

            ITransitionBase transition = new PushFadeTransition();
            transition.InitNewPage(page as UserControl);
            _root.Children.Add(page as UserControl);
            transition.PerformTransition(page as UserControl, oldPage); 
        }


        public static void PopPage()
        {
            if (_root.Children.Count >= 2)
            {
                ITransitionBase transition = new PopFadeTransition();
                transition.InitNewPage(_root.Children[_root.Children.Count - 1] as UserControl);
                transition.OnComplete += new Tarantula.MVP.Events.TransitionCompleteHandler(transition_OnComplete);
                transition.PerformTransition(
                    _root.Children[_root.Children.Count - 2] as UserControl,
                    _root.Children[_root.Children.Count - 1] as UserControl);
            }
        }

        static void transition_OnComplete(object sender, Tarantula.MVP.Events.TransitionCompleteEvent args)
        {
            _root.Children.Remove(args.OldPage);
        }

        
    }
}
