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
using System.Numerics;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Linq;

namespace Database
{
    /// <summary>
    /// Interaction logic for Exercise3.xaml
    /// </summary>
    public partial class Exercise3 : Window
    {
        DBConnectSingleton DBConnection = null;

        public Exercise3()
        {
            InitializeComponent();
        }


        private void runTest_button_Click(object sender, RoutedEventArgs e)
        {
            if(DBConnection != null)
            {
                int numberOfElements;
                if (int.TryParse(numberOfElements_textBox.Text, out numberOfElements))
                {
                    performanceComparison(numberOfElements);
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Number of elements field is not a number!");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Connect with Database first!");
            }
        }

        private void connectDB_button_Click(object sender, RoutedEventArgs e)
        {
            DBConnection = DBConnectSingleton.Instance();
            DBConnection.setConnectionData(server_textBox.Text, database_textBox.Text, user_textBox.Text, password_textBox.Text);
            DBConnection.OpenConnection();
        }


        private void performanceComparison(int numberOfElements)
        {
            TestData testData = new TestData();

            testData.fillCollectionRandomData(numberOfElements);
            testData.fillSqlDBRandomData(DBConnection, numberOfElements);

            numberElementsCollection_textBlock.Text = numberOfElements.ToString();
            numberElementsSql_textBlock.Text = numberOfElements.ToString();

            
            var sortTimeCollection = Stopwatch.StartNew();
                testData.sortCollectionDataByValue();
            sortTimeCollection.Stop();
            timeSortingCollection_textBlock.Text = sortTimeCollection.Elapsed.ToString();

            var sortTimeSql = Stopwatch.StartNew();
                testData.sortSqlDBDataByValue(DBConnection);
            sortTimeSql.Stop();
            timeSortingSql_textBlock.Text = sortTimeSql.Elapsed.ToString();


            int timeSortingCollection;
            int timeSortingSql;
            int.TryParse(timeSortingCollection_textBlock.Text, out timeSortingCollection);
            int.TryParse(timeSortingSql_textBlock.Text, out timeSortingSql);

            if(timeSortingCollection > timeSortingSql)
            {
                timeSortingCollection_textBlock.Background = Brushes.Green;
            }
            else
            {
                timeSortingSql_textBlock.Background = Brushes.Green;
            }
        }
    }

    public class TestData
    {
        private Random rand = new Random();
        private Dictionary<BigInteger, BigInteger> testData = new Dictionary<BigInteger, BigInteger>();

        private PleaseWait[] pleaseWait = new PleaseWait[4];

        public void fillCollectionRandomData(int numberOfElements)
        {
            pleaseWait[0] = new PleaseWait();
            pleaseWait[0].Show();
                for (int i = 0; i < numberOfElements; i++)
                {
                    testData.Add(new BigInteger(i), new BigInteger(rand.Next(1000000, 5000000)));
                }
            pleaseWait[0].Close();
        }

        public void fillSqlDBRandomData(DBConnectSingleton DBConnection, int numberOfElements)
        {
            string createTable = "CREATE TABLE testdata(id BIGINT, data BIGINT)";
            MySqlCommand createCommand = new MySqlCommand(createTable, DBConnection.getConnection());
            try
            {
                createCommand.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Table already exist. Orginal error: " + ex.Message);
            }

            pleaseWait[1] = new PleaseWait();
            pleaseWait[1].Show();
                for (int i = 0; i < numberOfElements; i++)
                {
                    string insertData = "INSERT INTO testdata (id, data) VALUES ('" + i + "', '" + rand.Next(1000000, 5000000) + "')";
                    MySqlCommand insertCommand = new MySqlCommand(insertData, DBConnection.getConnection());
                    insertCommand.ExecuteNonQuery();
                }
            pleaseWait[1].Close();
        }


        public void sortCollectionDataByValue()
        {
            pleaseWait[2] = new PleaseWait();
            pleaseWait[2].Show();
                var items = from pair in testData
                            orderby pair.Value ascending
                            select pair;
            pleaseWait[2].Close();
        }

        public void sortSqlDBDataByValue(DBConnectSingleton DBConnection)
        {
            string sort = "SELECT * FROM testdata ORDER BY data";
            MySqlCommand sortCommand = new MySqlCommand(sort, DBConnection.getConnection());
            sortCommand.ExecuteNonQuery();


            pleaseWait[3] = new PleaseWait();
            pleaseWait[3].Show();
                sortCommand.ExecuteNonQuery();
            pleaseWait[3].Close();
        }
    }
}
