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


namespace Database
{
    /// <summary>
    /// Interaction logic for ConnectSqlDB.xaml
    /// </summary>
    public partial class ConnectSqlDB : Window
    {
        private DBConnectSingleton DBConnection;
        MySqlConnection connection = null;


        public ConnectSqlDB()
        {
            InitializeComponent();
        }

        private void connect_button_Click(object sender, RoutedEventArgs e)
        {
            DBConnection = DBConnectSingleton.Instance();
            DBConnection.setConnectionData(server_textBox.Text, database_textBox.Text, user_textBox.Text, password_textBox.Text);

            MySqlConnection connection = DBConnection.connection;
        }

        public MySqlConnection getConncetion()
        {
            return connection;
        }
    }
}

