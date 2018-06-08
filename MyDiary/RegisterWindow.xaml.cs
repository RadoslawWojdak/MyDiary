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
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void Register()
        {
            if (enterPasswordBox.Password == reenterPasswordBox.Password)
            {
                if (emailTextBox.Text.Length == 0)
                    errorLabel.Content = "Enter your email address!";
                else if (usernameTextBox.Text.Length == 0)
                    errorLabel.Content = "Enter your username!";
                else if (enterPasswordBox.Password.Length == 0)
                    errorLabel.Content = "Enter your password!";
                else
                    CreateAccount();
            }
            else
                errorLabel.Content = "The re-entered password doesn't match!";
        }

        private bool doesUserExist(string email, string username)
        {
            string myConnectionString = "server=127.0.0.1; uid=root; pwd=; database=diary";
            MySqlConnection connection = new MySqlConnection(myConnectionString);

            connection.Open();

            string sql = "SELECT username, email FROM users WHERE username LIKE @username OR email LIKE @email";
            MySqlCommand myCommand = new MySqlCommand(sql, connection);
            myCommand.Parameters.AddWithValue("username", username);
            myCommand.Parameters.AddWithValue("email", email);

            MySqlDataReader reader = myCommand.ExecuteReader();
            bool found = false;
            while (reader.Read())
            {
                if (username.ToUpper() == reader.GetString("username").ToUpper() || email.ToUpper() == reader.GetString("email").ToUpper())
                    found = true;
            }
            reader.Close();

            connection.Close();

            return found;
        }

        private void CreateAccount()
        {
            if (doesUserExist(emailTextBox.Text, usernameTextBox.Text))
                errorLabel.Content = "User with this name or email already exists!";
            else
            {
                string myConnectionString = "server=127.0.0.1; uid=root; pwd=; database=diary";
                MySqlConnection connection = new MySqlConnection(myConnectionString);

                connection.Open();

                string sql = "INSERT INTO users (username, password, email) VALUES (@username, @password, @email)";
                MySqlCommand myCommand = new MySqlCommand(sql, connection);
                myCommand.Parameters.AddWithValue("username", usernameTextBox.Text);
                myCommand.Parameters.AddWithValue("password", enterPasswordBox.Password);
                myCommand.Parameters.AddWithValue("email", emailTextBox.Text);
                myCommand.ExecuteNonQuery();

                connection.Close();

                Globals.logged = true;
                Globals.username = usernameTextBox.Text;

                Close();
            }
        }

        private void registerButton_Click(object sender, RoutedEventArgs e)
        {
            Register();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Register();
        }
    }
}
