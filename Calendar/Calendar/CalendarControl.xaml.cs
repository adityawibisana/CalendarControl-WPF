using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Calendar
{
    /// <summary>
    /// Interaction logic for CalendarControl.xaml
    /// </summary>
    public partial class CalendarControl : UserControl, INotifyPropertyChanged
    {
        public MouseButtonEventHandler OutsideHandler;
        public event SelectionChangedEventHandler SelectedDateChanged;
        public event EventHandler CalendarItemClicked;

        private CalendarMode _calendarMode;
        public CalendarMode CalendarMode
        {
            get
            {
                return _calendarMode;
            }
            set
            {

                _calendarMode = value;
                if (_calendarMode == CalendarMode.Normal)
                    VisualStateManager.GoToElementState(CalendarRootContainer, "NormalCalendarMode", true);
                else if (_calendarMode == CalendarMode.Month)
                    VisualStateManager.GoToElementState(CalendarRootContainer, "MonthCalendarMode", true);

            }
        }

        public ObservableCollection<DateTime> CalendarList { get; set; }
        public ObservableCollection<String> DayList { get; set; }
        public ObservableCollection<DateTime> MonthList { get; set; }

        private DateTime _selectedDate;
        public DateTime SelectedDate
        {
            get
            {
                if (_selectedDate == null)
                    return DateTime.Now;
                return _selectedDate;
            }
            set
            {
                _selectedDate = value;
                //if (_selectedDate.Month != value.Month || _selectedDate.Year != value.Year)
                //{
                //    _selectedDate = value;
                //    InitCalendarData(value);
                //}
                //else
                //{
                //    _selectedDate = value;
                //}
                RaisePropertyChanged("SelectedDate");
            }
        }

        public CalendarControl()
        {
            InitializeComponent();
            //OutsideHandler = new MouseButtonEventHandler(HandleClickOutsideOfControl);
            this.DataContext = this;

            #region init day list (sunday, etc.)
            DayList = new ObservableCollection<String>();
            // 3 may is sunday
            DateTime dt = new DateTime(2015, 5, 3);
            for (int i = 0; i < 7; i++)
            {
                DayList.Add(dt.AddDays(i).ToString("ddd"));
            }
            #endregion

            #region init month list (january, etc.)
            MonthList = new ObservableCollection<DateTime>();
            // begin from 1 january 2000
            dt = new DateTime(2000, 1, 1);
            for (int i = 0; i < 12; i++)
            {
                MonthList.Add(dt.AddMonths(i));
            }
            #endregion
            //CalendarMode = CalendarMode.Normal;  
            InitCalendarData(DateTime.Now);
            SelectedDate = DateTime.Now;
        }

        private void InitCalendarData(DateTime beginDate)
        {
            CalendarList = new ObservableCollection<DateTime>();

            // first to show is date 1
            beginDate = beginDate.Subtract(TimeSpan.FromDays(beginDate.Day - 1));
            //get date 1
            beginDate = beginDate.Subtract(TimeSpan.FromDays(beginDate.DayOfWeek - DayOfWeek.Monday));
            //substract with 7, to get 7 day before
            beginDate = beginDate.Subtract(TimeSpan.FromDays(7));

            for (int i = 0; i < 49; i++)
            {
                DateTime date = beginDate.AddDays(i);
                CalendarList.Add(date);
            }
            RaisePropertyChanged("CalendarList");
        }

        private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                //SelectedDate = SelectedDate.AddMonths(-1);
                var beginDate = CalendarList[0];
                for (int i = 0; i < 7; i++)
                {
                    DateTime date = beginDate.AddDays(-i-1);
                    CalendarList.Insert(0, date);
                    CalendarList.RemoveAt(CalendarList.Count-1);
                }
                RaisePropertyChanged("CalendarList");
            }
            else
            {
                //SelectedDate = SelectedDate.AddMonths(1); 
                var beginDate = CalendarList[CalendarList.Count - 1];
                for (int i = 0; i < 7; i++)
                {
                    DateTime date = beginDate.AddDays(i+1);
                    //CalendarList.Insert(CalendarList.Count-1, date);
                    CalendarList.Add(date);
                    CalendarList.RemoveAt(0);
                }
                RaisePropertyChanged("CalendarList");  
                //CalendarListBox.ScrollIntoView(CalendarList[CalendarList.Count - 1]);
            }
            UpdateLayout();
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.RemovedItems.Count > 0)
            {
                DateTime oldVal = (DateTime)e.RemovedItems[0];
                DateTime newVal = (DateTime)e.AddedItems[0]; 
                //if (oldVal.Month != newVal.Month || oldVal.Year != newVal.Year)
                //    InitCalendarData(SelectedDate);
            }

            if (SelectedDateChanged != null)
            {
                SelectedDateChanged(sender, e);
            }

            //foreach (CalendarData cal in e.AddedItems)
            //{
            //    cal.IsSelected = true;
            //    SelectedDate = cal.CurrentDateTime;
            //    Storyboard s = (Storyboard)TryFindResource("CollapseCalendarStoryboard");
            //    s.Begin();
            //    break;
            //} 
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
            }
        }

        private void AddMouseCapture()
        {
            Mouse.Capture(this, CaptureMode.SubTree);
            AddHandler();
        }

        private void AddHandler()
        {
            //AddHandler(Mouse.PreviewMouseDownOutsideCapturedElementEvent, OutsideHandler, true);
        }

        private void HandleClickOutsideOfControl(object sender, MouseButtonEventArgs e)
        {
            //CollapseCalendar();
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

        private void OnCalendarLoaded(object sender, RoutedEventArgs e)
        {
            //CalendarListBox.SelectedItem = DateTime.Now;
            //AddMouseCapture();
        }

        private void OnCalendarUnLoaded(object sender, RoutedEventArgs e)
        {
            //ReleaseMouseCapture();
            //RemoveHandler(Mouse.PreviewMouseDownOutsideCapturedElementEvent, OutsideHandler);
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Grid g = sender as Grid;
            if (g.DataContext is DateTime)
            {
                SelectedDate = (DateTime)g.DataContext;
            }

            if (CalendarItemClicked != null)
            {
                CalendarItemClicked(this, e);
            }
        }

        private void OnSwitchStateButtonClicked(object sender, MouseButtonEventArgs e)
        {
            SwitchCalendarState();
            e.Handled = true;
        }

        private void SwitchCalendarState()
        {
            switch (CalendarMode)
            {
                case CalendarMode.Normal:
                    CalendarMode = CalendarMode.Month;
                    break;
                case CalendarMode.Month:
                    CalendarMode = CalendarMode.Normal;
                    break;
                default:
                    break;
            }
        }

        private void MonthMouseDown(object sender, MouseButtonEventArgs e)
        {
            Grid g = sender as Grid;
            if (g.DataContext is DateTime)
            {
                DateTime dt = (DateTime)g.DataContext;
                SelectedDate = SelectedDate.AddMonths(-1 * SelectedDate.Month + dt.Month);
            }
            CalendarMode = CalendarMode.Normal;
        }
    }

    public enum CalendarMode
    {
        Normal,
        Month
    }
}
