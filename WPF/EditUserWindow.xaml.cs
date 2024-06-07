using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Principal;
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
    /// Логика взаимодействия для EditUserWindow.xaml
    /// </summary>
    [ObservableObject]
    public partial class EditUserWindow : Window
    {
        [ObservableProperty]
        public User _current;

        [ObservableProperty]
        public Student? _student = null;

        public bool IsNew { get; set; }

        [ObservableProperty]
        public Student? _initialStudent;
        
        public string PasswordLabelContent
        {
            get => IsNew ? "Пароль" : "Смена пароля";
        }

        public bool IsNotNew
        {
            get => !IsNew;
        }

        public EditUserWindow(User? user = null)
        {
            InitializeComponent();
            DataContext = this;

            IsNew = user is null;
            Current = user ?? new User();

            InitialRefreshStudent();            
        }

        private async void InitialRefreshStudent()
        {
            await RefreshStudent();
            InitialStudent = Student;
        }

        private async Task RefreshStudent()
        {
            try
            {
                ChangeStudent(await SessionManager.getInstance().ResolveClient().GetFromJsonAsync<Student>($"/account/{Current.Id}/student"));
            }
            catch
            {

            }
        }

        private async void UpdatePasswordButton_Click(object sender, RoutedEventArgs e)
        {            
            try
            {
                HttpResponseMessage resp = await SessionManager.getInstance().ResolveClient().PostAsync(
                    "/account/setpassword",
                    JsonContent.Create(
                        new
                        {
                            login = Current.Login,
                            newPassword = passwordTextBox.Password
                        }
                    )
                );
                if (resp.IsSuccessStatusCode)
                {
                    MessageBox.Show("Пароль успешно изменен");
                    passwordTextBox.Password = String.Empty;
                }
                else
                {
                    throw new ArgumentException();
                }
                
            } 
            catch
            {
                MessageBox.Show("Не удалось изменить пароль");
            }
            
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (IsNotNew)
                {
                    HttpResponseMessage resp = await SessionManager.getInstance().ResolveClient().DeleteAsync($"/account/{Current.Id}");
                    if (!resp.IsSuccessStatusCode)
                    {
                        throw new ArgumentException();
                    }
                }

                MessageBox.Show("Аккаунт успешно удален.");
                this.Close();
            }
            catch
            {
                MessageBox.Show("Не удалось удалить пользователя");
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (IsNew)
                {
                    HttpResponseMessage createResponse = await SessionManager.getInstance().ResolveClient().PostAsync(
                        "/account",
                        JsonContent.Create(Current.Login)
                    );

                    User created = await createResponse.Content.ReadFromJsonAsync<User>();

                    if (Student is not null)
                    {
                        HttpResponseMessage changeAccountResponse = await SessionManager.getInstance().ResolveClient().PutAsync(
                            $"/student/{Student.Id}/account/{created.Id}", null
                        );
                    }

                    if (!createResponse.IsSuccessStatusCode)
                    {
                        throw new ArgumentException();
                    }
                }                
                else
                {
                    if (Student != InitialStudent)
                    {
                        if (Student is null)
                        {
                            HttpResponseMessage changeAccountResponse = await SessionManager.getInstance().ResolveClient().PutAsync(
                                $"/account/resetstudent/{Current.Id}", null
                            );

                            if (!changeAccountResponse.IsSuccessStatusCode)
                            {
                                throw new ArgumentException();
                            }
                        }
                        else
                        {
                            HttpResponseMessage changeAccountResponse = await SessionManager.getInstance().ResolveClient().PutAsync(
                                $"/student/{Student.Id}/account/{Current.Id}", null
                            );

                            if (!changeAccountResponse.IsSuccessStatusCode)
                            {
                                throw new ArgumentException();
                            }
                        }                        
                    }
                }
                MessageBox.Show("Аккаунт успешно сохранен");
                this.Close();
            }
            catch
            {
                MessageBox.Show("Не удалось сохранить аккаунт");
            }
        }

        private void SetStudentButton_Click(object sender, RoutedEventArgs e)
        {
            BrowseStudentWindow popUp = new BrowseStudentWindow();

            var result = popUp.ShowDialog();

            ChangeStudent(popUp.SelectedStudent);            
        }

        private void ChangeStudent(Student other)
        {
            Student = other;
            Current.Role = other is null ? "teacher" : "student";
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs("Current"));
        }
    }
}
