using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
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
using static MaterialDesignThemes.Wpf.Theme;

namespace WPF
{
    /// <summary>
    /// Логика взаимодействия для ExportPage.xaml
    /// </summary>
    [ObservableObject]
    public partial class ExportPage : Page
    {
        public List<Group> Groups { get; set; } = new List<Group>();

        public Group SelectedGroup { get; set; }

        public List<Discipline> Disciplines { get; set; } = new List<Discipline>();

        public Discipline SelectedDiscipline { get; set; }

        public List<Semester> Semesters { get; set; } = new List<Semester>();

        public Semester SelectedSemester { get; set; }

        /// <summary>
        /// Данные в csv-формате
        /// </summary>
        [ObservableProperty]
        private string _rawData = String.Empty;
        
        public bool IsRawDataContains
        {
            get => !string.IsNullOrWhiteSpace(RawData);
        }

        public ExportPage()
        {
            InitializeComponent();
            DataContext = this;

            Refresh();
        }

        /// <summary>
        /// Обновить данные из фильтров ComboBox'ов
        /// </summary>
        /// <returns></returns>
        private async Task Refresh()
        {
            try
            {
                Disciplines.AddRange(await SessionManager.getInstance().ResolveClient().GetFromJsonAsync<List<Discipline>>("/discipline"));
                Groups.AddRange(await SessionManager.getInstance().ResolveClient().GetFromJsonAsync<List<Group>>("/group"));
                Semesters.AddRange(await SessionManager.getInstance().ResolveClient().GetFromJsonAsync<List<Semester>>("/semester"));
                Semesters.Reverse();
            }
            catch { }
        }


        /// <summary>
        /// Очистить данные и нормализовать текст
        /// </summary>
        /// <returns></returns>
        private string PrepareRawData(string data)
        {
            return data.Replace("\r", "");
        }

        /// <summary>
        /// Нормализовать массив и удалить пустые элементы
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        private string[] PrepareArray(string[] array)
        {
            return array.Where(column => !string.IsNullOrWhiteSpace(column)).ToArray();
        }

        /// <summary>
        /// Обновить таблицу
        /// </summary>
        /// <returns></returns>
        private async Task UpdateGrid()
        {            
            try
            {
                string data = await SessionManager.getInstance().ResolveClient().GetStringAsync($"/export/{SelectedGroup.Id}/{SelectedDiscipline.Id}/{SelectedSemester.Id}");
                string[] rows = PrepareRawData(data).Split('\n');

                DataTable dataTable = new DataTable();
                string[] columns = PrepareArray(rows[0].Split(','));
                foreach (string column in columns)
                {
                    dataTable.Columns.Add(column);
                }

                for (int i = 1; i < rows.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(rows[i]))
                    {
                        continue;
                    }
                    
                    string[] fields = PrepareArray(rows[i].Split(','));
                    dataTable.Rows.Add(fields);
                }

                mainDataGrid.ItemsSource = null;
                mainDataGrid.ItemsSource = dataTable.DefaultView;

                RawData = data;
                PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs("IsRawDataContains"));
            }
            catch { }           
        }

        /// <summary>
        /// Экспортировать в Excel
        /// </summary>
        private void ExportInitiate()
        {
            if (!IsRawDataContains)
            {
                MessageBox.Show("Сначала загрузите данные", "Предупреждение Экспорта", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                DefaultExt = "csv"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                using (StreamWriter streamWriter = new StreamWriter(File.Open(saveFileDialog.FileName, FileMode.Create), Encoding.UTF8))
                {
                    streamWriter.Write(RawData);
                }

                ShellExecute(saveFileDialog.FileName);
            }
        }


        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            List<String> errors = new List<String>();

            if (SelectedGroup is null)
            {
                errors.Add("Не выбрана группа");
            }
            if (SelectedDiscipline is null)
            {
                errors.Add("Не выбрана дисциплина");
            }
            if (SelectedSemester is null)
            {
                errors.Add("Не выбран семестр");
            }

            if (errors.Count > 0)
            {
                MessageBox.Show(String.Join('\n', errors), "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                UpdateGrid();
            }            
        }


        [DllImport("Shell32.dll")]
        private static extern int ShellExecuteA(IntPtr hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirecotry, int nShowCmd);

        public static int ShellExecute(string filename, string parameters = "", string workingFolder = "", string verb = "open", int windowOption = 1)
        {
            //calls Windows ShellExecute API
            //for verbs see https://learn.microsoft.com/en-us/windows/win32/shell/launch (open/edit/runas...)
            //for windowOptions see https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-showwindow (show/hide/maximize etc)

            IntPtr parentWindow = IntPtr.Zero;

            try
            {
                int pid = ShellExecuteA(parentWindow, verb, filename, parameters, workingFolder, windowOption);
                return pid;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return 0;
            }

        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            ExportInitiate();
        }
    }
}
