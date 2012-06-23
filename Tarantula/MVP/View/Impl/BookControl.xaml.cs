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

namespace Tarantula.MVP.View.Impl
{
    public partial class BookControl : UserControl, IBookView
    {
        public event MouseEventHandler StartDrag;
        public event EventHandler EndDrag;
        public event BookEventHandler ShowBookDetails;
        public event BookEventHandler DeleteBook;
        event BookEventHandler IBookView.DeleteConnectedBooks
        {
            add { }
            remove { }
        }

        public event BookEventHandler SpawnSimilarBooks;
        public event EventHandler Dispose;

        private static readonly int LOW_ZINDEX = 10;
        private static readonly int HIGH_ZINDEX = 100;
        public static readonly int DELETE_THRESHOLD = 250;

        private Storyboard _fadeOut;
        private Storyboard _expand;
        private string _itemID;
        private string _title;

        public BookControl()
        {
            InitializeComponent();
            SetValue(Canvas.ZIndexProperty, LOW_ZINDEX);

            _fadeOut = (Storyboard)FindName("fadeOut");
            _fadeOut.Completed += new EventHandler(_fadeOut_Completed);

            _expand = (Storyboard)FindName("expand");
            _expand.Completed += new EventHandler(_expand_Completed);

            MouseLeftButtonDown += new MouseButtonEventHandler(BookControl_MouseLeftButtonDown);
            MouseLeftButtonUp += new MouseButtonEventHandler(BookControl_MouseLeftButtonUp);
        }

        void BookControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SetValue(Canvas.ZIndexProperty, LOW_ZINDEX);
            EndDrag(this, e);
        }

        void BookControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetValue(Canvas.ZIndexProperty, HIGH_ZINDEX);
            StartDrag(this, e);

            if (ShowBookDetails != null)
            {
                ShowBookDetails(this, new BookEvent(_itemID));
            }
        }

        void _expand_Completed(object sender, EventArgs e)
        {
            wave.Visibility = System.Windows.Visibility.Collapsed;
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

        public string ItemID
        {
            get { return _itemID; }
            set { _itemID = value; }
        }

        public string ImageURL
        {
            get { return bookImage.Source.ToString(); }
            set
            {
                if (value != null && value != string.Empty)
                {
                    bookImage.Source = new BitmapImage(new Uri(value));
                }
            }
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        private void SpawnButton_Click(object sender, RoutedEventArgs e)
        {
            _expand.Begin();
            wave.Visibility = Visibility.Visible;
            if (SpawnSimilarBooks!=null)
            {
                SpawnSimilarBooks(this,new BookEvent(_itemID));
            }
        }

        //mouse down and up events don't fire for buttons in beta2
        //private void DeleteButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    _clickTime = DateTime.Now;
        //}

        //private void DeleteButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    if (((double)(DateTime.Now.Ticks - _clickTime.Ticks) / 10000) > DELETE_THRESHOLD)
        //    {
        //        if (DeleteConnectedBooks != null)
        //        {
        //            DeleteConnectedBooks(this, new BookEvent(_itemID));
        //        }
        //    }
        //    else
        //    {
        //        if (DeleteBook != null)
        //        {
        //            DeleteBook(this, new BookEvent(_itemID));
        //        }
        //    }
        //}

        public double Top
        {
            get { return (double)GetValue(Canvas.TopProperty); }
            set { SetValue(Canvas.TopProperty, value); }
        }

        public double Left
        {
            get { return (double)GetValue(Canvas.LeftProperty); }
            set { SetValue(Canvas.LeftProperty, value); }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (DeleteBook != null)
            {
                DeleteBook(this, new BookEvent(_itemID));
            }
        }
    }
}
