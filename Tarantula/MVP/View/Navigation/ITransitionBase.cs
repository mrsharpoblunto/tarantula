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
    public interface ITransitionBase
    {
        event TransitionCompleteHandler OnComplete;

        void PerformTransition(UserControl newPage, UserControl oldPage);
        void InitNewPage(UserControl page);
    }
}
