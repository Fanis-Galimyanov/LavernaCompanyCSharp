using System.Windows;
using System.Data.SqlClient;
using System.Data;

namespace LavernaCompanyCSharp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadGrid();
        }


         static string relativePath = "..\\..\\..\\OrgTexnika.mdf";

        // Получаем абсолютный путь к базе данных:
         static string absolutePathToBD = System.IO.Path.GetFullPath(relativePath);
        SqlConnection connection = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename="+absolutePathToBD+";Integrated Security=True;Connect Timeout=30");

        public void clearData()
        {
            name_txt.Clear();
            type_txt.Clear();
            status_txt.Clear();
            search_txt.Clear();
        }

        public void LoadGrid()
        {
            SqlCommand cmd = new SqlCommand("select * from Technique", connection);
            DataTable dt = new DataTable();
            connection.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            dt.Load(sdr);
            connection.Close();
            dataGrid.ItemsSource = dt.DefaultView;
        }

        private void ClearDataBtn_Click(object sender, RoutedEventArgs e)
        {
            clearData();
        }

        public bool isValid()
        {
            if (name_txt.Text == string.Empty)
            {
                MessageBox.Show("Наименование техники не введено!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);  
                return false;
            }
            if (type_txt.Text == string.Empty)
            {
                MessageBox.Show("Тип техники не введен!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (status_txt.Text == string.Empty)
            {
                MessageBox.Show("Статус техники не введен!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private void InsertBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (isValid())
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO Technique VALUES (@name, @type, @status)", connection);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@name", name_txt.Text);
                    cmd.Parameters.AddWithValue("@type", type_txt.Text);
                    cmd.Parameters.AddWithValue("@status", status_txt.Text);
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();
                    LoadGrid();
                    MessageBox.Show("Запись успешно добавлена в базу!", "Сохранено", MessageBoxButton.OK, MessageBoxImage.Information);
                    clearData();
                }
            }
            catch(SqlException ex) {
                MessageBox.Show(ex.Message);
            }
            

        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            connection.Open();
            SqlCommand cmd = new SqlCommand("delete from Technique where id = "+search_txt.Text+"",connection);
            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("Запись была удалена", "Удаление", MessageBoxButton.OK, MessageBoxImage.Information);
                connection.Close();
                clearData();
                LoadGrid();
                connection.Close();

            }
            catch (SqlException ex)
            {
                MessageBox.Show("Удаелние не произошло" + ex.Message);
            }
            finally { 
                connection.Close(); 
            }
        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            if (search_txt.Text != string.Empty) {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("update Technique set name = N'"+name_txt.Text+"', type = N'"+type_txt.Text+"', status = N'"+status_txt.Text+"' WHERE ID = '"+search_txt.Text+"' ",connection);
                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Запись была обнавлена", "Обнавление", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    connection.Close();
                    clearData();
                    LoadGrid();
                }
            }
            else
            {
                MessageBox.Show("Введите ID записи которую хотите обновить!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}