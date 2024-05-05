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
    /// Interaction logic for EditModelWindow.xaml
    /// </summary>
    public partial class EditModelWindow : Window
    {
        public long id { get; set; }
        public string model_name { get; set; }
        public string manufacture_year { get; set; }

        string ConnString = ConfigurationManager.ConnectionStrings["CarsDbConnection"].ConnectionString;
        public EditModelWindow(long modelId, string modelName, string yearNo)
        {
            InitializeComponent();
            id = modelId;
            model_name = modelName;
            manufacture_year = yearNo;
            modelTxt.Text = model_name;
            yearTxt.Text = manufacture_year;
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            string modelName = modelTxt.Text;
            string yearNo = yearTxt.Text;

            try
            {

                using (SqlConnection sqlConnection = new SqlConnection(ConnString))
                {
                    SqlCommand cmd = new SqlCommand("update_model", sqlConnection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@model_name", modelName);
                    cmd.Parameters.AddWithValue("@manufacture_year", yearNo);

                    sqlConnection.Open();
                    int affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows > 0)
                    {
                        MessageBox.Show("Data updated successfully.");
                    }
                    else
                    {
                        MessageBox.Show("No data was updated. Please check the model ID and try again.");
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
