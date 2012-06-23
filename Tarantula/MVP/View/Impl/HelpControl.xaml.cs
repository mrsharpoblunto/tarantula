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
using Tarantula.MVP.View.Navigation;

namespace Tarantula.MVP.View.Impl
{
    public partial class HelpControl : UserControl,IHelpView
    {
        public event EventHandler OnOk;

        public HelpControl()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (OnOk!=null)
            {
                OnOk(this,new EventArgs());
            }
        }
    }
}
