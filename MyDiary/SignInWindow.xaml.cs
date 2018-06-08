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

namespace MyDiary
{
    /// <summary>
    /// Interaction logic for SignInWindow.xaml
    /// </summary>
    public partial class SignInWindow : Window
    {
        public SignInWindow()
        {
            InitializeComponent();
        }

        private void SignIn()
        {
            string myConnectionString = "server=127.0.0.1; uid=root; pwd=; database=diary";
            string sql = "SELECT * FROM users WHERE username LIKE @username";
            
            MySqlConnection connection = new MySqlConnection(myConnectionString);
            MySqlCommand myCommand = new MySqlCommand(sql, connection);
            myCommand.Parameters.AddWithValue("username", usernameTextBox.Text);
            myCommand.Parameters.AddWithValue("password", passwordBox.Password);

            connection.Open();

            MySqlDataReader reader = myCommand.ExecuteReader();
            bool signed = false;
            while (reader.Read())
            {
                if (usernameTextBox.Text == reader.GetString("username") && passwordBox.Password == reader.GetString("password"))
                {
                    signed = true;
                    break;
                }
            }
            reader.Close();

            connection.Close();

            if (signed)
            {
                Globals.logged = true;
                Globals.username = usernameTextBox.Text;
                Close();
            }
            else
                signInLabel.Visibility = Visibility.Visible;
        }

        private void signInButton_Click(object sender, RoutedEventArgs e)
        {
            SignIn();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SignIn();
        }
    }
}
