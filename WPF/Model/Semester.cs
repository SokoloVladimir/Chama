using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WPF.Model
{
    public class Semester
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("startYear")]
        public long StartYear { get; set; }

        [JsonPropertyName("isSecond")]
        public bool IsSecond { get; set; }

        public override string ToString()
        {
            return StartYear.ToString() + '/' + (IsSecond ? '2' : '1');
        }
    }
}
