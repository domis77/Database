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
using MySql.Data.MySqlClient;
using System.Collections;

namespace Database
{
    /// <summary>
    /// Interaction logic for Exercise1.xaml
    /// </summary>
    public partial class Exercise1 : Window
    {
        private DBConnectSingleton DBConnection;

        public Exercise1()
        {
            InitializeComponent();
        }


        private void connect_button_Click(object sender, RoutedEventArgs e)
        {
            DBConnection = DBConnectSingleton.Instance();
            DBConnection.setConnectionData(server_textBox.Text, database_textBox.Text, user_textBox.Text, password_textBox.Text);

            DBConnection.OpenConnection();
        }

        private void disconnect_button_Click(object sender, RoutedEventArgs e)
        {
            DBConnection.CloseConnection();
        }


        private void executeQuery_button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(query_textBox.Text))
            {
                DBConnection.executeQuery(queryType_comboBox.SelectedValue.ToString(), query_textBox.Text);
            }
            else
            {
                MessageBox.Show("Query is empty!");
            }
        }
    }



    public class DBConnectSingleton
    {
        private static DBConnectSingleton DBConnectInstance;
        public MySqlConnection connection;
        private bool connect = false;

        private string connectionData;


        private DBConnectSingleton() { }

        public static DBConnectSingleton Instance()
        {
            if(DBConnectInstance == null)
            {
                DBConnectInstance = new DBConnectSingleton();
            }
            return DBConnectInstance;
        }

        public void setConnectionData(string server, string database, string user, string password)
        {
            connectionData = "SERVER=" + server + ";" + "DATABASE=" + database + ";" + "UID=" + user + ";" + "PASSWORD=" + password + ";";
            connection = new MySqlConnection(connectionData);
        }


        public void OpenConnection()
        {
            try
            {
                connection.Open();
                connect = true;
                MessageBox.Show("Connected");
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        MessageBox.Show("Cannot connect to server. Error: " + ex.Message);
                        break;

                    case 1045:
                        MessageBox.Show("Invalid username/password, Error: " + ex.Message);
                        break;
                }
            }
        }

        public void CloseConnection()
        {
            try
            {
                connection.Close();
                connect = false;
                MessageBox.Show("Disconnected");
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }


        public void executeQuery(string queryType, string query)
        {
            if (connect == true)
            {
                switch (queryType.ToLower())
                {
                    case "select":
                        select(query);
                        break;

                    case "insert":
                        insert(query);
                        break;

                    case "update":
                        update(query);
                        break;

                    case "delete":
                        delete(query);
                        break;

                    default:
                        MessageBox.Show("Error!");
                        break;
                }
            }
            else
            {
                MessageBox.Show("CLOSED CONNECTION");
            }
        }


        private void select(string query)
        {
            ArrayList resultQuery = new ArrayList();

            if(connect == true)
            {

                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader dataReader = command.ExecuteReader();
   

                while (dataReader.Read())
                {
                    ArrayList row = new ArrayList();
                    for (int i = 0; i < dataReader.FieldCount; i++)
                    {
                        row.Add(dataReader.GetValue(i));
                    }
                    resultQuery.Add(row);
                }
                dataReader.Close();

                //return resultQuery;
            }
            else
            {
                resultQuery.Add("CLOSED CONNECTION");
            }
        }


        private void insert(string query)
        {
            if(connect == true)
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.ExecuteNonQuery();

                MessageBox.Show("Done!");
            }
            else
            {
                MessageBox.Show("CLOSED CONNECTION");
            }
        }

        private void update(string query)
        {
            if(connect == true)
            {
                MySqlCommand command = new MySqlCommand();
                command.CommandText = query;
                command.Connection = connection;

                command.ExecuteNonQuery();

                MessageBox.Show("Done!");
            }
            else
            {
                MessageBox.Show("CLOSED CONNECTION");
            }
        }

        private void delete(string query)
        {
            if (connect == true)
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.ExecuteNonQuery();

                MessageBox.Show("Done!");
            }
            else
            {
                MessageBox.Show("CLOSED CONNECTION");
            }
        }


    }
}
