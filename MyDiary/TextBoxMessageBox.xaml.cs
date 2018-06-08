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

namespace MyDiary
{
    /// <summary>
    /// Interaction logic for TextBoxMessageBox.xaml
    /// </summary>
    public partial class TextBoxMessageBox : Window
    {
        public TextBoxMessageBox(string description, string title="")
        {
            InitializeComponent();

            Globals.tbMessageBoxResult = MessageBoxResult.None;

            Title = title;
            descriptionLabel.Content = description;
        }

        public void ShowDialog()
        {
            Window win = this;
            win.ShowDialog();
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Globals.textBoxMessageBox = textBox.Text;
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            Globals.tbMessageBoxResult = MessageBoxResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Globals.tbMessageBoxResult = MessageBoxResult.Cancel;
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Globals.tbMessageBoxResult = MessageBoxResult.OK;
                Close();
            }
        }
    }
}
