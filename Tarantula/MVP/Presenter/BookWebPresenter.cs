using System;
using System.Collections.Generic;
using System.Text;
using Physics;
using System.Windows.Controls;
using Tarantula.MVP.View;
using Tarantula.MVP.Events;
using Tarantula.MVP.Resource;
using System.Windows.Input;
using Physics.ForceDissipationFunction;
using System.Windows;
using Tarantula.MVP.Model;

namespace Tarantula.MVP.Presenter
{
    public class BookWebPresenter: PresenterBase<IBookWebView>
    {
        private double OFFSET_X = 72;
        private double OFFSET_Y = 106;

        private struct DragInfo
        {
            public Point Offset;
            public Point Position;
            public IBookView View;
        }

        public event EventHandler OnLoadingBooks;
        public event EventHandler OnFinishedLoadingBooks;
        public event BookWebNotificationHandler OnLoadingBooksError;
        public event BookEventHandler ShowBookDetails;
        public event BookWebNotificationHandler SearchResultsSummary;

        private ParticleSimulation<IBookView, IBookConnectorView> _simulation;
        private DragInfo _dragInfo;

        public BookWebPresenter(IBookWebView view) : base(view)
        {
            view.Update += new EventHandler(view_Update);
            view.MouseMove += new MouseEventHandler(view_MouseMove);
            view.SizeChanged += new SizeChangedEventHandler(view_SizeChanged);

            _dragInfo = new DragInfo();

            //create physics simulator
            _simulation = new ParticleSimulation<IBookView, IBookConnectorView>((OFFSET_Y/2), (OFFSET_X/2), view.ActualWidth-OFFSET_X, view.ActualHeight-OFFSET_Y
                          , Constants.DEFAULT_GRAVITY
                          , Constants.DEFAULT_FRICTION_CONSTANT
                          , Constants.DEFAULT_STATIC_FRICTION
                          , Constants.DEFAULT_CONNECTION_LENGTH
                          , Constants.DEFAULT_SPRING_CONSTANT
                          , ForceDissipationFunctionFactory.LinearDissipationFunction(Constants.DEFAULT_RESTING_LENGTH));
            //hook up event to remove metadata when the associated particles are removed from the simulation
            _simulation.DisposeParticleMetaData += new MetaDataDisposeHandler<IBookView>(OnBookControlDispose);

            //register the shaking mouse gesture
            GestureManager.Instance.RegisterGesture(GestureFactory.CreateShakeGesture(), new GestureEventHandler(OnShake));
        }

        #region methods

        public void NewTextSearch(string searchtext)
        {
            FindBooksByKeyword(searchtext);
        }

        public void Clear()
        {
            for (int i = _simulation.Particles.Count - 1; i >= 0; --i)
            {
                Particle<IBookView, IBookConnectorView> p = _simulation.Particles[i];
                _simulation.RemoveParticle(p);
            }
        }

        #endregion

        #region event handlers

        void view_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _simulation.Height = e.NewSize.Height-OFFSET_Y;
            _simulation.Width = e.NewSize.Width-OFFSET_X;
        }

        void view_MouseMove(object sender, MouseEventArgs e)
        {
            _dragInfo.Position = e.GetPosition((UserControl)View);
            GestureManager.Instance.RecordGestureHistory(_dragInfo.Position);
        }

        void view_Update(object sender, EventArgs e)
        {
            //detect and act on any mouse gestures made by the user
            GestureManager.Instance.ProcessGestureHistory();

            //run through one frame of physics
            _simulation.RunSimulation(Constants.DEFAULT_TIMESTEP);

            //update the visual representation of the particles
            foreach (Particle<IBookView, IBookConnectorView> p in _simulation.Particles)
            {
                IBookView view = p.MetaData;

                //take away the phsyics of the currently selected item (if any)
                if (view == _dragInfo.View)
                {
                    double x = Math.Min(Math.Max(_simulation.Left, _dragInfo.Position.X - _dragInfo.Offset.X + (view.ActualWidth / 2)), _simulation.Width + _simulation.Left);
                    double y = Math.Min(Math.Max(_simulation.Top, _dragInfo.Position.Y - _dragInfo.Offset.Y + (view.ActualHeight / 2)), _simulation.Height + _simulation.Top);
                    p.InitPosition(x, y);
                }

                view.Left = p.Position.X - (view.ActualWidth / 2);
                view.Top = p.Position.Y - (view.ActualHeight / 2);

                //update the connector views
                foreach (ParticleConnector<IBookView, IBookConnectorView> c in p.Connectors)
                {
                    IBookConnectorView cview = c.MetaData;

                    Point[] points = new Point[] {
                            new Point(c.Particle1.Position.X,c.Particle1.Position.Y),
                            new Point(c.Particle2.Position.X,c.Particle2.Position.Y),
                            new Point(c.Particle2.PreviousPosition.X,c.Particle2.PreviousPosition.Y),
                            new Point(c.Particle1.PreviousPosition.X,c.Particle1.PreviousPosition.Y)};
                    cview.Points = points;
                }

            }
        }

        private void OnShake(object sender, GestureEvent e)
        {
            //disconnect any particles that have been shaken around
            if (_dragInfo.View != null)
            {
                foreach (Particle<IBookView, IBookConnectorView> p in _simulation.Particles)
                {
                    if (p.MetaData == _dragInfo.View)
                    {
                        p.DisconnectParticle();
                    }
                }
            }
        }

        /// <summary>
        /// requests a set of books matching the search criteria from the amazon web service
        /// </summary>
        private void FindBooksByKeyword(string initialBookSearch)
        {
            //give the controler a chance to put up some sort of loading indicator
            OnLoadingBooks(this, new EventArgs());

            BookCache.Instance.FindBooksByKeyword(initialBookSearch, new BooksFoundEventHandler(OnFindBooksByKeyWord));
        }

        /// <summary>
        /// requests a set of similar books from the amazon web service
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void OnViewSpawnSimilarBooks(object sender, BookEvent eventArgs)
        {
            //give the controler a chance to put up some sort of loading indicator
            OnLoadingBooks(this, new EventArgs());

            BookCache.Instance.FindSimilarBooks(eventArgs.ItemID, new BooksFoundEventHandler(OnSpawnSimilarBooks));
        }

        /// <summary>
        /// delete a selected book from the particle simulation and also remove it from the 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void OnViewDeleteBook(object sender, BookEvent eventArgs)
        {
            foreach (Particle<IBookView, IBookConnectorView> p in _simulation.Particles)
            {
                if (p.MetaData.ItemID == eventArgs.ItemID)
                {
                    _simulation.RemoveParticle(p);
                    break;
                }
            }
        }

        private void OnViewDeleteConnectedBooks(object sender, BookEvent eventArgs)
        {
            foreach (Particle<IBookView, IBookConnectorView> p in _simulation.Particles)
            {
                if (p.MetaData.ItemID == eventArgs.ItemID)
                {
                    Dictionary<IBookView, bool> markedForDeletion = new Dictionary<IBookView, bool>();
                    //do a recursive search to find all particles that are part of the current graph
                    RecursiveDelete(p, ref markedForDeletion);

                    //pass through all particles and delete any that were marked for deletion
                    for (int i = _simulation.Particles.Count - 1; i >= 0; --i)
                    {
                        if (markedForDeletion.ContainsKey(_simulation.Particles[i].MetaData))
                        {
                            _simulation.RemoveParticle(_simulation.Particles[i]);
                        }
                    }
                    break;
                }
            }
        }

        private void RecursiveDelete(Particle<IBookView, IBookConnectorView> p, ref Dictionary<IBookView, bool> markedForDeletion)
        {
            //if the book exists and hasn't yet been marked for deletion then recursively delete it and its children
            if (!markedForDeletion.ContainsKey(p.MetaData))
            {
                markedForDeletion.Add(p.MetaData, true);

                foreach (ParticleConnector<IBookView, IBookConnectorView> connector in p.Connectors)
                {
                    if (connector.Particle1 != p)
                    {
                        RecursiveDelete(connector.Particle1, ref markedForDeletion);
                    }
                    else
                    {
                        RecursiveDelete(connector.Particle2, ref markedForDeletion);
                    }
                }
            }
        }

        private void OnViewShowBookDetails(object sender, BookEvent eventArgs)
        {
            ShowBookDetails(this, eventArgs);
        }

        private void OnEndDrag(object sender, EventArgs e)
        {
            _dragInfo.View = null;
        }

        private void OnStartDrag(object sender, MouseEventArgs e)
        {
            _dragInfo.View = (IBookView)sender;
            _dragInfo.Position = e.GetPosition((UserControl)View);
            _dragInfo.Offset = e.GetPosition((UserControl)_dragInfo.View);
        }

        private void OnBookControlDispose(object sender, MetaDataDisposeEventArgs<IBookView> e)
        {
            e.MetaData.DisposeControl();
        }

        private void OnControlDisposed(object sender, EventArgs e)
        {
            View.Canvas.Children.Remove((UserControl)sender);
        }

        private void OnBookConnectorControlDispose(object sender, MetaDataDisposeEventArgs<IBookConnectorView> e)
        {
            e.MetaData.DisposeControl();
        }

        #endregion

        #region web service event handlers

        /// <summary>
        /// when the webservice completes a request initiaited by FindBooksByKeyword, this method takes the results
        /// and creates the view objects to represent the results
        /// </summary>
        private void OnFindBooksByKeyWord(string parentItemID, BooksFoundEvent e)
        {
            View.Dispatcher.BeginInvoke(() =>
            {
                if (e.Success)
                {
                    Random random = new Random(DateTime.Now.Millisecond);
                    foreach (Book book in e.Results)
                    {
                        Particle<IBookView, IBookConnectorView> existingParticle = FindParticle(book.ItemID);

                        if (existingParticle == null)
                        {
                            IBookView view = CreateView(book);

                            //create the particle in the canvas center and add the view as the particles metadata object
                            Particle<IBookView, IBookConnectorView> p
                                = new Particle<IBookView, IBookConnectorView>(Constants.DEFAULT_MASS, Constants.DEFAULT_REPELLANT_FORCE, _simulation.ForceDissipationFunction);
                            p.InitPosition(random.Next((int)Math.Round(View.ActualWidth)), random.Next((int)Math.Round(View.ActualWidth)));
                            p.DisposeConnectorMetaData += new MetaDataDisposeHandler<IBookConnectorView>(OnBookConnectorControlDispose);

                            _simulation.AddParticle(p, view);

                            //add the view to the containing canvas
                            View.Canvas.Children.Add((UserControl)view);
                        }
                    }

                    //hide whatever loading indicator was put up
                    OnFinishedLoadingBooks(this, new EventArgs());
                    SearchResultsSummary(this, new BookWebNotificationEvent(e.Results.Count + " Result(s) found"));
                }
                else
                {
                    //give the controller an opportunity to display a loading error
                    OnLoadingBooksError(this, new BookWebNotificationEvent(e.FailureMessage));
                }
            });
        }

        /// <summary>
        /// when the webservice completes a request initiaited by OnViewSpawnSimilarBooks, this method takes the results
        /// and creates the view objects to represent the results
        /// </summary>
        private void OnSpawnSimilarBooks(string parentItemID, BooksFoundEvent e)
        {
            View.Dispatcher.BeginInvoke(() =>
            {
                if (e.Success)
                {
                    //find the parent particle to add the similar books to
                    Particle<IBookView, IBookConnectorView> parentParticle = FindParticle(parentItemID);

                    Random random = new Random(DateTime.Now.Millisecond);
                    if (parentParticle != null)
                    {
                        foreach (Book book in e.Results)
                        {
                            Particle<IBookView, IBookConnectorView> existingParticle = FindParticle(book.ItemID);
                            //if the particle spawned doesn't already exist in the simulation, create it and attach it to the parent
                            if (existingParticle == null)
                            {
                                //create the view object
                                IBookView view = CreateView(book);
                                IBookConnectorView connectorView = CreateConnectorView();

                                //create the particle in the canvas center and add the view as the particles metadata object
                                Particle<IBookView, IBookConnectorView> p
                                    = new Particle<IBookView, IBookConnectorView>(Constants.DEFAULT_MASS, Constants.DEFAULT_REPELLANT_FORCE, _simulation.ForceDissipationFunction);
                                p.InitPosition(random.Next((int)Math.Round(View.ActualWidth)), random.Next((int)Math.Round(View.ActualHeight)));
                                p.DisposeConnectorMetaData += new MetaDataDisposeHandler<IBookConnectorView>(OnBookConnectorControlDispose);

                                _simulation.AddParticleToParticle(p, view, parentParticle, connectorView);

                                //add the view to the containing canvas
                                View.Canvas.Children.Add((UserControl)view);
                                View.Canvas.Children.Add((UserControl)connectorView);
                            }
                            //the particle exists, add a connection from the parent particle to the existing particle
                            else
                            {
                                //create the particle connector view object
                                IBookConnectorView connectorView = ViewFactory.CreateBookConnectorView();
                                parentParticle.AddConnection(existingParticle, connectorView, Constants.DEFAULT_CONNECTION_LENGTH, Constants.DEFAULT_SPRING_CONSTANT);
                                View.Canvas.Children.Add((UserControl)connectorView);
                            }
                        }
                    }

                    //hide whatever loading indicator was put up
                    OnFinishedLoadingBooks(this, new EventArgs());
                    SearchResultsSummary(this, new BookWebNotificationEvent(e.Results.Count + " Result(s) found"));
                }
                else
                {
                    //give the controller an opportunity to display a loading error
                    OnLoadingBooksError(this, new BookWebNotificationEvent(e.FailureMessage));
                }
            });
        }

        #endregion

        #region helpers

        private IBookView CreateView(Book book)
        {
            //create the view object
            IBookView view = ViewFactory.CreateBookView();
            view.ImageURL = book.SmallImageURL;
            view.ItemID = book.ItemID;
            view.Title = book.Title;
            view.StartDrag += new MouseEventHandler(OnStartDrag);
            view.EndDrag += new EventHandler(OnEndDrag);
            view.SpawnSimilarBooks += new BookEventHandler(OnViewSpawnSimilarBooks);
            view.DeleteBook += new BookEventHandler(OnViewDeleteBook);
            view.DeleteConnectedBooks += new BookEventHandler(OnViewDeleteConnectedBooks);
            view.ShowBookDetails += new BookEventHandler(OnViewShowBookDetails);
            view.Dispose += new EventHandler(OnControlDisposed);
            return view;
        }

        private IBookConnectorView CreateConnectorView()
        {
            //create the particle connector view object
            IBookConnectorView view = ViewFactory.CreateBookConnectorView();
            view.Dispose += new EventHandler(OnControlDisposed);
            return view;
        }

        private Particle<IBookView, IBookConnectorView> FindParticle(string itemID)
        {
            foreach (Particle<IBookView, IBookConnectorView> p in _simulation.Particles)
            {
                if (p.MetaData.ItemID == itemID)
                {
                    return p;
                }
            }
            return null;
        }

        #endregion


        }
    }
