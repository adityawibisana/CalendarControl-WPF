using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Calendar
{
    /// <summary>
    /// Interaction logic for CalendarControl.xaml
    /// </summary>
    public partial class CalendarControl : UserControl, INotifyPropertyChanged
    {
        MouseButtonEventHandler OutsideHandler;
        public ObservableCollection<CalendarData> CalendarList { get; set; }
        private DateTime _selectedDate;
        public DateTime SelectedDate
        {
            get
            {
                return _selectedDate;
            }
            set
            {
                _selectedDate = value;
                RaisePropertyChanged("SelectedDate");
            }
        }

        public CalendarControl()
        {
            InitializeComponent();
            OutsideHandler = new MouseButtonEventHandler(HandleClickOutsideOfControl);
            this.DataContext = this;

            SelectedDate = DateTime.Now;
            InitCalendarData(SelectedDate);
        }

        private void InitCalendarData(DateTime beginDate)
        {
            CalendarList = new ObservableCollection<CalendarData>();

            // first to show is date 1
            beginDate = beginDate.Subtract(TimeSpan.FromDays(beginDate.Day - 1));
            //get date 1
            beginDate = beginDate.Subtract(TimeSpan.FromDays(beginDate.DayOfWeek - DayOfWeek.Monday));
            //substract with 7, to get 7 day before
            beginDate = beginDate.Subtract(TimeSpan.FromDays(7));

            for (int i = 0; i < 49; i++)
            {
                DateTime date = beginDate.AddDays(i); 
                CalendarList.Add(new CalendarData() { CurrentDateTime = date, ParentDateTime = SelectedDate, IsSelected = SelectedDate == date }); 
            }
            CalendarListBox.ItemsSource = CalendarList; 
        } 

        private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                SelectedDate = SelectedDate.AddMonths(-1); 
            }
            else
            {
                SelectedDate = SelectedDate.AddMonths(1);
            }
            InitCalendarData(SelectedDate);
        } 

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 0) { 
                foreach (CalendarData cal in CalendarList)
                {
                    cal.IsSelected = false;
                } 
            } 

            foreach (CalendarData cal in e.AddedItems)
            {
                cal.IsSelected = true;
                SelectedDate = cal.CurrentDateTime;
                Storyboard s = (Storyboard)TryFindResource("CollapseCalendarStoryboard");
                s.Begin();
                break;
            } 
        }
         
        public void CollapseCalendar()
        {
            if (CalendarGridContainer.Visibility == Visibility.Collapsed)
                return;

            Storyboard s = (Storyboard)TryFindResource("CollapseCalendarStoryboard");
            s.Begin();

            RemoveHandler(Mouse.PreviewMouseDownOutsideCapturedElementEvent, OutsideHandler); 
            ReleaseMouseCapture();
        }

        private void OnCalendarClicked(object sender, MouseButtonEventArgs e)
        {
            if (CalendarGridContainer.Visibility == Visibility.Visible)
            {
                CollapseCalendar();
            }
            else 
            {
                Storyboard s = (Storyboard)TryFindResource("ExpandCalendarStoryboard");
                s.Begin();

                AddMouseCapture(); 
            }  
        }

        private void AddMouseCapture()
        {
            Mouse.Capture(this, CaptureMode.SubTree);
            AddHandler(); 
        }

        private void AddHandler()
        { 
             AddHandler(Mouse.PreviewMouseDownOutsideCapturedElementEvent, OutsideHandler, true); 
        }

        private void HandleClickOutsideOfControl(object sender, MouseButtonEventArgs e)
        {
            CollapseCalendar(); 
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            // take a copy to prevent thread issues
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        } 

    }

    public class CalendarData : INotifyPropertyChanged
    {
        public DateTime ParentDateTime { get; set; }
        public Brush ForegroundColor
        {
            get
            {
                return (ParentDateTime.Month == CurrentDateTime.Month || IsSelected) ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Gray);
            }
        }
        public bool IsShowLeftBorder
        {
            get
            {
                return CurrentDateTime.Day == 1;
            }
        }
        public bool IsShowTopBorder
        {
            get
            {
                return CurrentDateTime.Day <= 7;
            }
        }
        private bool _isSelected;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                RaisePropertyChanged("IsSelected");
                RaisePropertyChanged("ShowMonth");
                RaisePropertyChanged("HideMonth");
                RaisePropertyChanged("ForegroundColor");
            }
        }
        public bool HideMonth
        {
            get
            {
                return !ShowMonth;
            }
        }
    
        public bool ShowMonth
        {
            get
            {
                return CurrentDateTime.Day == 1 || IsSelected;
            }
        }
        private DateTime _currentDateTime;
        public DateTime CurrentDateTime
        {
            get
            {
                return _currentDateTime;
            }
            set
            {
                _currentDateTime = value;
            }
        }
        public String CurrentMonth
        {
            get
            {
                return CurrentDateTime.ToString("MMM", new CultureInfo("en-GB"));
            }
        }
        public int CurrentDate
        {
            get
            {
                return CurrentDateTime.Day;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            // take a copy to prevent thread issues
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        } 
        
    } 
}
