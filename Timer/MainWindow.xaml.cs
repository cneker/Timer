using System;
using System.ComponentModel;
using System.Media;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;

namespace Timer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private DispatcherTimer _timer;
        private SoundPlayer _player;

        private bool _isStarted = false;
        private bool _isPaused = false;
        private bool _isPauseResumeButtonEnabled = false;
        private bool _isStartStopButtonEnabled = false;

        private TimeSpan _allTime;
        private DateTime _startTime;
        private TimeSpan _timeToEnd;
        private DateTime _pauseTime;
        
        public string Time
        {
            get => _timeToEnd.ToString("hh\\:mm\\:ss");
        }

        public string Status
        {
            get
            {
                string status = string.Empty;
                if (_isStarted && _isPaused)
                    status = "Paused";
                if (_isStarted && !_isPaused)
                    status = "Started";
                if (!_isStarted)
                    status = "Stopped";
                return status;
            }
        }

        public string AllTime
        {
            get => _allTime.ToString();
            set
            {
                if (TimeSpan.TryParse(value, out TimeSpan allTime))
                {
                    _allTime = allTime;
                    if (_allTime <= TimeSpan.Zero)
                    {
                        IsStartStopButtonEnabled = false;
                    }
                    else
                        IsStartStopButtonEnabled = true;
                }
                else
                    _allTime = TimeSpan.FromMinutes(60);
                TimeToEnd = _allTime;
                OnPropertyChanged();
            }
        }

        public TimeSpan TimeToEnd
        {
            get => _timeToEnd;
            set
            {
                if (value < TimeSpan.Zero)
                {
                    StopTimer();
                    _player.Play();
                }
                else
                    _timeToEnd = value;
                OnPropertyChanged("Time");
            }
        }

        public bool IsPauseResumeButtonEnabled
        {
            get => _isPauseResumeButtonEnabled;
            set
            {
                _isPauseResumeButtonEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool IsStartStopButtonEnabled
        {
            get => _isStartStopButtonEnabled;
            set
            {
                _isStartStopButtonEnabled = value;
                OnPropertyChanged();
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            _player = new SoundPlayer(@"media\stopsound.wav");
            _timer = new DispatcherTimer();
            _timer.Tick += delegate
            {
                var now = DateTime.Now;
                var sub = now.Subtract(_startTime);
                TimeToEnd = _allTime.Subtract(sub);
            };
            _timer.Interval = TimeSpan.FromMilliseconds(15);
            AllTime = TimeSpan.Zero.ToString();
            TimeToEnd = _allTime;
            DataContext = this;
        }

        private void Stop_Start(object sender, RoutedEventArgs e)
        {
            if(_isStarted)
            {
                StopTimer();
            }
            else
            {
                Start();
            }
        }

        private void Pause_Resume(object sender, RoutedEventArgs e)
        {
            if (_isPaused)
            {
                var sub = DateTime.Now.Subtract(_pauseTime);
                _startTime = _startTime.Add(sub);
                _timer.Start();
            }
            else
            {
                _timer.Stop();
                _pauseTime = DateTime.Now;
            }
            _isPaused = !_isPaused;
            OnPropertyChanged("Status");
        }

        private void Start()
        {
            _startTime = DateTime.Now;
            _timer.Start();
            IsPauseResumeButtonEnabled = true;
            _isStarted = true;
            OnPropertyChanged("Status");
        }
        private void StopTimer()
        {
            _timer.Stop();
            _isPaused = false;
            _isStarted = false;
            IsPauseResumeButtonEnabled = false;
            IsStartStopButtonEnabled = false;
            AllTime = TimeSpan.Zero.ToString();
            OnPropertyChanged("Status");
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
