using System;
using System.ComponentModel;
using System.Media;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
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
        private string _status = "Stopped";

        private bool _isStarted = false;
        private bool _isPaused = false;
        private bool _isPauseResumeButtonEnabled = false;
        private bool _isStartStopButtonEnabled = false;
        private Visibility _isTimeVIsible = Visibility.Visible;

        private TimeSpan _allTime;
        private DateTime _startTime;
        private TimeSpan _timeToEnd;
        private DateTime _pauseTime;
        
        public string Time
        {
            get => _timeToEnd.ToString();
        }

        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

        public string AllTime
        {
            get => _allTime.ToString();
            set
            {
                if (TimeSpan.TryParse(value, out TimeSpan allTime) && CheckForValidTimeFormat(value))
                {
                    _allTime = allTime;
                    Status = "Stopped";
                }
                else
                {
                    _allTime = TimeSpan.Zero;
                    Status = "Invalid time";
                }
                if (_allTime > TimeSpan.Zero)
                    IsStartStopButtonEnabled = true;
                else
                    IsStartStopButtonEnabled = false;
                TimeToEnd = _allTime;
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

        public Visibility IsTimeVisible
        {
            get => _isTimeVIsible;
            set
            {
                _isTimeVIsible = value;
                OnPropertyChanged();
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
                Status = "Started";
            }
            else
            {
                _timer.Stop();
                _pauseTime = DateTime.Now;
                Status = "Paused";
            }
            _isPaused = !_isPaused;
            OnPropertyChanged("Status");
        }

        private void Start()
        {
            _startTime = DateTime.Now;
            _timer.Start();
            IsPauseResumeButtonEnabled = true;
            IsTimeVisible = Visibility.Hidden;
            _isStarted = true;
            Status = "Started";
        }
        private void StopTimer()
        {
            _timer.Stop();
            _isPaused = false;
            _isStarted = false;
            IsPauseResumeButtonEnabled = false;
            IsStartStopButtonEnabled = false;
            IsTimeVisible = Visibility.Visible;
            AllTime = TimeSpan.Zero.ToString();
            Status = "Stopped";
            OnPropertyChanged("AllTime");
        }

        private bool CheckForValidTimeFormat(string time) => Regex.IsMatch(time, @"\d{2}:\d{2}:\d{2}") && int.Parse(time.Split(':')[0]) < 24;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            AllTime = ((TextBox)sender).Text;
        }
    }
}
