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
using Tarantula.MVP.Events;
using Tarantula.MVP.View.Navigation;

namespace Tarantula.MVP.View.Impl
{
    public partial class TextSearchControl : UserControl,ITextSearchView
    {
        public event TextSearchEventHandler StartSearch;
        public event EventHandler CancelSearch;

        public TextSearchControl()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(TextSearchControl_Loaded);
        }

        void TextSearchControl_Loaded(object sender, RoutedEventArgs e)
        {
            searchText.Focus();
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            if (StartSearch != null && !string.IsNullOrEmpty(searchText.Text))
            {
                TextSearchEvent args = new TextSearchEvent(searchText.Text);
                StartSearch(this, args);
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (CancelSearch != null)
            {
                CancelSearch(this, new EventArgs());
            }
        }

        private void UserControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key==Key.Enter)
            {
                searchButton_Click(this,new RoutedEventArgs());              
            }
            else if (e.Key==Key.Escape)
            {
                cancelButton_Click(this,new RoutedEventArgs());
            }
        }


    }
}
