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
        private bool _changed;
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

            _changed = false;
        }

        public NoteWindow(uint id) : this()
        {
            LoadNote(id);
            _changed = false;
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

        private void LoadNote(uint id)
        {
            _noteID = id;

            string myConnectionString = "server=127.0.0.1; uid=root; pwd=; database=diary";
            MySqlConnection connection = new MySqlConnection(myConnectionString);

            connection.Open();

            string sql = "SELECT * FROM notes WHERE notes.id=@id";
            MySqlCommand myCommand = new MySqlCommand(sql, connection);
            myCommand.Parameters.AddWithValue("id", id);

            MySqlDataReader reader = myCommand.ExecuteReader();
            while (reader.Read())
            {
                titleTextBox.Text = reader.GetString("title");
                noteTextBox.Text = reader.GetString("text");
            }

            connection.Close();

            LoadTags();
        }

        private void CreateTags()
        {
            List<string> tags = tagsTextBox.Text.Split(',').ToList();
            List<uint> tags_id = new List<uint>();
            for (int i = 0; i < tags.Count; i++)
                tags[i] = tags[i].Trim();
            while (tags.Remove("")) ;

            string myConnectionString = "server=127.0.0.1; uid=root; pwd=; database=diary";
            MySqlConnection connection = new MySqlConnection(myConnectionString);

            connection.Open();

            string sql = "INSERT IGNORE INTO tags(text) VALUES(@text)";
            MySqlCommand myCommand = new MySqlCommand(sql, connection);
            myCommand.Parameters.Add("text", MySqlDbType.String);
            foreach (string tag in tags)
            {
                myCommand.Parameters[0].Value = tag;
                myCommand.ExecuteNonQuery();
            }
            connection.Close();

            foreach (string tag in tags)
            {
                connection.Open();
                sql = "SELECT id FROM tags WHERE tags.text LIKE @text";
                myCommand = new MySqlCommand(sql, connection);
                myCommand.Parameters.Add("text", MySqlDbType.String);
                myCommand.Parameters[0].Value = tag;

                MySqlDataReader reader = myCommand.ExecuteReader();
                while (reader.Read())
                {
                    tags_id.Add(reader.GetUInt32("id"));
                }
                connection.Close();
            }

            //Refresh tags
            connection.Open();
            sql = "DELETE FROM notes_tags WHERE notes_id = @id";
            myCommand = new MySqlCommand(sql, connection);
            myCommand.Parameters.AddWithValue("id", _noteID);
            myCommand.ExecuteNonQuery();

            sql = "INSERT IGNORE INTO notes_tags(notes_id, tags_id) VALUES(@notes_id, @tags_id)";
            myCommand = new MySqlCommand(sql, connection);
            myCommand.Parameters.AddWithValue("notes_id", _noteID);
            myCommand.Parameters.Add("tags_id", MySqlDbType.String);
            for (int i = 0; i < tags_id.Count; i++)
            {
                myCommand.Parameters[1].Value = tags_id[i];
                myCommand.ExecuteNonQuery();
            }

            connection.Close();
        }

        private void LoadTags()
        {
            tagsTextBox.Text = "";

            string myConnectionString = "server=127.0.0.1; uid=root; pwd=; database=diary";
            MySqlConnection connection = new MySqlConnection(myConnectionString);

            connection.Open();

            string sql = "SELECT text FROM tags, notes_tags WHERE notes_tags.notes_id LIKE @id AND notes_tags.tags_id = tags.id";
            MySqlCommand myCommand = new MySqlCommand(sql, connection);
            myCommand.Parameters.AddWithValue("id", _noteID);

            MySqlDataReader reader = myCommand.ExecuteReader();
            while (reader.Read())
            {
                tagsTextBox.Text += reader.GetString("text") + ",";
            }

            connection.Close();

            tagsTextBox.Text = tagsTextBox.Text.Substring(0, tagsTextBox.Text.Length - 1);   //Last comma
        }

        private void SaveNote()
        {
            if (_noteID == uint.MaxValue)
                NewNote();
            else
                UpdateNote();
            CreateTags();
            _changed = false;
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
            MessageBoxResult mbResult = MessageBoxResult.No;
            if (_changed)
                mbResult = MessageBox.Show("Do you want to save the changes?", "Save note", MessageBoxButton.YesNoCancel);

            if (mbResult != MessageBoxResult.Cancel)
            {
                if (mbResult == MessageBoxResult.Yes)
                    SaveNote();

                Window win = this;
                Closed = true;
                win.Close();
            }
        }

        public void Close(System.ComponentModel.CancelEventArgs e)
        {
            if (!Closed && _changed)
            {
                MessageBoxResult mbResult = MessageBox.Show("Do you want to save the changes?", "Save note", MessageBoxButton.YesNoCancel);

                if (mbResult == MessageBoxResult.Cancel)
                    e.Cancel = true;
                else
                {
                    if (mbResult == MessageBoxResult.Yes)
                        SaveNote();

                    Closed = true;
                }
            }
        }

        private void menuSave_Click(object sender, RoutedEventArgs e)
        {
            SaveNote();
        }

        private void menuLoad_Click(object sender, RoutedEventArgs e)
        {
            LoadNoteWindow loadWin = new LoadNoteWindow();
            loadWin.Owner = this;
            loadWin.Show();
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

        private void tagsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _changed = true;
        }

        private void titleTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _changed = true;
        }

        private void noteTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _changed = true;
        }
    }
}
