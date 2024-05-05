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
    /// Interaction logic for EditDriverWindow.xaml
    /// </summary>
    public partial class EditDriverWindow : Window
    {
        public long id { get; set; }
        public string driver_name { get; set; }
        public string license_number { get; set; }

        string ConnString = ConfigurationManager.ConnectionStrings["CarsDbConnection"].ConnectionString;
        public EditDriverWindow(long driverId, string driverName, string licenseNo)
        {
            InitializeComponent();
            id = driverId;
            driver_name = driverName;
            license_number = licenseNo;
            driverTxt.Text = driver_name;
            licenseTxt.Text = license_number;
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            string driverName = driverTxt.Text;
            string licenseNo = licenseTxt.Text;

            try
            {
                
                using (SqlConnection sqlConnection = new SqlConnection(ConnString))
                {
                    SqlCommand cmd = new SqlCommand("update_drivers", sqlConnection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@driver_name", driverName);
                    cmd.Parameters.AddWithValue("@license_number", licenseNo);

                    sqlConnection.Open();
                    int affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows > 0)
                    {
                        MessageBox.Show("Data updated successfully.");
                    }
                    else
                    {
                        MessageBox.Show("No data was updated. Please check the driver ID and try again.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
            this.DialogResult = true;
        }

    }
}
