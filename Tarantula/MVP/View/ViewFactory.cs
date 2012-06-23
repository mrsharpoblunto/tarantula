using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Tarantula.MVP.View.Impl;

namespace Tarantula.MVP.View
{
    public class ViewFactory
    {
        public static IBookConnectorView CreateBookConnectorView()
        {
            return new BookConnectorControl();
        }

        public static IBookView CreateBookView()
        {
            return new BookControl();
        }
    }
}
