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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using MySql.Data.MySqlClient;

namespace MyDiary
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private enum WinType { WinNote, WinLoadNote, WinSignIn, WinRegister };

        private List<Button> diaryButtons;
        private List<Button> noteButtons;
        private DispatcherTimer _timer;
        private string _diariesFromUser;
        private bool _hasTextBoxMessageBox;
        private bool _closed;

        public MainWindow()
        {
            InitializeComponent();

            diaryButtons = new List<Button>();
            noteButtons = new List<Button>();

            _timer = new DispatcherTimer();
            _timer.Tick += tick;
            _timer.Interval = new TimeSpan(0, 0, 1);
            _timer.Start();

            _diariesFromUser = "";
            _hasTextBoxMessageBox = false;
            _closed = false;

            AdjustControlsParameters(Width, Height);
        }

        private void tick(object sender, EventArgs e)
        {
            if (_hasTextBoxMessageBox && Globals.tbMessageBoxResult != MessageBoxResult.None)
            {
                _hasTextBoxMessageBox = false;

                if (Globals.tbMessageBoxResult == MessageBoxResult.OK)
                {
                    string diaryName = Globals.textBoxMessageBox;
                    if (createDiary(diaryName))
                        createDiaryButton(diaryName);
                }
            }

            if (_diariesFromUser != Globals.username)
            {
                if (Globals.logged)
                {
                    Title = Globals.username + " - MyDiary";
                    _diariesFromUser = Globals.username;
                    usernameLabel.Content = Globals.username;

                    List<string> diariesNames = getDiariesNames(_diariesFromUser);
                    foreach (string name in diariesNames)
                        createDiaryButton(name);
                    
                    menuNewDiary.IsEnabled = true;
                    menuSignIn.IsEnabled = false;
                    menuRegister.IsEnabled = false;
                    menuSignOut.IsEnabled = true;
                }
            }
            else if (!Globals.logged)
            {
                usernameLabel.Content = "You're not logged in";
            }
        }

        private List<string> getDiariesNames(string username)
        {
            List<string> diariesNames = new List<string>();

            string myConnectionString = "server=127.0.0.1; uid=root; pwd=; database=diary";
            MySqlConnection connection = new MySqlConnection(myConnectionString);

            connection.Open();

            string sql = "SELECT diaries.name FROM diaries, users WHERE users.username LIKE @username AND users.id = diaries.users_id";
            MySqlCommand myCommand = new MySqlCommand(sql, connection);
            myCommand.Parameters.AddWithValue("username", username);

            MySqlDataReader reader = myCommand.ExecuteReader();
            while (reader.Read())
            {
                diariesNames.Add(reader.GetString("name"));
            }

            connection.Close();

            return diariesNames;
        }

        private bool doesDiaryExists(string diaryName)
        {
            string myConnectionString = "server=127.0.0.1; uid=root; pwd=; database=diary";
            MySqlConnection connection = new MySqlConnection(myConnectionString);

            connection.Open();

            string sql = "SELECT name, username FROM diaries, users WHERE diaries.name LIKE @diaryName AND users.username LIKE @username AND diaries.users_id = users.id";
            MySqlCommand myCommand = new MySqlCommand(sql, connection);
            myCommand.Parameters.AddWithValue("diaryName", diaryName);
            myCommand.Parameters.AddWithValue("username", Globals.username);

            MySqlDataReader reader = myCommand.ExecuteReader();
            bool exists = false;
            while (reader.Read())
            {
                if (diaryName.ToUpper() == reader.GetString("name").ToUpper() && Globals.username == reader.GetString("username"))
                {
                    exists = true;
                    break;
                }
            }

            connection.Close();

            return exists;
        }

        private bool createDiary(string diaryName)
        {
            if (Globals.logged && !doesDiaryExists(diaryName))
            {
                string myConnectionString = "server=127.0.0.1; uid=root; pwd=; database=diary";
                MySqlConnection connection = new MySqlConnection(myConnectionString);

                connection.Open();

                string sql = "INSERT INTO diaries(users_id, name) SELECT users.id, @diaryName FROM users WHERE username LIKE @username";
                MySqlCommand myCommand = new MySqlCommand(sql, connection);
                myCommand.Parameters.AddWithValue("username", Globals.username);
                myCommand.Parameters.AddWithValue("diaryName", diaryName);
                myCommand.ExecuteNonQuery();

                connection.Close();

                return true;
            }
            return false;
        }

        private void createDiaryButton(string name)
        {
            Button button = new Button();
            button.Content = name;
            button.Click += diaryButton_Click;

            diaryButtons.Add(button);
            diariesStackPanel.Children.Add(button);
        }

        //at the beginning "ActualWidth" and "ActualHeight" is equal to 0
        private void AdjustControlsParameters(double winWidth, double winHeight)
        {
            menu.Width = winWidth;
        }
        
        private void Close()
        {
            for (int i = 0; i < OwnedWindows.Count; i++)
            {
                Window win = OwnedWindows[i];
                win.Close();
            }

            if (!_closed && OwnedWindows.Count == 0)
            {
                _closed = true;
                Window win = this;
                win.Close();
            }
        }

        private void Close(System.ComponentModel.CancelEventArgs e)
        {
            if (!_closed)
            {
                _closed = true;
                Close();
                if (OwnedWindows.Count > 0)
                {
                    e.Cancel = true;
                    _closed = false;
                }
            }
        }

        private void createWindow(WinType type)
        {
            Window win = null;

            switch (type)
            {
                case WinType.WinNote:
                case WinType.WinLoadNote:
                    if (type == WinType.WinNote)
                        win = new NoteWindow();
                    else
                        win = new LoadNoteWindow();
                    win.Owner = this;
                    win.Show();
                    break;
                case WinType.WinSignIn:
                case WinType.WinRegister:
                    for (int i = OwnedWindows.Count - 1; i >= 0; i--)
                        OwnedWindows[i].Close();
                    if (OwnedWindows.Count > 0)
                        break;

                    if (type == WinType.WinSignIn)
                        win = new SignInWindow();
                    else
                        win = new RegisterWindow();

                    win.Owner = this;
                    win.ShowDialog();
                    break;
            }
        }

        private void uncheckTheDiaryButtons()
        {
            foreach (Button button in diaryButtons)
                button.Background = new SolidColorBrush(Color.FromRgb(221, 221, 221));
        }

        private void menuNewNote_Click(object sender, RoutedEventArgs e)
        {
            createWindow(WinType.WinNote);
        }

        private void menuLoadNote_Click(object sender, RoutedEventArgs e)
        {
            createWindow(WinType.WinLoadNote);
        }

        private void menuNewDiary_Click(object sender, RoutedEventArgs e)
        {
            TextBoxMessageBox mb = new TextBoxMessageBox("Enter new diary's name:", "New diary");
            mb.ShowDialog();
            _hasTextBoxMessageBox = true;
        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void menuSignIn_Click(object sender, RoutedEventArgs e)
        {
            createWindow(WinType.WinSignIn);
        }

        private void menuRegister_Click(object sender, RoutedEventArgs e)
        {
            createWindow(WinType.WinRegister);
        }

        private void menuAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("MyDiary is a virtual program for storing notes in diaries.", "MyDiary");
        }

        private void menuVersion_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("v. pre-alpha", "MyDiary");
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            AdjustControlsParameters(ActualWidth, ActualHeight);
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            AdjustControlsParameters(ActualWidth, ActualHeight);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Close(e);
        }

        private void menuSignOut_Click(object sender, RoutedEventArgs e)
        {
            Globals.logged = false;
            Globals.username = "";
            Globals.openDiary = "";

            _diariesFromUser = "";
            Title = "MyDiary";

            menuNewNote.IsEnabled = false;
            menuLoadNote.IsEnabled = false;
            menuNewDiary.IsEnabled = false;
            menuSignIn.IsEnabled = true;
            menuRegister.IsEnabled = true;
            menuSignOut.IsEnabled = false;

            diariesStackPanel.Children.Clear();
            diaryButtons.Clear();
            noteButtons.Clear();
        }

        private void diaryButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Globals.openDiary = button.Content.ToString();

            uncheckTheDiaryButtons();
            button.Background = Brushes.LightCyan;

            menuNewNote.IsEnabled = true;
            menuLoadNote.IsEnabled = true;
        }
    }
}
