using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WPF
{
    public class NavEventArgs : EventArgs
    {
        public object NextContent { get; set; }
        public object PrevContent { get; set; }

        public Page? NextPage
        {
            get => (Page)NextContent;
        }

        public Page? PrevPage
        {
            get => (Page)PrevContent;
        }

        public NavEventArgs(object prevContent, object nextContent)
        {
            NextContent = nextContent;
        }
    }
}
