using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Tarantula.MVP.Presenter;
using Tarantula.MVP.View;
using Tarantula.MVP.View.Impl;

namespace Tarantula
{
    public partial class Page : UserControl
    {
        private NotificationPresenter _notificationPresenter;
        private LoadingPresenter _loadingPresenter;
        private OptionsPresenter _optionsPresenter;
        private HelpPresenter _helpPresenter;
        private TextSearchPresenter _textSearchPresenter;
        private BookDetailPresenter _bookDetailPresenter;
        private BookWebPresenter _bookWebPresenter;

        public Page()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(Page_Loaded);
        }

        void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard fadeInStoryBoard = (Storyboard)FindName("FadeInStoryBoard");
            fadeInStoryBoard.Begin();

            _notificationPresenter = new NotificationPresenter(NotificationControl);
            _notificationPresenter.ShowNotification("Loaded Okay!");

            _loadingPresenter = new LoadingPresenter(LoadingControl);

            _optionsPresenter  = new OptionsPresenter(OptionsControl);
            _optionsPresenter.ClearBookWeb += new EventHandler(_optionsPresenter_ClearBookWeb);
            _optionsPresenter.Help += new EventHandler(_optionsPresenter_Help);
            _optionsPresenter.TextBookSearch += new EventHandler(_optionsPresenter_TextBookSearch);

            _bookDetailPresenter = new BookDetailPresenter(DetailControl);
            _bookDetailPresenter.Hidden += new Tarantula.MVP.Events.BookEventHandler(_bookDetailPresenter_Hidden);

            _helpPresenter = new HelpPresenter(new HelpControl());
            
            _textSearchPresenter = new TextSearchPresenter(new TextSearchControl());
            _textSearchPresenter.StartSearch += new Tarantula.MVP.Events.TextSearchEventHandler(_textSearchPresenter_StartSearch);

            _bookWebPresenter= new BookWebPresenter(bookWebControl);
            _bookWebPresenter.OnLoadingBooks += new EventHandler(_bookWebPresenter_OnLoadingBooks);
            _bookWebPresenter.OnFinishedLoadingBooks += new EventHandler(_bookWebPresenter_OnFinishedLoadingBooks);
            _bookWebPresenter.OnLoadingBooksError += new Tarantula.MVP.Events.BookWebNotificationHandler(_bookWebPresenter_OnLoadingBooksError);
            _bookWebPresenter.ShowBookDetails += new Tarantula.MVP.Events.BookEventHandler(_bookWebPresenter_ShowBookDetails);
            _bookWebPresenter.SearchResultsSummary += new Tarantula.MVP.Events.BookWebNotificationHandler(_bookWebPresenter_SearchResultsSummary);

        }

        void _bookWebPresenter_SearchResultsSummary(object sender, Tarantula.MVP.Events.BookWebNotificationEvent eventArgs)
        {
             _notificationPresenter.ShowNotification(eventArgs.Notification);
        }

        void _bookWebPresenter_ShowBookDetails(object sender, Tarantula.MVP.Events.BookEvent eventArgs)
        {
            _bookDetailPresenter.Hide(eventArgs.ItemID);
        }

        void _bookWebPresenter_OnLoadingBooksError(object sender, Tarantula.MVP.Events.BookWebNotificationEvent eventArgs)
        {          
           _loadingPresenter.HideLoading();
           _notificationPresenter.ShowNotification(eventArgs.Notification);
        }

        void _bookWebPresenter_OnFinishedLoadingBooks(object sender, EventArgs e)
        {
            _loadingPresenter.HideLoading();
        }

        void _bookWebPresenter_OnLoadingBooks(object sender, EventArgs e)
        {
            _loadingPresenter.ShowLoading();
        }

        void _textSearchPresenter_StartSearch(object sender, Tarantula.MVP.Events.TextSearchEvent eventArgs)
        {
            _bookWebPresenter.NewTextSearch(eventArgs.SearchText);
        }

        void _bookDetailPresenter_Hidden(object sender, Tarantula.MVP.Events.BookEvent eventArgs)
        {
            _bookDetailPresenter.SetBook(eventArgs.ItemID);
            _bookDetailPresenter.Show();
        }

        void _optionsPresenter_TextBookSearch(object sender, EventArgs e)
        {
            _textSearchPresenter.Show();
        }

        void _optionsPresenter_Help(object sender, EventArgs e)
        {
            _helpPresenter.Show();
        }

        void _optionsPresenter_ClearBookWeb(object sender, EventArgs e)
        {
            _bookWebPresenter.Clear();
        }
    }
}
