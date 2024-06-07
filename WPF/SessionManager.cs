using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WPF.Model;

namespace WPF
{
    class SessionManager
    {
        private static SessionManager? _instance = null;

        private static readonly object cookie = "cookie";

        public Frame Frame { get; set; } = new Frame();

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
    }
}
