using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using MultiAgentSystemPCL;

namespace Fish
{
    /// <summary>
    /// </summary>
    /// <seealso cref="System.Windows.Window" />
    /// <seealso cref="System.Windows.Markup.IComponentConnector" />
    public partial class MainWindow : Window
    {
        /// <summary>
        ///     My ocean
        /// </summary>
        private Ocean myOcean;

        public int nbLapinStart = 20;
        public int nbRenardStart = 2;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        /// <summary>
        ///     Handles the Loaded event of the MainWindow control.
        /// </summary>
        /// <param name="_sender">The source of the event.</param>
        /// <param name="_e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void MainWindow_Loaded(object _sender, RoutedEventArgs _e)
        {
            //this.oceanCanvas.MouseDown += this.oceanCanvas_MouseDown;
            oceanCanvas.MouseRightButtonDown += oceanCanvas_MouseDown;
            oceanCanvas.MouseLeftButtonDown += oceanCanvas_MouseDown;


            myOcean = new Ocean(nbLapinStart, nbRenardStart, oceanCanvas.ActualWidth, oceanCanvas.ActualHeight);
            myOcean.oceanUpdatedEvent += myOcean_oceanUpdatedEvent;

            var dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 15);
            dispatcherTimer.Start();
        }

        /// <summary>
        ///     Handles the Tick event of the dispatcherTimer control.
        /// </summary>
        /// <param name="_sender">The source of the event.</param>
        /// <param name="_e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void dispatcherTimer_Tick(object _sender, EventArgs _e)
        {
            myOcean.UpdateEnvironnement();
            LapinPresent.Content = myOcean.lapinList.Count;
            RenardPresent.Content = myOcean.renardList.Count;
            EatBar.Content = Ocean.getTimeEat();
            LifeBar.Content = Ocean.getTimeLife();
            VisibilityFox.Content = Ocean.getVisionRenard();
            //this.RenardPresent.Content = this.myOcean.renardList.Count;
            TikLapinApparition.Content = myOcean.timeToAppear;
        }

        private void ValidationParameter(object sender, RoutedEventArgs e)
        {
            int nbLapinStartCmd;
            int nbRenardStartCmd;
            int lapinTikAppear;
            int renardSeeVision;
            int renardTimeToEat;
            int animalTimeToLive;

            if (LapinStart.Text != null && int.TryParse(LapinStart.Text, out nbLapinStartCmd))
            {
                myOcean.lapinList.RemoveAll(agent => agent.Equals(agent));
                for (var i = 0; i < nbLapinStartCmd; i++)
                    myOcean.lapinList.Add(new LapinAgent(myOcean.randomGenerator.NextDouble() * myOcean.MAX_WIDTH,
                        myOcean.randomGenerator.NextDouble() * myOcean.MAX_HEIGHT,
                        myOcean.randomGenerator.NextDouble() * 2 * Math.PI));
            }

            if (RenardStart.Text != null && int.TryParse(RenardStart.Text, out nbRenardStartCmd))
            {
                myOcean.renardList.RemoveAll(agent => agent.Equals(agent));
                for (var i = 0; i < nbRenardStartCmd; i++)
                    myOcean.renardList.Add(new RenardAgent(myOcean.randomGenerator.NextDouble() * myOcean.MAX_WIDTH,
                        myOcean.randomGenerator.NextDouble() * myOcean.MAX_HEIGHT,
                        myOcean.randomGenerator.NextDouble() * 2 * Math.PI));
            }

            if (LapinTik.Text != null && int.TryParse(LapinTik.Text, out lapinTikAppear))
                myOcean.timeToAppear = lapinTikAppear;

            if (RenardSee.Text != null && int.TryParse(RenardSee.Text, out renardSeeVision))
            {
                for (var i = 0; i < myOcean.renardList.Count; i++) myOcean.renardList[i].DISTANCE_MIN = renardSeeVision;
                Ocean.setVisionRenard(renardSeeVision);
            }

            if (RenardRepas.Text != null && int.TryParse(RenardRepas.Text, out renardTimeToEat))
            {
                for (var i = 0; i < myOcean.renardList.Count; i++) myOcean.renardList[i].timeToLive = renardTimeToEat;
                Ocean.setTimeEat(renardTimeToEat);
            }

            if (RenardVieux.Text != null && int.TryParse(RenardVieux.Text, out animalTimeToLive))
            {
                for (var i = 0; i < myOcean.renardList.Count; i++)
                    myOcean.renardList[i].timeToLiveAlt = animalTimeToLive;
                for (var i = 0; i < myOcean.lapinList.Count; i++) myOcean.lapinList[i].timeToLiveAlt = animalTimeToLive;
                Ocean.setTimeLife(animalTimeToLive);
            }
        }

        /// <summary>
        ///     Mies the ocean_ocean updated event.
        /// </summary>
        /// <param name="_lapin">The _lapin.</param>
        /// <param name="_renard">The _renard.</param>
        /// <param name="_obstacles">The _obstacles.</param>
        private void myOcean_oceanUpdatedEvent(List<LapinAgent> _lapin, List<RenardAgent> _renard,
            List<BadZone> _obstacles)
        {
            oceanCanvas.Children.Clear();

            foreach (var lapin in _lapin) DrawLapin(lapin);

            foreach (var renard in _renard) DrawRenard(renard);

            foreach (var obstacle in _obstacles) DrawObstacle(obstacle);

            oceanCanvas.UpdateLayout();
        }

        /// <summary>
        ///     Draws the obstacle.
        /// </summary>
        /// <param name="_obstacle">The _obstacle.</param>
        private void DrawObstacle(BadZone _obstacle)
        {
            var circle = new Ellipse();
            circle.StrokeThickness = 50;
            circle.Stroke = new ImageBrush(new BitmapImage(new Uri(@"../../arbre.png", UriKind.Relative)));
            circle.Margin = new Thickness(_obstacle.PosX - _obstacle.Radius, _obstacle.PosY - _obstacle.Radius, 0, 0);
            oceanCanvas.Children.Add(circle);
        }

        /// <summary>
        ///     Draws the lapin.
        /// </summary>
        /// <param name="_lapin">The _lapin.</param>
        private void DrawLapin(LapinAgent _lapin)
        {
            var body = new Line();
            body.StrokeThickness = 40;
            if (_lapin.SpeedX >= 0 && _lapin.SpeedY >= 0)
                body.Stroke = new ImageBrush(new BitmapImage(new Uri(@"../../lapinb.png", UriKind.Relative)));
            else if (_lapin.SpeedX < 0 && _lapin.SpeedY < 0)
                body.Stroke = new ImageBrush(new BitmapImage(new Uri(@"../../lapinh.png", UriKind.Relative)));
            else if (_lapin.SpeedX >= 0 && _lapin.SpeedY < 0)
                body.Stroke = new ImageBrush(new BitmapImage(new Uri(@"../../lapind.png", UriKind.Relative)));
            else
                body.Stroke = new ImageBrush(new BitmapImage(new Uri(@"../../laping.png", UriKind.Relative)));
            //body.Stroke = new ImageBrush(new BitmapImage(new Uri(@"../../lapin.png", UriKind.Relative)));
            body.X1 = _lapin.PosX;
            body.Y1 = _lapin.PosY;
            body.X2 = _lapin.PosX - 40 * _lapin.SpeedX;
            body.Y2 = _lapin.PosY - 40 * _lapin.SpeedY;
            oceanCanvas.Children.Add(body);
        }

        /// <summary>
        ///     Draws the renard.
        /// </summary>
        /// <param name="_renard">The _renard.</param>
        private void DrawRenard(RenardAgent _renard)
        {
            var body = new Line();
            //body.Stroke = Brushes.Red;
            body.StrokeThickness = 40;

            //body.Height = 40;
            //body.Width = 40;
            //body.Stroke = new 


            body.Stroke = new ImageBrush(new BitmapImage(new Uri(@"../../renard.png", UriKind.Relative)));
            //body.Fill = new ImageBrush(new BitmapImage(new Uri(@"../../back1.png",UriKind.Relative)));
            body.X1 = _renard.PosX;
            body.Y1 = _renard.PosY;
            body.X2 = _renard.PosX - 40 * _renard.SpeedX;
            body.Y2 = _renard.PosY - 40 * _renard.SpeedY;
            oceanCanvas.Children.Add(body);
        }

        /// <summary>
        ///     Handles the MouseDown event of the oceanCanvas control.
        /// </summary>
        /// <param name="_sender">The source of the event.</param>
        /// <param name="_mouseEvent">The <see cref="MouseButtonEventArgs" /> instance containing the event data.</param>
        private void oceanCanvas_MouseDown(object _sender, MouseButtonEventArgs _mouseEvent)
        {
            myOcean.AddObstacle(_mouseEvent.GetPosition(oceanCanvas).X,
                _mouseEvent.GetPosition(oceanCanvas).Y, 10);
        }
    }
}