using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WPF.Model
{
    public class Student
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("surname")]
        public string Surname { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("patronym")]
        public string? Patronym { get; set; }

        [JsonPropertyName("accountId")]
        public long? AccountId { get; set; }

        [JsonPropertyName("groupId")]
        public long GroupId { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("account")]
        public User? Account { get; set; }

        [JsonPropertyName("group")]
        public Group Group { get; set; }

        public override string ToString()
        {
            return Surname + ' ' + Name + ' ' + Patronym;
        }
    }
}
