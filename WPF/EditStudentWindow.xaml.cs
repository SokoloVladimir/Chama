using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using WPF.Model;

namespace WPF
{
    /// <summary>
    /// Логика взаимодействия для EditStudentWindow.xaml
    /// </summary>
    [ObservableObject]
    public partial class EditStudentWindow : Window
    {
        public bool IsNew { get; set; }

        [ObservableProperty]
        private Student _current;

        public EditStudentWindow(Group group, Student? student = null)
        {
            InitializeComponent();

            IsNew = student is null;
            Current = student ?? new Student()
            {
                IsDeleted = false,
                GroupId = group.Id,
                Group = group
            };

            DataContext = this;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (IsNew)
                {
                    this.Close();
                }
                else
                {
                    SessionManager.Instance.ResolveClient().DeleteAsync($"student/{Current.Id}");
                }

                MessageBox.Show("Студент успешно удален");
            }
            catch
            {
                MessageBox.Show("Ошибка удаления студента");                
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (IsNew)
                {
                    var s = await SessionManager.Instance.ResolveClient().PostAsJsonAsync("student", Current);
                    MessageBox.Show("Студент успешно добавлен");
                }
                else
                {
                    SessionManager.Instance.ResolveClient().PutAsJsonAsync("student", Current);
                    MessageBox.Show("Студент успешно сохранен");
                }                
            } 
            catch 
            {
                if (IsNew)
                {
                    MessageBox.Show("Ошибка добавления студента");
                }
                else
                {
                    MessageBox.Show("Ошибка сохранения студент");
                }                
            }
        }
    }
}
