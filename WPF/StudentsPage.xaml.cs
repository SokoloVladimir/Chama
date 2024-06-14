using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
    /// Логика взаимодействия для StudentsPage.xaml
    /// </summary>
    [ObservableObject]
    public partial class StudentsPage : Page
    {
        [ObservableProperty]   
        List<Student> _students = new List<Student>();

        [ObservableProperty]
        Student _selectedStudent;

        [ObservableProperty]
        Group _group;

        public StudentsPage(Group group)
        {
            InitializeComponent();
            DataContext = this;

            Group = group;
            Refresh();
        }

        public async Task Refresh()
        {
            try
            {
                Students = null;
                Students = await SessionManager.Instance.ResolveClient().GetFromJsonAsync<List<Student>>($"student?groupId={Group.Id}");
            }
            catch
            {
                MessageBox.Show("Ошибка обновления");
            }
        }

        private async void DataGrid_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedStudent is null)
            {
                return;
            }

            Window window = new EditStudentWindow(Group, SelectedStudent);
            window.Closed += PopUpWindow_Closed;
            window.ShowDialog();
        }

        private async void NewRecordButton_Click(object sender, RoutedEventArgs e)
        {
            Window window = new EditStudentWindow(Group);
            window.Closed += PopUpWindow_Closed;
            window.ShowDialog();            
        }

        private async void PopUpWindow_Closed(object? sender, EventArgs e)
        {
            await Refresh();
        }

        private void NavExportButton_Click(object sender, RoutedEventArgs e)
        {
            SessionManager.Instance.Navigate(new ExportPage());
        }
    }
}
