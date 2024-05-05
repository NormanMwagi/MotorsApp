using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MotorsApp
{
    /// <summary>
    /// Interaction logic for DriverWindow.xaml
    /// </summary>
    public partial class DriverWindow : Window
    {
        private DataTable dt;
        string ConnString = ConfigurationManager.ConnectionStrings["CarsDbConnection"].ConnectionString;
        public DriverWindow()
        {
            InitializeComponent();

            dt = new DataTable();
            getData();
        }

        private void btnAdd_Click_1(object sender, RoutedEventArgs e)
        {
            string driverName = driverTxt.Text;
            string licenseNo = licenseTxt.Text;
            try
            {

              
                using (SqlConnection sqlConnection = new SqlConnection(ConnString))
                {
                    SqlCommand cmd = new SqlCommand("add_driver", sqlConnection);

                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter idParam = new SqlParameter("@id", SqlDbType.BigInt)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(idParam);
                    cmd.Parameters.AddWithValue("@driver_name", driverName);
                    cmd.Parameters.AddWithValue("@license_number", licenseNo);
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
                        driverTxt.Text = "";
                        licenseTxt.Text = "";
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
            DataRowView selectedRow = (DataRowView)driverDataGrid.SelectedItem;
            if (selectedRow != null)
            {
                // Assuming 'id' is the name of your ID column in the DataTable
                long driverId = Convert.ToInt64(selectedRow["id"]);
                string driverName = selectedRow["driver_name"].ToString();
                string licenseNo = selectedRow["license_number"].ToString();
                // Open the edit window and pass the selected cars's details
                EditDriverWindow editWindow = new EditDriverWindow(driverId, driverName, licenseNo);
                editWindow.ShowDialog();

                // Refresh the DataGrid after editing
                getData();
            }
        }
        // Event handler for Delete button click
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected item
            DataRowView selectedRow = (DataRowView)driverDataGrid.SelectedItem;
            if (selectedRow != null)
            {
                long leagueId = Convert.ToInt64(selectedRow["id"]);

                using (SqlConnection sqlConnection = new SqlConnection(ConnString))
                {
                    SqlCommand cmd = new SqlCommand("delete_driver", sqlConnection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", leagueId);

                    sqlConnection.Open();
                    int affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows > 0)
                    {
                        MessageBox.Show("Driver deleted successfully.");
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete the Driver.");
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
                SqlCommand cmd = new SqlCommand("get_drivers", sqlConnection);
                sqlConnection.Open();

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                dt.Clear(); // Clear the existing data
                da.Fill(dt); // Fill the DataTable with new data
            }

            driverDataGrid.ItemsSource = dt.DefaultView; // Bind the DataTable to the DataGrid
        }

    }
}