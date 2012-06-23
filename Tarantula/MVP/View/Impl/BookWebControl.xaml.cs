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
    public partial class BookWebControl : UserControl,IBookWebView
    {
        public event EventHandler Update;

        public Canvas Canvas
        {
            get { return container; }
        }

        public BookWebControl()
        {
            InitializeComponent();
        }

        private void RenderLoopStoryBoard_Completed(object sender, EventArgs e)
        {
            if (Update!=null)
            {
                Update(this,new EventArgs());
            }
            RenderLoopStoryBoard.Begin();
        }


    }
}
