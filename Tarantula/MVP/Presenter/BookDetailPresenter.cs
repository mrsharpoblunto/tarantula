using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Tarantula.MVP.Model;
using Tarantula.MVP.Resource;
using Tarantula.MVP.View;
using Tarantula.MVP.Events;

namespace Tarantula.MVP.Presenter
{
    public class BookDetailPresenter : PresenterBase<IBookDetailView>
    {
        public event BookEventHandler Hidden;

        public BookDetailPresenter(IBookDetailView view)
            : base(view)
        {
            view.Hidden += new BookEventHandler(OnViewHidden);
        }

        public void SetBook(string itemID)
        {
            Book book = BookCache.Instance.FindBook(itemID);

            if (book != null)
            {
                View.Title = "Title: " + book.Title;
                View.Author = "Author(s): " + (book.Author != string.Empty ? book.Author : "None specified.");
                View.LowestNewPrice = "Lowest new price: " + (book.LowestNewPrice != string.Empty ? ("$" + Math.Round((decimal.Parse(book.LowestNewPrice) / 100), 2) + " ($US)") : "None specified.");
                View.LowestUsedPrice = "Lowest used price: " + (book.LowestUsedPrice != string.Empty ? ("$" + Math.Round((decimal.Parse(book.LowestUsedPrice) / 100), 2) + " ($US)") : "None specified.");
                View.ItemID = book.ItemID;
                View.ImageURL = book.LargeImageURL;
                View.DetailURL = book.DetailURL;
            }
        }

        private void OnViewHidden(object sender, BookEvent e)
        {
            Hidden(this, e);
        }

        public void Show()
        {
            View.Show();
        }

        public void Hide(string itemID)
        {
            View.Hide(itemID);
        }
    }
}
