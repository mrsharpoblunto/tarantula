using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Browser;
using System.Xml.Linq;
using Tarantula.MVP.Events;
using System.Windows;
using Tarantula.MVP.Model;

namespace Tarantula.MVP.Resource
{
    /// <summary>
    /// wraps up access to the amazon web service proxy with a caching proxy to speed up repeat searches
    /// </summary>
    public class BookCache
    {
        #region constants

        private const string DESTINATION = "ecs.amazonaws.com";
        private const string SERVICE_VERSION = "2011-08-01";
        private const string SERVICE_XMLNS = "http://webservices.amazon.com/AWSECommerceService/2011-08-01";

        private const string SEARCH_INDEX = "Books";
        private const string ITEM_PAGE = "1";
        private const string RESPONSE_GROUP = "Request,Small,OfferSummary,Images,Reviews";

        #endregion

        private class TextSearchState {
            public WebRequest Request { get; set; }
            public string SearchText { get; set; }
            public BooksFoundEventHandler Callback { get; set; }
        }

        private static readonly BookCache _instance = new BookCache();
        private readonly Dictionary<string, Book> _books;
        private readonly Dictionary<string, List<Book>> _textSearches;
        private readonly Dictionary<string, List<Book>> _similaritySearches;
        private readonly SignedRequestHelper _requestSigner;

        private BookCache()
        {
            _books = new Dictionary<string, Book>();
            _textSearches = new Dictionary<string, List<Book>>();
            _similaritySearches = new Dictionary<string, List<Book>>();

            _requestSigner = new SignedRequestHelper(Constants.MY_AWS_ACCESS_KEY_ID, Constants.MY_AWS_SECRET_KEY, DESTINATION);

            //load up any information present in the secondary cache
            SecondaryCache.Instance.LoadCache(_books, _textSearches, _similaritySearches);        }

        public static BookCache Instance
        {
            get { return _instance; }
        }

        public void Clear()
        {
            _books.Clear();
            _textSearches.Clear();
            _similaritySearches.Clear();
        }

        public void Save()
        {
            //cache the results out to a secondary cache
            SecondaryCache.Instance.SaveCache(_books, _textSearches, _similaritySearches);            
        }

        #region find books by keyword

        public void FindBooksByKeyword(string searchText, BooksFoundEventHandler callback)
        {
            //check if the search has been cached, if it hasn't then call the web service
            if (!_textSearches.ContainsKey(searchText))
            {
                IDictionary<string, string> requestParams = new Dictionary<string, String>();
                requestParams["Service"] = "AWSECommerceService";
                requestParams["Operation"] = "ItemSearch";
                requestParams["Keywords"] = HttpUtility.UrlEncode(searchText);
                requestParams["ResponseGroup"] = RESPONSE_GROUP;
                requestParams["SearchIndex"] = SEARCH_INDEX;
                requestParams["ItemPage"] = ITEM_PAGE;
                requestParams["Version"] = SERVICE_VERSION;
                requestParams["AssociateTag"] = Constants.MY_AWS_ASSOCIATE_TAG;

                Uri query = new Uri(_requestSigner.Sign(requestParams), UriKind.Absolute);

                _textSearches.Add(searchText, new List<Book>());
                WebRequest request = HttpWebRequest.Create(query);

                TextSearchState state = new TextSearchState();
                state.SearchText = searchText;
                state.Callback = callback;
                state.Request = request;

                request.BeginGetResponse(Service_ItemSearchCompleted, state);
            }
            //if it has then retrive the results from the cache
            else
            {
                BooksFoundEvent foundEvent = new BooksFoundEvent();
                foundEvent.Success = true;
                foundEvent.Results = _textSearches[searchText];
                callback.Invoke(string.Empty, foundEvent);
            }
        }

        private void Service_ItemSearchCompleted(IAsyncResult result)
        {
            TextSearchState requestState = (TextSearchState)result.AsyncState;
            BooksFoundEvent foundEvent = new BooksFoundEvent();
            
            try
            {
                List<Book> books = new List<Book>();
                books = BuildResponseList(requestState.Request, result);

                //cache the search results, and the induvidual books
                _textSearches[requestState.SearchText].AddRange(books);
                AddBooks(books.ToArray());

                //record the results
                foundEvent.Results = _textSearches[requestState.SearchText];
                foundEvent.Success = true;
            }
            catch (Exception)
            {
                foundEvent.Success = false;
                foundEvent.FailureMessage = "No Results Found";
            }
            requestState.Callback.Invoke(string.Empty, foundEvent);
        }

        #endregion

        #region find books by similarity

        public void FindSimilarBooks(string itemID, BooksFoundEventHandler callback)
        {
            if (!_books.ContainsKey(itemID)) {
                BooksFoundEvent foundEvent = new BooksFoundEvent();
                foundEvent.Success = false;
                foundEvent.FailureMessage = "No item exists for the given ID";
                foundEvent.Results = new List<Book>();
                callback.Invoke(itemID, foundEvent);
            }

            //check if the search has been cached, if it hasn't then call the web service
            if (!_similaritySearches.ContainsKey(itemID))
            {
                IDictionary<string, string> requestParams = new Dictionary<string, String>();
                requestParams["Service"] = "AWSECommerceService";
                requestParams["Operation"] = "SimilarityLookup";
                requestParams["ItemId"] = itemID;
                requestParams["ResponseGroup"] = RESPONSE_GROUP;
                requestParams["SimilarityType"] = "Random";
                requestParams["Version"] = SERVICE_VERSION;
                requestParams["AssociateTag"] = Constants.MY_AWS_ASSOCIATE_TAG;

                Uri query = new Uri(_requestSigner.Sign(requestParams), UriKind.Absolute);

                _similaritySearches.Add(itemID, new List<Book>());
                WebRequest request = HttpWebRequest.Create(query);

                TextSearchState state = new TextSearchState();
                state.SearchText = itemID;
                state.Callback = callback;
                state.Request = request;

                request.BeginGetResponse(Service_SimilarityLookupCompleted, state);
            }
            //if it has then retrieve the results from the cache
            else
            {
                BooksFoundEvent foundEvent = new BooksFoundEvent();
                foundEvent.Success = true;
                foundEvent.Results = _similaritySearches[itemID];
                callback.Invoke(itemID, foundEvent);
            }
        }

        private void Service_SimilarityLookupCompleted(IAsyncResult result)
        {
            BooksFoundEvent foundEvent = new BooksFoundEvent();
            TextSearchState requestState = (TextSearchState)result.AsyncState;

            try
            {
                List<Book> books = new List<Book>();
                books = BuildResponseList(requestState.Request, result);

                //cache the search results, and the induvidual books
                _similaritySearches[requestState.SearchText].AddRange(books);
                AddBooks(books.ToArray());

                //record the results
                foundEvent.Results = _similaritySearches[requestState.SearchText];
                foundEvent.Success = true;
            }
            catch (Exception)
            {
                foundEvent.Success = false;
                foundEvent.FailureMessage = "No Results Found";
            }
            requestState.Callback.Invoke(requestState.SearchText, foundEvent);
        }

        #endregion

        public Book FindBook(string itemID)
        {
            if (_books.ContainsKey(itemID))
            {
                return _books[itemID];
            }
            return null;
        }

        private void AddBooks(Book[] books)
        {
            foreach (Book book in books)
            {
                if (!_books.ContainsKey(book.ItemID))
                {
                    _books.Add(book.ItemID, book);
                }
            }
        }

        #region helpers

        private static XName Name(string name)
        {
            return XName.Get(name, SERVICE_XMLNS);
        }

        private static List<Book> BuildResponseList(WebRequest request,IAsyncResult asyncResult)
        {
            List<Book> result = new List<Book>();

            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asyncResult);

            using (TextReader reader = new StreamReader(response.GetResponseStream()))
            {
                XDocument xmlProducts = XDocument.Parse(reader.ReadToEnd());

                IEnumerable<Book> books = from book in xmlProducts.Descendants(Name("Item"))
                            select new Book
                            {
                                Title = book.Element(Name("ItemAttributes")).Element(Name("Title")).Value,
                                ItemID = book.Element(Name("ASIN")).Value,
                                DetailURL = book.Element(Name("DetailPageURL")).Value,
                                Author = (from author in book.Element(Name("ItemAttributes")).Descendants(Name("Author"))
                                         select author.Value).Aggregate(string.Empty,(acc,a) => acc+=","+a,acc => !string.IsNullOrEmpty(acc) ? acc.Substring(1): acc),

                                SmallImageURL =  book.Element(Name("SmallImage")) != null ? book.Element(Name("SmallImage")).Element(Name("URL")).Value : string.Empty,
                                LargeImageURL = book.Element(Name("MediumImage")) != null ? book.Element(Name("MediumImage")).Element(Name("URL")).Value : string.Empty,
                                LowestNewPrice = book.Element(Name("OfferSummary")) != null && book.Element(Name("OfferSummary")).Element(Name("LowestNewPrice")) != null ? book.Element(Name("OfferSummary")).Element(Name("LowestNewPrice")).Element(Name("Amount")).Value : string.Empty,
                                LowestUsedPrice = book.Element(Name("OfferSummary")) != null && book.Element(Name("OfferSummary")).Element(Name("LowestUsedPrice")) != null ? book.Element(Name("OfferSummary")).Element(Name("LowestUsedPrice")).Element(Name("Amount")).Value : string.Empty
                            };

                foreach (var book in books)
                {
                    //only bother including books that have a cover image
                    if (!string.IsNullOrEmpty(book.SmallImageURL)
                        && !string.IsNullOrEmpty(book.LargeImageURL)
                        && book.SmallImageURL.ToLower().EndsWith(".jpg") &&
                        book.LargeImageURL.ToLower().EndsWith(".jpg"))
                    {
                        result.Add(book);
                    }
                }
            }

            return result;
        }

        #endregion

    }
}
