using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPF.Model;

namespace WPF
{
    public class SessionManager
    {        
        private static SessionManager? _instance = null;
        private static readonly object cookie = "cookie";

        private Frame _frame = new Frame();

        private Window _mainWindow = new Window();
        private string _windowTitle = String.Empty;

        public AuthResponse? AuthResponse { get; set; } = null;

        public bool IsAuthorizied
        {
            get => this.AuthResponse is not null;
        }

        public static SessionManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (cookie)
                    {
                        if (_instance is null)
                        {
                            _instance = new SessionManager();
                        }
                    }
                }

                return _instance;
            }
        }

        public delegate void NavigationHanlder(object sender, NavEventArgs e);
        public event NavigationHanlder OnNavigation;

        /// <summary>
        /// Возможен ли переход вперёд
        /// </summary>        
        public bool CanGoForward
        {
            get => _frame.CanGoForward;
        }

        /// <summary>
        /// Возможен ли переход назад
        /// </summary>
        public bool CanGoBack
        {
            get => _frame.CanGoBack;
        }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        private SessionManager()
        {
            OnNavigation += UpdateWindowTitle;
        }
        
        /// <summary>
        /// Получить клиент с авторизацией
        /// </summary>
        /// <returns>авторизованный HTTP-клиент</returns>
        /// <exception cref="Exception">неавторизован</exception>
        public HttpClient ResolveClient()
        {
            if (AuthResponse is not null) 
            {
                HttpClient httpClient = new HttpClient()
                {
                    BaseAddress = new Uri(AuthResponse.InstanceAPI),
                };

                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {AuthResponse.AccessToken}");

                return httpClient;
            }
            else
            {
                throw new Exception("Not authorizied");
            }            
        }

        /// <summary>
        /// Навигироваться на страницу
        /// </summary>
        /// <param name="content"></param>
        public void Navigate(object content)
        {
            OnNavigation?.Invoke(this, new NavEventArgs(_frame.Content, content));
            _frame.Navigate(content);
        }

        /// <summary>
        /// Перейти вперёд
        /// </summary>
        public void GoForward()
        {
            if (CanGoForward)
            {
                object prevContent = _frame.Content;
                _frame.GoForward();
                OnNavigation?.Invoke(this, new NavEventArgs(prevContent, _frame.Content));
            }
        }

        /// <summary>
        /// Перейти назад
        /// </summary>
        public void GoBack()
        {
            if (CanGoBack)
            {
                object prevContent = _frame.Content;
                _frame.GoBack();
                OnNavigation?.Invoke(this, new NavEventArgs(prevContent, _frame.Content));
            }
        }

        /// <summary>
        /// Установить фрейм навигации
        /// </summary>
        /// <param name="frame"></param>
        public void SetFrame(Frame frame)
        {
            _frame = frame;
        }

        /// <summary>
        /// Установить главное окно
        /// </summary>
        /// <param name="mainWindow"></param>
        public void SetMainWindow(Window mainWindow)
        {
            _mainWindow = mainWindow;
            _windowTitle = mainWindow.Title;
        }

        /// <summary>
        /// Обновить заголовок окна
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateWindowTitle(object sender, NavEventArgs e)
        {
            if (e.NextPage is not null)
            {
                _mainWindow.Title = _windowTitle + (string.IsNullOrWhiteSpace(_windowTitle) ? "" : ": ") + e.NextPage.Title;
            }
        }
    }
}
