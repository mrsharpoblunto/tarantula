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
    public partial class BookConnectorControl : UserControl, IBookConnectorView
    {
        public event EventHandler Dispose;

        private Storyboard _fadeOut;

        public BookConnectorControl()
        {
            InitializeComponent();
            SetValue(Canvas.ZIndexProperty, 0);
            _fadeOut = (Storyboard)FindName("fadeOut");
            _fadeOut.Completed += new EventHandler(_fadeOut_Completed);
        }

        void _fadeOut_Completed(object sender, EventArgs e)
        {
            if (Dispose!=null)
            {
                Dispose(this,new EventArgs());
            }
        }

        public void DisposeControl()
        {
            _fadeOut.Begin();
        }

        public Point[] Points
        {
            set
            {
                PointCollection pc = new PointCollection();
                foreach (Point point in value)
                {
                    pc.Add(point);
                }
                line.Points = pc;
            }
        }
    }
}
