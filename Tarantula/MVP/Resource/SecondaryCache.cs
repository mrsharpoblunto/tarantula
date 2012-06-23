using System;
using System.Configuration;
using System.IO.IsolatedStorage;
using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace Tarantula.MVP.Model
{
    /// <summary>
    /// This class implements an xml based cache using isolated storage, so that results can be cached 
    /// between browser sessions to reduce web service calls
    /// </summary>
    public class SecondaryCache
    {
        private double _timeOut;
        private bool _notimeout=false;

        private static readonly SecondaryCache _instance = new SecondaryCache();

        private SecondaryCache() 
        {
            _timeOut = Constants.DEFAULT_SECONDARY_CACHE_TIMEOUT;
        }

        public static SecondaryCache Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// The timeout value of the secondary cache (in seconds)
        /// </summary>
        public double Timeout
        {
            get { return _timeOut; }
            set { _timeOut = value; }
        }

        /// <summary>
        /// saves the contents of the primary cache to isolated storage
        /// unfortunately silverlight 1.1 alpha doesn't have xml serialization so we write it all out manually
        /// </summary>
        /// <param name="books"></param>
        /// <param name="textSearches"></param>
        /// <param name="similaritySearches"></param>
        public void SaveCache(Dictionary<string, Book> books,
                            Dictionary<string, List<Book>> textSearches,
                            Dictionary<string, List<Book>> similaritySearches)
        {
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(Constants.SECONDARY_CACHE_FILE, FileMode.Create, isoStore))
                {
                    using (XmlWriter writer = XmlWriter.Create(isoStream))
                    {
                        writer.WriteStartElement("tarantula");
                        if (_notimeout)
                        {
                            writer.WriteAttributeString("notimeout", "true");
                        }
                        writer.WriteAttributeString("timestamp", DateTime.Now.Ticks.ToString());

                        #region cache all the books
                        writer.WriteStartElement("books");
                        foreach (Book book in books.Values)
                        {
                            writer.WriteStartElement("book");
                            writer.WriteAttributeString("itemid", book.ItemID);
                            writer.WriteAttributeString("title", book.Title);
                            writer.WriteAttributeString("author", book.Author);
                            writer.WriteAttributeString("detailurl", book.DetailURL);
                            writer.WriteAttributeString("largeimageurl", book.LargeImageURL);
                            writer.WriteAttributeString("lowestnewprice", book.LowestNewPrice);
                            writer.WriteAttributeString("lowestusedprice", book.LowestUsedPrice);
                            writer.WriteAttributeString("smallimageurl", book.SmallImageURL);
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                        #endregion

                        #region cache all the text searches
                        writer.WriteStartElement("textsearches");
                        foreach (string textSearch in textSearches.Keys)
                        {
                            writer.WriteStartElement("textsearch");
                            writer.WriteAttributeString("searchtext", textSearch);
                            foreach (Book tsbook in textSearches[textSearch])
                            {
                                writer.WriteStartElement("textsearchbook");
                                writer.WriteAttributeString("itemid", tsbook.ItemID);
                                writer.WriteEndElement();
                            }
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                        #endregion

                        #region cache all the similarity searches
                        writer.WriteStartElement("similaritysearches");
                        foreach (string similaritySearch in similaritySearches.Keys)
                        {
                            writer.WriteStartElement("similaritysearch");
                            writer.WriteAttributeString("similaritytext", similaritySearch);
                            foreach (Book ssbook in similaritySearches[similaritySearch])
                            {
                                writer.WriteStartElement("similaritysearchbook");
                                writer.WriteAttributeString("itemid", ssbook.ItemID);
                                writer.WriteEndElement();
                            }
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                        #endregion

                        writer.WriteEndElement();
                    }
                }
            }
        }

        /// <summary>
        /// loads the contents of the secondary cache into the primary cache (if its still current)
        /// </summary>
        /// <param name="books"></param>
        /// <param name="textSearches"></param>
        /// <param name="similaritySearches"></param>
        public void LoadCache(Dictionary<string, Book> books,
                            Dictionary<string, List<Book>> textSearches,
                            Dictionary<string, List<Book>> similaritySearches)
        {
            books.Clear();
            textSearches.Clear();
            similaritySearches.Clear();

            List<Book> currentTextSearch = null;
            List<Book> currentSimilaritySearch = null;

            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                //there is no cache so quit early
                if (isoStore.GetFileNames(Constants.SECONDARY_CACHE_FILE).Length == 0)
                {
                    return;
                }

                using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(Constants.SECONDARY_CACHE_FILE, FileMode.Open, isoStore))
                {
                    using (XmlReader reader = XmlReader.Create(isoStream))
                    {
                        while (reader.Read())
                        {
                            //get the timestamp and quit early if the cache has been invalidated
                            if (reader.IsStartElement("tarantula"))
                            {
                                if (!string.IsNullOrEmpty(reader.GetAttribute("notimeout")))
                                {
                                    _notimeout = true;
                                }

                                //ignore timeouts if the override has been applied to the secondary cache
                                if (!_notimeout)
                                {
                                    long cacheTimeStamp = long.Parse(reader.GetAttribute("timestamp"));
                                    cacheTimeStamp += (long) (_timeOut*TimeSpan.TicksPerSecond);
                                    if (DateTime.Now.Ticks > cacheTimeStamp)
                                    {
                                        return;
                                    }
                                }
                            }

                            #region read a book into the primary cache
                            if (reader.IsStartElement("book"))
                            {
                                Book newBook = new Book();
                                newBook.ItemID = reader.GetAttribute("itemid");
                                newBook.Author = reader.GetAttribute("author");
                                newBook.DetailURL = reader.GetAttribute("detailurl");
                                newBook.LargeImageURL = reader.GetAttribute("largeimageurl");
                                newBook.LowestNewPrice = reader.GetAttribute("lowestnewprice");
                                newBook.LowestUsedPrice = reader.GetAttribute("lowestusedprice");
                                newBook.SmallImageURL = reader.GetAttribute("smallimageurl");
                                newBook.Title = reader.GetAttribute("title");
                                books.Add(newBook.ItemID, newBook);
                            }
                            #endregion

                            #region begin reading a new text search into the primary cache
                            if (reader.IsStartElement("textsearch"))
                            {
                                currentTextSearch = new List<Book>();
                                textSearches.Add(reader.GetAttribute("searchtext"), currentTextSearch);
                            }

                            if (reader.IsStartElement("textsearchbook"))
                            {
                                currentTextSearch.Add(books[reader.GetAttribute("itemid")]);
                            }
                            #endregion

                            #region begin reading a new similarity search into the primary cache
                            if (reader.IsStartElement("similaritysearch"))
                            {
                                currentSimilaritySearch = new List<Book>();
                                similaritySearches.Add(reader.GetAttribute("similaritytext"), currentSimilaritySearch);
                            }

                            if (reader.IsStartElement("similaritysearchbook"))
                            {
                                currentSimilaritySearch.Add(books[reader.GetAttribute("itemid")]);
                            }
                            #endregion

                        }
                    }
                }
            }

        }
    }
}
