using System;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Interop;
using Firebase.Database;
using System.Reactive.Linq;
using Firebase.Database.Query;

namespace TCATimerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FirebaseClient fbClient;
        private Timer _currentTimer;
        private System.Timers.Timer sysTimer;
        public int RemainingHours = 0;
        public int RemainingMinutes = 0;
        public int RemainingSeconds = 0;

        private Timer currentTimer
        {
            get { return _currentTimer; }
            set
            {
                _currentTimer = value;
                this.Dispatcher.Invoke(() =>
                {
                    recalculateTimer();
                });
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            SystemEvents.DisplaySettingsChanged += new EventHandler(SystemEvents_DisplaySettingsChanged);
            this.Loaded += new RoutedEventHandler(Window_Loaded);
        }

        private void recalculateTimer()
        {
            // Show timer when we have data
            calculateNewTime();
            TimerGrid.Visibility = Visibility.Visible;

            // Setup Timer
            sysTimer = new System.Timers.Timer(); // fire every second
            sysTimer.Interval = GetInterval();
            sysTimer.Elapsed += SysTimer_Elapsed;
            sysTimer.AutoReset = false;
            sysTimer.Start();
        }

        static double GetInterval()
        {
            DateTime now = DateTime.Now;
            return 1000 - now.Millisecond;
        }


        private void SysTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            calculateNewTime();
            sysTimer.Interval = GetInterval();
            sysTimer.Start();
        }

        private void calculateNewTime()
        {
            // Calculate remaining time
            var remainingTime = currentTimer.endTimeToDateTime() - DateTime.UtcNow;
            RemainingHours = Math.Max(0, remainingTime.Hours);
            if (remainingTime.Days > 0)
            {
                RemainingHours += Math.Max(1, remainingTime.Days * 24);
            }
            RemainingMinutes = Math.Max(0, remainingTime.Minutes);
            RemainingSeconds = Math.Max(0, remainingTime.Seconds);
            this.Dispatcher.Invoke(() => { renderTimerUpdates(); });
        }

        private void renderTimerUpdates()
        {
            HourTime.Text = RemainingHours.ToString();
            MinuteTime.Text = RemainingMinutes.ToString();
            SecondTime.Text = RemainingSeconds.ToString();

            // Update display
            if (RemainingHours > 0)
            {
                secondsVisible(false);
                minutesVisible(true);
                hoursVisible(true);
            }
            else
            {
                secondsVisible(true);
                minutesVisible(true);
                hoursVisible(false);
            }
        }
        
        private void SystemEvents_DisplaySettingsChanged(object? sender, EventArgs e)
        {
            setWindowPosition();
        }

        private void setupFirebaseConnection()
        {
            var timerId = Properties.Settings.Default.TimerId;
            fbClient = new FirebaseClient(Properties.Settings.Default.DatabaseUrl);
            var observable = fbClient.Child("timers").AsObservable<Timer>();
            var subscription = observable.Where(f => f.Key == timerId).Subscribe(d => currentTimer = d.Object);
        }

        private void refreshServerTimeOffset()
        {
            var serverTime = fbClient.Child(".info/serverTimeOffset").OnceAsync<Object>();
            serverTime.Wait();
            MessageBox.Show(serverTime.Result.ToString());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Hide timer to start
            TimerGrid.Visibility = Visibility.Hidden;
            setWindowPosition();
            setupFirebaseConnection();
        }

        private void setWindowPosition()
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom - this.Height;
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            Win32Utils.SetWindowExTransparent(hwnd);
        }

        private void hoursVisible(bool visible)
        {
            if (visible)
            {
                HourLabel.Visibility = Visibility.Visible;
                HourTime.Visibility = Visibility.Visible;
            }
            else
            {
                HourLabel.Visibility = Visibility.Collapsed;
                HourTime.Visibility = Visibility.Collapsed;
            }
        }

        private void minutesVisible(bool visible)
        {
            if (visible)
            {
                MinuteLabel.Visibility = Visibility.Visible;
                MinuteTime.Visibility = Visibility.Visible;
            }
            else
            {
                MinuteLabel.Visibility = Visibility.Collapsed;
                MinuteTime.Visibility = Visibility.Collapsed;
            }
        }

        private void secondsVisible(bool visible)
        {
            if (visible)
            {
                SecondLabel.Visibility = Visibility.Visible;
                SecondTime.Visibility = Visibility.Visible;
            }
            else
            {
                SecondLabel.Visibility = Visibility.Collapsed;
                SecondTime.Visibility = Visibility.Collapsed;
            }
        }
    }
}
