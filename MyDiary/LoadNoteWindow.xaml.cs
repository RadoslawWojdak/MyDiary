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
        List<string> _allNotes;
        List<uint> _allNoteIDs; 

        List<Button> _noteButtons;
        List<CheckBox> _tagCheckBoxes;
        List<uint> _noteIDs;
        bool _closed;

        public LoadNoteWindow()
        {
            InitializeComponent();
            
            _allNoteIDs = new List<uint>();
            _noteButtons = new List<Button>();
            _tagCheckBoxes = new List<CheckBox>();
            _closed = false;

            AdjustControlsParameters(Width, Height);

            _allNotes = GetNotesNames(Globals.username, Globals.openDiary, ref _noteIDs);

            foreach (string note in _allNotes)
                createNoteButton(note);
            foreach (string tag in GetTagsNames(Globals.username, Globals.openDiary))
                createTagCheckBox(tag);

            foreach (uint id in _noteIDs)
                _allNoteIDs.Add(id);
        }

        //at the beginning "ActualWidth" and "ActualHeight" is equal to 0
        private void AdjustControlsParameters(double winWidth, double winHeight)
        {
            notesScroll.Width = winWidth - 16;
            notesScroll.Height = winHeight - 71;
            tagsScroll.Width = winWidth - 16;
        }

        private List<string> GetNotesNames(string username, string diary, ref List<uint> noteIDs)
        {
            noteIDs = new List<uint>();
            List<string> notesNames = new List<string>();

            string myConnectionString = "server=127.0.0.1; uid=root; pwd=; database=diary";
            MySqlConnection connection = new MySqlConnection(myConnectionString);

            connection.Open();

            string sql = "SELECT notes.id, notes.title FROM notes, diaries, users WHERE users.username LIKE @username AND diaries.name LIKE @diary AND users.id = diaries.users_id AND diaries.id = notes.diaries_id";
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

        private List<string> GetTagsNames(string username, string diary)
        {
            List<string> tags = new List<string>();
            
            string myConnectionString = "server=127.0.0.1; uid=root; pwd=; database=diary";
            MySqlConnection connection = new MySqlConnection(myConnectionString);

            connection.Open();

            string sql = "SELECT DISTINCT tags.text FROM tags, notes, notes_tags, diaries, users WHERE users.username LIKE @username AND diaries.name LIKE @diary AND diaries.users_id = users.id AND notes.diaries_id = diaries.id AND notes_tags.notes_id = notes.id AND notes_tags.tags_id = tags.id";
            MySqlCommand myCommand = new MySqlCommand(sql, connection);
            myCommand.Parameters.AddWithValue("username", username);
            myCommand.Parameters.AddWithValue("diary", diary);

            MySqlDataReader reader = myCommand.ExecuteReader();
            while (reader.Read())
            {
                tags.Add(reader.GetString(0));
            }

            connection.Close();

            foreach (string tag in tags)
                Console.WriteLine(tag);

            return tags;
        }

        private List<string> CheckMatchingNotes(string username, string diary, ref List<uint> noteIDs)
        {
            List<string> matchingNotes = new List<string>();
            noteIDs = new List<uint>();
            List<string> tagList = new List<string>();
            for (int i = 0; i < _tagCheckBoxes.Count; i++)
                if (_tagCheckBoxes[i].IsChecked ?? true)
                    tagList.Add(_tagCheckBoxes[i].Content.ToString());
                    
            string myConnectionString = "server=127.0.0.1; uid=root; pwd=; database=diary";
            MySqlConnection connection = new MySqlConnection(myConnectionString);

            if (tagList.Count > 0)
            {
                foreach (uint note_id in _allNoteIDs)
                {
                    List<string> noteTags = new List<string>();
                    uint id = 0;

                    connection.Open();
                    string sql = "SELECT notes.id, tags.text FROM tags, notes, notes_tags, diaries, users WHERE users.username LIKE @username AND diaries.name LIKE @diary AND notes.id LIKE @note_id AND diaries.users_id = users.id AND notes.diaries_id = diaries.id AND notes_tags.tags_id = tags.id AND notes_tags.notes_id = notes.id";

                    MySqlCommand myCommand = new MySqlCommand(sql, connection);
                    myCommand.Parameters.AddWithValue("username", username);
                    myCommand.Parameters.AddWithValue("diary", diary);
                    myCommand.Parameters.AddWithValue("note_id", note_id);

                    MySqlDataReader reader = myCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        id = reader.GetUInt32(0);
                        noteTags.Add(reader.GetString(1));
                    }
                    connection.Close();

                    bool matched = true;
                    foreach (string tag in tagList)
                        if (!noteTags.Contains(tag))
                            matched = false;
                    if (matched)
                    {
                        string note = "";
                        for (int i = 0; i < _allNoteIDs.Count; i++)
                            if (_allNoteIDs[i] == id)
                            {
                                note = _allNotes[i];
                                break;
                            }
                        noteIDs.Add(id);
                        matchingNotes.Add(note);
                    }
                }
            }
            else
            {
                noteIDs = _allNoteIDs;
                matchingNotes = _allNotes;
            }

            return matchingNotes;
        }

        private void createNoteButton(string name)
        {
            Button button = new Button();
            button.Content = name;
            button.Click += button_Click;

            _noteButtons.Add(button);
            notesStackPanel.Children.Add(button);
        }

        private void removeAllNoteButtons()
        {
            notesStackPanel.Children.Clear();

            _noteButtons.Clear();
            _noteIDs.Clear();
        }

        private void createTagCheckBox(string name)
        {
            CheckBox checkBox = new CheckBox();
            checkBox.Content = name;
            checkBox.Click += tag_Click;

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

        private void tag_Click(object sender, RoutedEventArgs e)
        {
            removeAllNoteButtons();

            List<uint> ids = new List<uint>();
            CheckMatchingNotes(Globals.username, Globals.openDiary, ref ids);
            foreach (uint id in ids)
                for (int i = 0; i < _allNoteIDs.Count; i++)
                    if (_allNoteIDs[i] == id)
                    {
                        createNoteButton(_allNotes[i]);
                        _noteIDs.Add(_allNoteIDs[i]);
                        break;
                    }
        }
    }
}
