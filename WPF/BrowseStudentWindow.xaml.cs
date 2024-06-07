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
using System.Windows.Shapes;
using WPF.Model;

namespace WPF
{
    /// <summary>
    /// Логика взаимодействия для BrowseStudentWindow.xaml
    /// </summary>
    public partial class BrowseStudentWindow : Window
    {
        public List<Group> Groups { get; set; } = new List<Group>();

        public List<Student> Students { get; set; } = new List<Student>();

        public Group SelectedGroup { get; set; }

        public Student SelectedStudent { get; set; }

        public BrowseStudentWindow()
        {
            InitializeComponent();
            DataContext = this;

            Refresh();
        }

        private async void Refresh()
        {
            try
            {
                Groups.Clear();
                Groups.AddRange(await SessionManager.getInstance().ResolveClient().GetFromJsonAsync<List<Group>>("/group"));
            }
            catch
            {

            }            
        }

        private async void GroupComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Students.Clear();
                Students.AddRange(await SessionManager.getInstance().ResolveClient().GetFromJsonAsync<List<Student>>($"/student?groupId={SelectedGroup.Id}"));
            }
            catch
            {

            }
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
