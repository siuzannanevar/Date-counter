using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DatesCounter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<string, DateTime> holidays = new Dictionary<string, DateTime>();

        public MainWindow()
        {
            InitializeComponent(); 

            try
            {
                datepicker1.SelectedDate = DateTime.Now.Date;
                datepicker2.SelectedDate = DateTime.Now.Date;
                holidayDatePicker.SelectedDate = DateTime.Now.Date;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"UI elements not found: {ex.Message}", "Warning",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            LoadDefaultHolidays();
        }

        private void LoadDefaultHolidays()
        {
            DateTime now = DateTime.Now.Date;

            DateTime newYear = new DateTime(now.Year, 1, 1);
            if (newYear < now) newYear = newYear.AddYears(1);
            holidays["New Year (January 1)"] = newYear;

            DateTime xmas = new DateTime(now.Year, 12, 25);
            if (xmas < now) xmas = xmas.AddYears(1);
            holidays["Christmas (December 25)"] = xmas;

            DateTime helloween = new DateTime(now.Year, 10, 31);
            if (helloween < now) helloween = helloween.AddYears(1);
            holidays["Halloween (October 31)"] = helloween;

            UpdateHolidayCombobox();
        }

        private void UpdateHolidayCombobox()
        {
            holidayCombobox.Items.Clear();
            foreach (var name in holidays.Keys)
            {
                holidayCombobox.Items.Add(name);
            }
            if (holidayCombobox.Items.Count > 0)
                holidayCombobox.SelectedIndex = 0;
        }

        private void CalculateDifference_Click(object sender, RoutedEventArgs e)
        {
            if (!datepicker1.SelectedDate.HasValue || !datepicker2.SelectedDate.HasValue)
            {
                MessageBox.Show("Please, choose 2 dates", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime date1 = datepicker1.SelectedDate.Value.Date;
            DateTime date2 = datepicker2.SelectedDate.Value.Date;

            DateTime earlierDate = date1 < date2 ? date1 : date2;
            DateTime laterDate = date1 < date2 ? date2 : date1;

            if (earlierDate == laterDate)
            {
                resultText.Text = "The difference is 0 days";
                return;
            }

            int totalDays = (laterDate - earlierDate).Days;

            int years = 0;
            DateTime tempDate = earlierDate;
            while (tempDate.AddYears(1) <= laterDate)
            {
                years++;
                tempDate = tempDate.AddYears(1);
            }

            DateTime afterYears = earlierDate.AddYears(years);
            int months = 0;
            tempDate = afterYears;
            while (tempDate.AddMonths(1) <= laterDate)
            {
                months++;
                tempDate = tempDate.AddMonths(1);
            }

            DateTime afterMonths = afterYears.AddMonths(months);
            int remainingDays = (laterDate - afterMonths).Days;

            resultText.Text = $"Difference is {years} years, {months} months, {remainingDays} days.";
            resultText.Text += $"\n(Total: {totalDays} days)";
        }

        private void HolidayCalculate_Click(object sender, RoutedEventArgs e)
        {
            if (holidayCombobox.SelectedItem is string name)
            {
                ShowDaysUntilHoliday(name);
            }
            else
            {
                MessageBox.Show("Please select a holiday first", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ShowDaysUntilHoliday(string name)
        {
            if (!holidays.ContainsKey(name)) return;

            DateTime now = DateTime.Now.Date;
            DateTime stored = holidays[name];

            DateTime next = new DateTime(now.Year, stored.Month, stored.Day);
            if (next < now) next = next.AddYears(1);

            int daysLeft = (next - now).Days;
            holidayResultText.Text = $"Before '{name}' {daysLeft} days left";
        }

        private void AddHoliday_Click(object sender, RoutedEventArgs e)
        {
            string name = holidayTextBox.Text.Trim();
            if (string.IsNullOrEmpty(name) || !holidayDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Enter the name of the holiday and choose the date.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (holidays.ContainsKey(name))
            {
                MessageBox.Show($"Holiday '{name}' already exists. Please choose another name.", "Duplicate",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            DateTime picked = holidayDatePicker.SelectedDate.Value.Date;
            DateTime now = DateTime.Now.Date;
            DateTime next = new DateTime(now.Year, picked.Month, picked.Day);
            if (next < now) next = next.AddYears(1);

            holidays[name] = next;
            UpdateHolidayCombobox();

            holidayCombobox.SelectedItem = name;
            holidayTextBox.Clear();

            MessageBox.Show($"Holiday \"{name}\" is added.", "Success",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}