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
    /// Interaction logic for EditCarWindow.xaml
    /// </summary>
    public partial class EditCarWindow : Window
    {
        public long id { get; set; }
        public string car_name { get; set; }
        public string country { get; set; }

        string ConnString = ConfigurationManager.ConnectionStrings["CarsDbConnection"].ConnectionString;

        public EditCarWindow(long carId, string carName, string countryName)
        {
            InitializeComponent();
            id = carId;
            car_name = carName;
            country = countryName;
            carNameTxt.Text = car_name;
            countryNameTxt.Text = country;
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            string carName = carNameTxt.Text;
            string countryName = countryNameTxt.Text;

            try
            {
                
                using (SqlConnection sqlConnection = new SqlConnection(ConnString))
                {
                    SqlCommand cmd = new SqlCommand("update_car", sqlConnection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@car_name", carName);
                    cmd.Parameters.AddWithValue("@country", countryName);

                    sqlConnection.Open();
                    int affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows > 0)
                    {
                        MessageBox.Show("Data updated successfully.");
                    }
                    else
                    {
                        MessageBox.Show("No data was updated. Please check the car ID and try again.");
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
