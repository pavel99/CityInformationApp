using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CityInformationApp
{
    public partial class CityInformationUI : Form
    {
        public CityInformationUI()
        {
            InitializeComponent();
        }

        public int cityId;

        private bool isUpdateModeOn = false;

        private string connectionString =
            ConfigurationManager.ConnectionStrings["CityInfoDB"].ConnectionString;


        private void saveButton_Click(object sender, EventArgs e)
        {
            City aCity = new City();
            aCity.cityName = cityNameTextBox.Text;
            aCity.about = aboutTextBox.Text;
            aCity.country = countryTextBox.Text;
            if (isUpdateModeOn)
            {
                

                SqlConnection connection = new SqlConnection(connectionString);


                

                string query = "UPDATE Cities SET  About ='" + aCity.about + "' WHERE Id = '" + cityId + "'";


                

                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                int rowAffected = command.ExecuteNonQuery();
                connection.Close();
                if (rowAffected > 0)
                {
                    MessageBox.Show("Updated Successfully!");

                    saveButton.Text = "Save";
                    cityId = 0;
                    isUpdateModeOn = false;
                    ShowAllCityListView();
                    
                }
                else
                {
                    MessageBox.Show("Update Failed!");
                }
            }
            else
            {


                if (cityNameTextBox.Text == "" || aboutTextBox.Text == "" || countryTextBox.Text == "")
                {
                    MessageBox.Show("Please enter all values");
                }
                else
                {

                    if (!IscityAlreadyExists(aCity.cityName))
                    {
                        if (cityNameTextBox.Text.Length >= 4)
                        {

                            SqlConnection connection = new SqlConnection(connectionString);

                            string query = "INSERT INTO Cities Values('" + aCity.cityName + "','" + aCity.about + "','" +
                                           aCity.country + "')";

                            SqlCommand command = new SqlCommand(query, connection);

                            connection.Open();
                            int rowAffected = command.ExecuteNonQuery();
                            connection.Close();

                            if (rowAffected > 0)
                            {
                                MessageBox.Show("Saved Successfully!");
                            }
                            else
                            {
                                MessageBox.Show("Saving failed");
                            }
                        }

                        else
                        {
                            MessageBox.Show("City name must be atleast 4 characters long");
                        }

                    }
                    else
                    {
                        MessageBox.Show("City Name already Exists");
                    }
                }
                cityNameTextBox.Clear();
                aboutTextBox.Clear();
                countryTextBox.Clear();
                ShowAllCityListView();



            }
        }

        public bool IscityAlreadyExists  (string cityName)
            {
                SqlConnection connection = new SqlConnection(connectionString);
                string query = "SELECT * FROM Cities Where City_name='" + cityName + "'";
                bool isCityExists = false;
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    isCityExists = true;
                    break;
                }
                reader.Close();
                connection.Close();
                return isCityExists;


            }

       
        public void ShowAllCityListView()
        {
            
            SqlConnection connection = new SqlConnection(connectionString);

            
            string query = "SELECT * FROM Cities";

            //3.Execyute Query
            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            List<City> cityList= new List<City>();
            while (reader.Read())
            {
                City city = new City();

                city.Id = 1;
                city.cityName = (reader["City_name"].ToString());
                city.about = (reader["About"].ToString());
                city.country = (reader["Country"].ToString());
               
               
               

                cityList.Add(city);

            }
            LoadCitytListView(cityList);


        }
        public void LoadCitytListView(List<City> cityList)
        {
            int id = 1;
            showListView.Items.Clear();
            foreach (var city in cityList)
            {


                ListViewItem item = new ListViewItem(id.ToString());
                item.SubItems.Add(city.cityName);
                item.SubItems.Add(city.about);
                item.SubItems.Add(city.country);
                
               
                showListView.Items.Add(item);
               // city.Id++;
                id++;
            }


        }

        private void CityInformationUI_Load(object sender, EventArgs e)
        {
            ShowAllCityListView();
        }

        private void showListView_DoubleClick(object sender, EventArgs e)
        {
            ListViewItem item = showListView.SelectedItems[0];
            int id = int.Parse(item.Text.ToString());

            City city = GetCityByID(id);

            if (city != null)
            {
                cityNameTextBox.Enabled = false;
                isUpdateModeOn = true;
                saveButton.Text = "Update";
                cityId = city.Id;

                cityNameTextBox.Text = city.cityName;
                aboutTextBox.Text = city.about;
                countryTextBox.Text = city.country;

            }
        }

        public City GetCityByID(int id)
        {

            SqlConnection connection = new SqlConnection(connectionString);


            string query = "SELECT * FROM Cities WHERE Id='" + id + "'";


            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            List<City> cityList = new List<City>();
            while (reader.Read())
            {
                City city = new City();
                city.Id= int.Parse(reader["Id"].ToString());
                city.cityName = (reader["City_name"].ToString());
                city.about = (reader["About"].ToString());
                city.country= (reader["Country"].ToString());

                cityList.Add(city);

            }

            reader.Close();
            connection.Close();
            return cityList.FirstOrDefault();
        }

        private void Search_Click(object sender, EventArgs e)
        {
            City city = new City();
            city.cityName = cityNameTextBox.Text;
            city.about = aboutTextBox.Text;
            city.country = countryTextBox.Text;
            if (cityRadioButton.Checked)
            {
                

                SqlConnection connection = new SqlConnection(connectionString);

                string query = "SELECT * FROM Cities Where City_name like'" + searchTextBox.Text + "%'";

                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                List<City> cityList = new List<City>();
                while (reader.Read())
                {
                    city.cityName = (reader["City_name"].ToString());
                    city.about = (reader["About"].ToString());
                    city.country = (reader["Country"].ToString());
                    cityList.Add(city);

                }
                LoadCitytListView(cityList);
            }
            if (countryRadioButton.Checked)
            {
                SqlConnection connection = new SqlConnection(connectionString);

                string query = "SELECT * FROM Cities Where Country like'" + searchTextBox.Text + "%'";

                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                List<City> cityList = new List<City>();
                while (reader.Read())
                {
                    city.cityName = (reader["City_name"].ToString());
                    city.about = (reader["About"].ToString());
                    city.country = (reader["Country"].ToString());
                    cityList.Add(city);

                }
                LoadCitytListView(cityList);
                
            }











        }

    }
    }

