using System;
using System.Collections.Generic;
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
using System.Windows.Forms;
using System.Collections;
using System.IO;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;
using System.Threading;


namespace Database
{
    /// <summary>
    /// Interaction logic for Exercise2.xaml
    /// </summary>
    public partial class Exercise2 : Window
    {
        AddressBook addressBook = new AddressBook();
        DBConnectSingleton DBConnection = null;


        public Exercise2()
        {
            InitializeComponent();
        }




        private void browseJson_button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select json file";
            openFileDialog.Filter = "json file (*.json)|*.json";
            openFileDialog.Multiselect = false;

            System.Windows.Forms.MessageBox.Show("Sample file located in 'ExampleDataFile' project folder", null, MessageBoxButtons.OK, MessageBoxIcon.Information);
            openFileDialog.ShowDialog();
            try
            {
                if (openFileDialog.OpenFile() != null)
                {
                    using (StreamReader streamReader = new StreamReader(openFileDialog.FileName))
                    {
                        string json = streamReader.ReadToEnd();

                        addressBook.addressList = JsonConvert.DeserializeObject<List<Person>>(json);
                        addressBook_listView.ItemsSource = addressBook.addressList;
                    }
                }

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
            }
        }


        private void connectDB_button_Click(object sender, RoutedEventArgs e)
        {
            DBConnection = DBConnectSingleton.Instance();
            DBConnection.setConnectionData(server_textBox.Text, database_textBox.Text, user_textBox.Text, password_textBox.Text);
            DBConnection.OpenConnection();
            
            string table = table_textBox.Text.ToString();
            string getAllData = "SELECT * FROM " + table;
            
            MySqlCommand command = new MySqlCommand(getAllData, DBConnection.getConnection());
            MySqlDataReader dataReader = command.ExecuteReader();


           addressBook.addressList = new List<Person>();                       
            while (dataReader.Read())
            {
                addressBook.addressList.Add(new Person()
                {
                    firstName = dataReader.GetValue(0).ToString(),
                    lastName = dataReader.GetValue(1).ToString(),
                    postCode = dataReader.GetValue(2).ToString(),
                    city = dataReader.GetValue(3).ToString(),
                    street = dataReader.GetValue(4).ToString()
                });
            }
            dataReader.Close();


            addressBook_listView.ItemsSource = addressBook.addressList;            
        }

        private void addRecord_button_Click(object sender, RoutedEventArgs e)
        {
            if(DBConnection != null)
            {
                string table = table_textBox.Text.ToString();
                string apsCommMark = "', '";
                string insertRecord = "INSERT INTO " + table + " (firstName, lastName, postCode, city, street) VALUES " +
                    "('" + addFirstName_textBox.Text.ToString() + apsCommMark + addLastName_textBox.Text.ToString() + apsCommMark +
                    addPostCode_textBox.Text.ToString() + apsCommMark + addCity_textBox.Text.ToString() + apsCommMark +
                    addStreet_textBox.Text.ToString() + "')";

                MySqlCommand command = new MySqlCommand(insertRecord, DBConnection.getConnection());
                command.ExecuteNonQuery();

                System.Windows.MessageBox.Show("Recor added!");

                connectDB_button_Click(null, null); //refresh view
            }
            else
            {
                System.Windows.MessageBox.Show("Database unconnected!");
            }
        }



        private void ComboBoxItem_Selected_firstName(object sender, RoutedEventArgs e)
        {
            try
            {
                sortAndDisplay("firstName", addressBook_listView);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error: Could not find Address Book. Original error: " + ex.Message);
            }
        }

        private void ComboBoxItem_Selected_lastName(object sender, RoutedEventArgs e)
        {
            try
            {
                sortAndDisplay("lastName", addressBook_listView);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error: Could not find Address Book. Original error: " + ex.Message);
            }
        }

        private void ComboBoxItem_Selected_postCode(object sender, RoutedEventArgs e)
        {
            try
            {
                sortAndDisplay("postCode", addressBook_listView);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error: Could not find Address Book. Original error: " + ex.Message);
            }
        }

        private void ComboBoxItem_Selected_city(object sender, RoutedEventArgs e)
        {
            try
            {
                sortAndDisplay("city", addressBook_listView);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error: Could not find Address Book. Original error: " + ex.Message);
            }
        }

        private void ComboBoxItem_Selected_street(object sender, RoutedEventArgs e)
        {
            try
            {
                sortAndDisplay("street", addressBook_listView);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error: Could not find Address Book. Original error: " + ex.Message);
            }
        }

        private void sortAndDisplay(string sortBy, System.Windows.Controls.ListView listView)
        {
            addressBook.sortBy = sortBy;
            addressBook.addressList.Sort(addressBook);

            listView.ItemsSource = null;
            listView.ItemsSource = addressBook.addressList;
        }

    }



    public class AddressBook : IComparer<Person>
    {
        public List<Person> addressList { get; set; }

        public string sortBy { get; set; }

        public int Compare(Person a, Person b)
        {
            if (sortBy == "firstName")
            {
                return ((Person)a).firstName.CompareTo(((Person)b).firstName);
            }
            else if (sortBy == "lastName")
            {
                return ((Person)a).lastName.CompareTo(((Person)b).lastName);
            }
            else if (sortBy == "postCode")
            {
                return ((Person)a).postCode.CompareTo(((Person)b).postCode);
            }
            else if (sortBy == "city")
            {
                return ((Person)a).city.CompareTo(((Person)b).city);
            }
            else if (sortBy == "street")
            {
                return ((Person)a).street.CompareTo(((Person)b).street);
            }

            else
            {
                return 0;
            }
        }
    }



    public class Person
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string postCode { get; set; }
        public string city { get; set; }
        public string street { get; set; }
    }
}
