using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Frame MainFrameContent { get; set; } = new Frame();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            SessionManager navManager = SessionManager.getInstance();
            navManager.Frame = MainFrameContent;
            navManager.Frame.Navigate(new AuthPage());
        }


    }
}