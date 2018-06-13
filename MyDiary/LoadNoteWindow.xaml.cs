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
    /// Interaction logic for LoadNote.xaml
    /// </summary>
    public partial class LoadNoteWindow : Window
    {
        List<Button> _noteButtons;
        List<CheckBox> _tagCheckBoxes;
        List<uint> _noteIDs;
        bool _closed;

        public LoadNoteWindow()
        {
            InitializeComponent();

            _noteButtons = new List<Button>();
            _tagCheckBoxes = new List<CheckBox>();
            _closed = false;

            AdjustControlsParameters(Width, Height);

            foreach (string note in getNotesNames(Globals.username, Globals.openDiary, ref _noteIDs))
                createNoteButton(note);
            foreach (string tag in getTagsNames(Globals.username, Globals.openDiary))
                createTagCheckBox(tag);
        }

        //at the beginning "ActualWidth" and "ActualHeight" is equal to 0
        private void AdjustControlsParameters(double winWidth, double winHeight)
        {
            notesScroll.Width = winWidth - 16;
            notesScroll.Height = winHeight - 71;
            tagsScroll.Width = winWidth - 16;
        }

        private List<string> getNotesNames(string username, string diary, ref List<uint> noteIDs)
        {
            noteIDs = new List<uint>();
            List<string> notesNames = new List<string>();

            string myConnectionString = "server=127.0.0.1; uid=root; pwd=; database=diary";
            MySqlConnection connection = new MySqlConnection(myConnectionString);

            connection.Open();

            string sql = "SELECT notes.id, notes.title, notes.diaries_id, diaries.name, diaries.users_id, users.username, users.id FROM notes, diaries, users WHERE users.username LIKE @username AND diaries.name LIKE @diary AND users.id = diaries.users_id AND diaries.id = notes.diaries_id";
            MySqlCommand myCommand = new MySqlCommand(sql, connection);
            myCommand.Parameters.AddWithValue("username", username);
            myCommand.Parameters.AddWithValue("diary", diary);

            MySqlDataReader reader = myCommand.ExecuteReader();
            while (reader.Read())
            {
                noteIDs.Add(reader.GetUInt32(0));
                notesNames.Add(reader.GetString("title"));
            }

            connection.Close();

            return notesNames;
        }

        private List<string> getTagsNames(string username, string diary)
        {
            List<string> tags = new List<string>();
            
            string myConnectionString = "server=127.0.0.1; uid=root; pwd=; database=diary";
            MySqlConnection connection = new MySqlConnection(myConnectionString);

            connection.Open();

            string sql = "SELECT tags.text FROM tags, notes, notes_tags, diaries, users WHERE users.username = @username AND diaries.name = @diary AND diaries.users_id = users.id AND notes.diaries_id = diaries.id AND notes_tags.notes_id = notes.id AND notes_tags.tags_id = tags.id";
            MySqlCommand myCommand = new MySqlCommand(sql, connection);
            myCommand.Parameters.AddWithValue("username", username);
            myCommand.Parameters.AddWithValue("diary", diary);

            MySqlDataReader reader = myCommand.ExecuteReader();
            while (reader.Read())
            {
                tags.Add(reader.GetString(0));
            }

            connection.Close();

            return tags;
        }

        private void createNoteButton(string name)
        {
            Button button = new Button();
            button.Content = name;
            button.Click += button_Click;

            _noteButtons.Add(button);
            notesStackPanel.Children.Add(button);
        }

        private void createTagCheckBox(string name)
        {
            CheckBox checkBox = new CheckBox();
            checkBox.Content = name;

            _tagCheckBoxes.Add(checkBox);
            tagsStackpanel.Children.Add(checkBox);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            AdjustControlsParameters(this.Width, this.Height);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            for (int i = 0; i < _noteButtons.Count; i++)
            {
                if (_noteButtons[i] == button)
                {
                    NoteWindow noteWin = new NoteWindow(_noteIDs[i]);
                    noteWin.Owner = Owner;
                    noteWin.Show();
                    break;
                }
            }

            Close();
        }
    }
}
