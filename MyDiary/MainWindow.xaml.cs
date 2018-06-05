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
        public MainWindow()
        {
            InitializeComponent();
            AdjustControlsParameters();
        }

        private void AdjustControlsParameters()
        {
            menu.Width = ActualWidth;
        }

        private void menuNewNote_Click(object sender, RoutedEventArgs e)
        {
            ;
        }

        private void menuNewDiary_Click(object sender, RoutedEventArgs e)
        {
            ;
        }

        private void Close()
        {
            Window win = GetWindow(this);
            win.Close();
        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void menuAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("MyDiary is a virtual program for storing notes in diaries.", "MyDiary");
        }

        private void menuVersion_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("v.0.0.1", "MyDiary");
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            AdjustControlsParameters();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            AdjustControlsParameters();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Close();
        }
    }
}
