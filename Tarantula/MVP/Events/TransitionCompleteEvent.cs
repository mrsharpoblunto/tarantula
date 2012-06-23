using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Tarantula.MVP.Events
{
    public delegate void TransitionCompleteHandler(object sender, TransitionCompleteEvent args);

    public class TransitionCompleteEvent
    {
        private UserControl _oldPage;
        private UserControl _newPage;

        public UserControl OldPage
        {
            get { return _oldPage; }
            set { _oldPage = value; }
        }

        public UserControl NewPage
        {
            get { return _newPage; }
            set { _newPage = value; }
        }
    }
}
