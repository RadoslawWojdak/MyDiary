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
    /// Interaction logic for NoteWindow.xaml
    /// </summary>
    public partial class NoteWindow : Window
    {
        private uint _noteID;
        private bool _isClosed;
        public bool Closed { get { return _isClosed; } set { _isClosed = value; } }

        public NoteWindow()
        {
            InitializeComponent();

            _noteID = uint.MaxValue;
            _isClosed = false;
            tagsTextBox.Text = "";
            titleTextBox.Text = "";
            noteTextBox.Text = "";

            AdjustControlsParameters(Width, Height);
        }

        private void NewNote()
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            string myConnectionString = "server=127.0.0.1; uid=root; pwd=; database=diary";
            MySqlConnection connection = new MySqlConnection(myConnectionString);

            connection.Open();
            
            string sql = "INSERT INTO notes(diaries_id, title, text, creation_date, modification_date) SELECT diaries.id, @title, @text, @creation_date, @creation_date FROM diaries, users WHERE diaries.name LIKE @diary AND users.username LIKE @username AND users.id = diaries.users_id";
            MySqlCommand myCommand = new MySqlCommand(sql, connection);
            myCommand.Parameters.AddWithValue("diary", Globals.openDiary);
            myCommand.Parameters.AddWithValue("title", titleTextBox.Text);
            myCommand.Parameters.AddWithValue("text", noteTextBox.Text);
            myCommand.Parameters.AddWithValue("creation_date", date);
            myCommand.Parameters.AddWithValue("username", Globals.username);
            myCommand.ExecuteNonQuery();

            sql = "SELECT LAST_INSERT_ID()";
            myCommand = new MySqlCommand(sql, connection);
            MySqlDataReader reader = myCommand.ExecuteReader();
            while (reader.Read())
            {
                _noteID = reader.GetUInt32(0);
            }

            connection.Close();
        }

        private void UpdateNote()
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            string myConnectionString = "server=127.0.0.1; uid=root; pwd=; database=diary";
            MySqlConnection connection = new MySqlConnection(myConnectionString);

            connection.Open();

            string sql = "UPDATE notes SET title=@title, text=@text, modification_date=@date WHERE id=@id";
            MySqlCommand myCommand = new MySqlCommand(sql, connection);
            myCommand.Parameters.AddWithValue("title", titleTextBox.Text);
            myCommand.Parameters.AddWithValue("text", noteTextBox.Text);
            myCommand.Parameters.AddWithValue("date", date);
            myCommand.Parameters.AddWithValue("id", _noteID);
            myCommand.ExecuteNonQuery();

            connection.Close();
        }

        //at the beginning "ActualWidth" and "ActualHeight" is equal to 0
        private void AdjustControlsParameters(double winWidth, double winHeight)
        {
            menu.Width = winWidth;

            double textBoxLeft = Math.Max(tagsLabel.ActualWidth, titleLabel.ActualWidth);
            double textBoxWidth = winWidth - textBoxLeft - 20;
            
            tagsTextBox.Width = textBoxWidth;
            tagsTextBox.Margin = new Thickness(textBoxLeft, menu.Height + 2, 0, 0);

            titleTextBox.Width = textBoxWidth;
            titleTextBox.Margin = new Thickness(textBoxLeft, tagsTextBox.Margin.Top + tagsTextBox.ActualHeight, 0, 0);

            noteTextBox.Width = winWidth - 24;
            noteTextBox.Height = winHeight - menu.Height - titleTextBox.Height - 42;
        }

        public void Close()
        {
            MessageBoxResult mbResult = MessageBox.Show("Do you want to save the changes?", "Save note", MessageBoxButton.YesNoCancel);

            if (mbResult != MessageBoxResult.Cancel)
            {
                Window win = this;
                Closed = true;
                win.Close();
            }
        }

        public void Close(System.ComponentModel.CancelEventArgs e)
        {
            if (!Closed)
            {
                MessageBoxResult mbResult = MessageBox.Show("Do you want to save the changes?", "Save note", MessageBoxButton.YesNoCancel);

                if (mbResult == MessageBoxResult.Cancel)
                    e.Cancel = true;
                else
                    Closed = true;
            }
        }

        private void menuSave_Click(object sender, RoutedEventArgs e)
        {
            if (_noteID == uint.MaxValue)
                NewNote();
            else
                UpdateNote();
        }

        private void menuLoad_Click(object sender, RoutedEventArgs e)
        {
            ;
        }

        private void menuClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Close(e);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            AdjustControlsParameters(ActualWidth, ActualHeight);
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            AdjustControlsParameters(ActualWidth, ActualHeight);
        }
    }
}
