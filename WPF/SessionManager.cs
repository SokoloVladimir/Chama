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

        public AuthResponse? AuthResponse { get; set; } = null;

        public bool IsAuthorizied
        {
            get => this.AuthResponse is not null;
        }        

        private SessionManager()
        { 

        }

        public static SessionManager getInstance()
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

        public void Navigate(object content)
        {
            _frame.Navigate(content);

            if (content is Page page)
            {
                _mainWindow.Title = page.Title;
            }
        }

        public void SetFrame(Frame frame)
        {
            _frame = frame;
        }

        public void SetMainWindow(Window mainWindow)
        {
            _mainWindow = mainWindow;
        }
    }
}
