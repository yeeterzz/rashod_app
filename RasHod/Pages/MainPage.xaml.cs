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
using RasHod.Pages;
using RasHod.Windows;
using RasHod.Models;

namespace RasHod.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var categories = App.DB.Categories.ToList();

            // Исправлено: имя класса создаем в соответствии с твоей таблицей (Categories)
            categories.Insert(0, new Categories { id = 0, name = "Все" });

            CBCategory.ItemsSource = categories;
            CBCategory.SelectedIndex = 0;

            Refresh();
        }

        private void Refresh()
        {

            var query = App.DB.Rashod.AsQueryable();

            if (!string.IsNullOrWhiteSpace(TBSearch.Text))
            {
                string search = TBSearch.Text.ToLower();
                query = query.Where(x => x.Title.ToLower().Contains(search) ||
                                        (x.Comment != null && x.Comment.ToLower().Contains(search)));
            }

            if (CBCategory.SelectedValue != null && (int)CBCategory.SelectedValue != 0)
            {
                int catId = (int)CBCategory.SelectedValue;
                query = query.Where(x => x.CategoryId == catId);
            }

            if (DT1.SelectedDate.HasValue)
            {
                query = query.Where(x => x.Date >= DT1.SelectedDate.Value);
            }
            if (DT2.SelectedDate.HasValue)
            {
                query = query.Where(x => x.Date <= DT2.SelectedDate.Value);
            }

            if (CHBCurrMonth.IsChecked == true)
            {
                DateTime today = DateTime.Today;
                DateTime startOfMonth = new DateTime(today.Year, today.Month, 1);
                query = query.Where(x => x.Date >= startOfMonth && x.Date <= today);
            }

            var resultList = query.OrderByDescending(x => x.Date).ToList();

            DGRashod.ItemsSource = resultList;

            TBCount.Text = resultList.Count.ToString();
            TBSum.Text = resultList.Sum(x => x.Amount).ToString("N2") + " руб.";
        }

        private void CBCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Refresh();
        }

        private void TBSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Refresh();
        }

        private void DT1_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            Refresh();
        }

        private void DT2_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            Refresh();
        }

        private void CHBCurrMonth_Checked(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void DGRashod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void BTAdd_Click(object sender, RoutedEventArgs e)
        {
            var newRashod = new Rashod { Date = DateTime.Today };

            ModalWindow win = new ModalWindow(newRashod);
            win.Owner = Window.GetWindow(this); 

            if (win.ShowDialog() == true)
            {
                App.DB.SaveChanges(); 
                Refresh();
            }
        }

        private void BTEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = DGRashod.SelectedItem as Rashod;

            if (selected == null)
            {
                MessageBox.Show("Выберите запись для редактирования!", "Внимание", MessageBoxButton.OK);
                return;
            }

            ModalWindow win = new ModalWindow(selected);
            win.Owner = Window.GetWindow(this);

            if (win.ShowDialog() == true)
            {
                App.DB.SaveChanges();
                Refresh(); 
            }
        }

        private void BTDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = DGRashod.SelectedItem as Rashod;
            if (selected == null)
            {
                MessageBox.Show("Выберите запись для удаления!", "Внимание", MessageBoxButton.OK);
                return;
            }

            var result = MessageBox.Show($"Удалить расход '{selected.Title}'?", "Подтверждение", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                App.DB.Rashod.Remove(selected);
                App.DB.SaveChanges();
                Refresh();
            }
        }
    }
}
