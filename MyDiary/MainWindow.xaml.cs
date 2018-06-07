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

namespace MyDiary
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _closed;
        private Window _signInWindow;

        public MainWindow()
        {
            InitializeComponent();

            _closed = false;

            AdjustControlsParameters(Width, Height);
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

        private void menuNewNote_Click(object sender, RoutedEventArgs e)
        {
            NoteWindow note = new NoteWindow();
            note.Owner = this;
            note.Show();
        }

        private void menuNewDiary_Click(object sender, RoutedEventArgs e)
        {
            ;
        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void menuSignIn_Click(object sender, RoutedEventArgs e)
        {
            bool _winIsOpen = false;

            for (int i = 0; i < OwnedWindows.Count; i++)
            {
                if (OwnedWindows[i].Equals(_signInWindow))
                    _winIsOpen = true;
            }

            if (!_winIsOpen)
            {
                _signInWindow = new SignInWindow();
                _signInWindow.Owner = this;
                _signInWindow.Show();
            }
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
    }
}
