using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WPF.Model
{
    public partial class User
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("login")]
        public string Login { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }
    }
}
