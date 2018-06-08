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

namespace MyDiary
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private enum WinType { WinNote, WinSignIn, WinRegister };

        private DispatcherTimer _timer;
        private bool _hasTextBoxMessageBox;
        private bool _closed;
        
        public MainWindow()
        {
            InitializeComponent();

            _timer = new DispatcherTimer();
            _timer.Tick += tick;
            _timer.Interval = new TimeSpan(0, 0, 1);
            _timer.Start();

            _hasTextBoxMessageBox = false;
            _closed = false;

            AdjustControlsParameters(Width, Height);
        }

        private void tick(object sender, EventArgs e)
        {
            if (Globals.logged)
            {
                Title = Globals.username + " - MyDiary";

                menuSignIn.IsEnabled = false;
                menuRegister.IsEnabled = false;
                menuSignOut.IsEnabled = true;
            }

            if (_hasTextBoxMessageBox && Globals.tbMessageBoxResult != MessageBoxResult.None)
            {
                _hasTextBoxMessageBox = false;

                if (Globals.tbMessageBoxResult == MessageBoxResult.OK)
                {
                    string diaryName = Globals.textBoxMessageBox;
                    menuFile.Header = diaryName;
                }
            }
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
                    win = new NoteWindow();
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

        private void menuNewNote_Click(object sender, RoutedEventArgs e)
        {
            createWindow(WinType.WinNote);
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

            Title = "MyDiary";

            menuSignIn.IsEnabled = true;
            menuRegister.IsEnabled = true;
            menuSignOut.IsEnabled = false;
        }
    }
}
