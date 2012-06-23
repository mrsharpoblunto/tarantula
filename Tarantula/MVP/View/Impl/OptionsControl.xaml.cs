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
    public partial class OptionsControl : UserControl,IOptionsView
    {
        public event EventHandler TextBookSearch;
        public event EventHandler ClearBookWeb;
        public event EventHandler ShowHelp;

        public OptionsControl()
        {
            InitializeComponent();
        }

        private void TextSearch_Click(object sender, RoutedEventArgs e)
        {
            if (TextBookSearch!=null)
            {
                TextBookSearch(this,new EventArgs());
            }
        }

        private void ClearResults_Click(object sender, RoutedEventArgs e)
        {
            if (ClearBookWeb != null)
            {
                ClearBookWeb(this, new EventArgs());
            }
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            if (ShowHelp != null)
            {
                ShowHelp(this, new EventArgs());
            }
        }

    }
}
