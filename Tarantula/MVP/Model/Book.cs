using System;

namespace Tarantula.MVP.Model
{
    /// <summary>
    /// Summary description for AmazonItem
    /// </summary>
    public class Book
    {
        #region fields

        private string _title;
        private string _author;
        private string _smallImageURL;
        private string _largeImageURL;
        private string _detailURL;
        private string _itemID;
        private string _lowestNewPrice;
        private string _lowestUsedPrice;

        #endregion

        #region constructor

        public Book()
        {
            _title = string.Empty;
            _author = string.Empty;
            _smallImageURL = string.Empty;
            _largeImageURL = string.Empty;
            _itemID = string.Empty;
            _detailURL = string.Empty;
            _lowestNewPrice = string.Empty;
            _lowestUsedPrice = string.Empty;
        }

        #endregion

        #region properties

        public string LowestNewPrice
        {
            get { return _lowestNewPrice; }
            set { _lowestNewPrice = value; }
        }

        public string LowestUsedPrice
        {
            get { return _lowestUsedPrice; }
            set { _lowestUsedPrice = value; }
        }

        public string DetailURL
        {
            get { return _detailURL; }
            set { _detailURL = value; }
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; }

        }
        public string Author
        {
            get { return _author; }
            set { _author = value; }
        }

        public string SmallImageURL
        {
            get { return _smallImageURL; }
            set { _smallImageURL = value; }
        }

        public string LargeImageURL
        {
            get { return _largeImageURL; }
            set { _largeImageURL = value; }
        }

        public string ItemID
        {
            get { return _itemID; }
            set { _itemID = value; }
        }

        #endregion
    }

}