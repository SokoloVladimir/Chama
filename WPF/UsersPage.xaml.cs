using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
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
using WPF.Model;

namespace WPF
{
    /// <summary>
    /// Логика взаимодействия для UsersPage.xaml
    /// </summary>
    [ObservableObject]
    public partial class UsersPage : Page
    {
        [ObservableProperty]
        public List<User> _users = new List<User>();

        public User Selected { get; set; }

        public UsersPage()
        {
            InitializeComponent();
            DataContext = this;

            Refresh();
        }

        private async void Refresh()
        {
            Users = null;
            Users = await SessionManager.Instance.ResolveClient().GetAsync("/account").Result.Content.ReadFromJsonAsync<List<User>>();

            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs("Users"));
        }

        private void DataGrid_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Selected is null)
            {
                return;
            }

            var result = new EditUserWindow(Selected).ShowDialog();
            Refresh();
        }

        private void NewRecordButton_Click(object sender, RoutedEventArgs e)
        {
            var result = new EditUserWindow().ShowDialog();
            Refresh();
        }
    }
}
