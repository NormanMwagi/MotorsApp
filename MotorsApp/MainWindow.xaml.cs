using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Data;
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

namespace MotorsApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataTable dt;
        string ConnString = ConfigurationManager.ConnectionStrings["CarsDbConnection"].ConnectionString;
        public MainWindow()
        {
            InitializeComponent();
            ModelWindow modelWindow = new ModelWindow();
            modelWindow.Show();
            DriverWindow driverWindow = new DriverWindow();
            driverWindow.Show();
            dt = new DataTable();
            getData();
        }

        private void btnAdd_Click_1(object sender, RoutedEventArgs e)
        {
            string carName = carTxt.Text;
            string countryName = countryTxt.Text;
            try
            {
                
                using (SqlConnection sqlConnection = new SqlConnection(ConnString))
                {
                    SqlCommand cmd = new SqlCommand("add_car", sqlConnection);

                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter idParam = new SqlParameter("@id", SqlDbType.BigInt)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(idParam);
                    cmd.Parameters.AddWithValue("@car_name", carName);
                    cmd.Parameters.AddWithValue("@country", countryName);
                    sqlConnection.Open();
                    int affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows > 0)
                    {
                        if (cmd.Parameters["@id"].Value != DBNull.Value)
                        {
                            long carId = (long)cmd.Parameters["@id"].Value;
                            MessageBox.Show($"Data added with ID: {carId}");
                        }
                        else
                        {
                            MessageBox.Show("Data inserted successfully  but no id returned");
                        }
                        // Clear the input fields
                        carTxt.Text = "";
                        countryTxt.Text = "";
                        getData();
                    }
                    else
                    {
                        MessageBox.Show("Data insert failed ");
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message} ");
            }
        }
        // Event handler for Edit button click
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected item
            DataRowView selectedRow = (DataRowView)carDataGrid.SelectedItem;
            if (selectedRow != null)
            {
                // Assuming 'id' is the name of your ID column in the DataTable
                long carId = Convert.ToInt64(selectedRow["id"]);
                string carName = selectedRow["car_name"].ToString();
                string countryName = selectedRow["country"].ToString();
                // Open the edit window and pass the selected cars's details
                EditCarWindow editWindow = new EditCarWindow(carId, carName, countryName);
                editWindow.ShowDialog();

                // Refresh the DataGrid after editing
                getData();
            }
        }
        // Event handler for Delete button click
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected item
            DataRowView selectedRow = (DataRowView)carDataGrid.SelectedItem;
            if (selectedRow != null)
            {
                long leagueId = Convert.ToInt64(selectedRow["id"]);

                using (SqlConnection sqlConnection = new SqlConnection(ConnString))
                {
                    SqlCommand cmd = new SqlCommand("delete_car", sqlConnection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", leagueId);

                    sqlConnection.Open();
                    int affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows > 0)
                    {
                        MessageBox.Show("Car deleted successfully.");
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete the Car.");
                    }
                }

                // Refresh the DataGrid after deleting
                getData();
            }
        }


        // Update getData method to bind the DataTable to the DataGrid
        private void getData()
        {
            using (SqlConnection sqlConnection = new SqlConnection(ConnString))
            {
                SqlCommand cmd = new SqlCommand("get_cars", sqlConnection);
                sqlConnection.Open();

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                dt.Clear(); // Clear the existing data
                da.Fill(dt); // Fill the DataTable with new data
            }

            carDataGrid.ItemsSource = dt.DefaultView; // Bind the DataTable to the DataGrid
        }

    }
}