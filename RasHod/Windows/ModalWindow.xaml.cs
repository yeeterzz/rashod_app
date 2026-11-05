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
using System.Windows.Shapes;

namespace RasHod.Windows
{
    /// <summary>
    /// Логика взаимодействия для ModalWindow.xaml
    /// </summary>
    public partial class ModalWindow : Window
    {
        private Rashod contextRashod;
        public ModalWindow(Rashod rashod)
        {
            InitializeComponent();

            contextRashod = rashod;
            DataContext = contextRashod;

            cbCategory.ItemsSource = App.DB.Categories.ToList();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string errorMessage = "";

            if (string.IsNullOrWhiteSpace(contextRashod.Title))
            {
                errorMessage += "Название не может быть пустым.\n";
            }
            else if (contextRashod.Title.Length > 200)
            {
                errorMessage += "Название должно быть не более 200 символов.\n";
            }

            if (!decimal.TryParse(tbAmount.Text.Replace(",", "."), out decimal price) || price <= 0)
            {
                errorMessage += "Сумма должна быть положительным числом.\n";
            }

            if (contextRashod.Date == null)
            {
                errorMessage += "Дата должна быть заполнена.\n";
            }
            else if (contextRashod.Date > DateTime.Today)
            {
                errorMessage += "Дата не может быть позже сегодняшнего дня.\n";
            }

            if (contextRashod.CategoryId == 0)
            {
                errorMessage += "Необходимо выбрать категорию.\n";
            }

            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                MessageBox.Show(errorMessage, "Ошибка валидации", MessageBoxButton.OK);
                return;
            }

            contextRashod.Amount = price;
            if (contextRashod.id == 0)
            {
                App.DB.Rashod.Add(contextRashod);
            }
            DialogResult = true;
            Close();
        }
    }
}
