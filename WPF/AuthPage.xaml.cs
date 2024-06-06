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
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        public string InstanceAPI { get; set; }

        public string Login { get; set; }

        public AuthPage()
        {
            InitializeComponent();
            DataContext = this;            
        }

        private async void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            string password = passwordTextBox.Password;
            SessionManager sessionManager = SessionManager.getInstance();

            try
            {
                using HttpClient protocol = new HttpClient();

                protocol.BaseAddress = new Uri(InstanceAPI);
                HttpResponseMessage response = await protocol.PostAsync($"/Account/token/{Login}/{password}", null);
                AuthResponse? authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();

                if (response.IsSuccessStatusCode && authResponse is not null)
                {
                    authResponse.InstanceAPI = InstanceAPI;
                    sessionManager.AuthResponse = authResponse;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    MessageBox.Show("Неправильный пароль или логин");
                }
            }
            catch
            {
                MessageBox.Show("Непредвиденная ошибка");
            }


            if (sessionManager.AuthResponse is not null)
            {
                if (sessionManager.AuthResponse.Username == "admin")
                {
                    sessionManager.Frame.Navigate(new UsersPage());
                } 
                else
                {
                    switch (sessionManager.AuthResponse.Role)
                    {
                        case "student":
                            //sessionManager.Frame.Navigate(new UsersPage());
                            break;

                        case "teacher":
                            //sessionManager.Frame.Navigate(new UsersPage());
                            break;

                        default:
                            throw new Exception($"Unknown role '{sessionManager.AuthResponse.Role}'");
                    }
                }               
            }                       
        }
    }
}
