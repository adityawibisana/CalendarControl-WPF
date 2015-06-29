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
        
        public ObservableCollection<DateTime> CalendarList { get; set; }
        public ObservableCollection<String> DayList { get; set; }

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

            #region init day list (sunday, etc.)
            DayList = new ObservableCollection<String>();
            // 3 may is sunday
            DateTime dt = new DateTime(2015, 5, 3);
            for (int i = 0; i < 7; i++)
            {
                DayList.Add(dt.AddDays(i).ToString("ddd"));
            }
            #endregion
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
                //CalendarList.Add(new CalendarData() { CurrentDateTime = date, ParentDateTime = SelectedDate, IsSelected = SelectedDate == date }); 
            }
            RaisePropertyChanged("CalendarList");
            //CalendarListBox.ItemsSource = CalendarList; 
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
            if (e.AddedItems.Count > 0 && e.RemovedItems.Count > 0)
            {
                DateTime oldVal = (DateTime)e.RemovedItems[0];
                DateTime newVal = (DateTime)e.AddedItems[0];

                if (oldVal.Month != newVal.Month)
                    InitCalendarData(SelectedDate);
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
             AddHandler(Mouse.PreviewMouseDownOutsideCapturedElementEvent, OutsideHandler, true); 
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
            CalendarListBox.SelectedItem = DateTime.Now;
            AddMouseCapture();
        }

        private void OnCalendarUnLoaded(object sender, RoutedEventArgs e)
        {
            ReleaseMouseCapture();
            RemoveHandler(Mouse.PreviewMouseDownOutsideCapturedElementEvent, OutsideHandler);
        } 

    }  
}
