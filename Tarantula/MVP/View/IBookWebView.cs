using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Tarantula.MVP.View
{
    public interface IBookWebView: IView
    {
        event EventHandler Update;
        event MouseEventHandler MouseMove;
        event SizeChangedEventHandler SizeChanged;

        double ActualWidth { get; }
        double ActualHeight { get; }
        Canvas Canvas { get; }
    }
}
