using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Tarantula.MVP.Events;
using System.Windows.Browser;

namespace Tarantula.MVP.View.Impl
{
    public partial class BookDetailControl : UserControl, IBookDetailView
    { 
        public event BookEventHandler Hidden;

        private readonly Storyboard _fadeIn;
        private readonly Storyboard _fadeOut;
        private string _itemID, _detailURL;

        public BookDetailControl()
        {
            InitializeComponent();

            detail.Visibility = System.Windows.Visibility.Collapsed;
            _fadeIn = (Storyboard) FindName("fadeIn");
            _fadeOut = (Storyboard) FindName("fadeOut");
            _fadeOut.Completed += new EventHandler(_fadeOut_Completed);
        }

        void _fadeOut_Completed(object sender, EventArgs e)
        {
            detail.Visibility = System.Windows.Visibility.Collapsed;
            if (Hidden != null)
            {
                Hidden(this, new BookEvent(_itemID));
            }
        }

        public string Author
        {
            set { author.Text = value; }
        }

        public string ItemID
        {
            set { _itemID = value; }
        }

        public string ImageURL
        {
            set
            {
                if (value != null && value != string.Empty)
                {
                    image.Source = new BitmapImage(new Uri(value));
                }
            }
        }

        public string Title
        {
            set { title.Text = value; }
        }

        public string DetailURL
        {
            set { _detailURL = value; }
        }

        public string LowestNewPrice
        {
            set { lowestNewPrice.Text = value; }
        }

        public string LowestUsedPrice
        {
            set { lowestUsedPrice.Text = value; }
        }

        public void Show()
        {
            detail.Visibility = System.Windows.Visibility.Visible;
            _fadeOut.Pause();
            _fadeIn.Begin();
        }

        public void Hide(string itemID)
        {
            if (itemID != _itemID)
            {
                _itemID = itemID;
                if (detail.Visibility == Visibility.Visible)
                {
                    _fadeIn.Pause();
                    _fadeOut.Begin();
                }
                else
                {
                    if (Hidden != null)
                    {
                        Hidden(this, new BookEvent(_itemID));
                    }
                }
            }
        }

        private void DetailsButton_Click(object sender, RoutedEventArgs e)
        {
            HtmlPage.Window.Navigate(new Uri(_detailURL), "__blank"); 
        }
    }
}
