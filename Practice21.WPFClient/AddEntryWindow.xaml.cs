using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Practice21.WPFClient
{
    /// <summary>
    /// Interaction logic for AddEntryWindow.xaml
    /// </summary>
    public partial class AddEntryWindow : Window, INotifyPropertyChanged
    {
        private bool minimalInfo = false;

        public bool MinimalInfo
        {
            get { return minimalInfo; }
            set {
                if (minimalInfo != value) {
                    minimalInfo = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        public long PhoneNumber
        {
            get {
                string stringPhoneNumber = Regex.Replace(txtPhoneNumber.Text, @"[\s()\-+]", "");
                return stringPhoneNumber.Length>0 ? Convert.ToInt64(stringPhoneNumber) : 0;
            }
        }


        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public AddEntryWindow()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            MinimalInfo = (txtFirstName.Text.Length > 0 || txtLastName.Text.Length > 0) && PhoneNumber > 0;
        }

        private void txt_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
        }

        private void txtPhoneNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBoxNumberChecker.txtPhoneNumber_PreviewTextInput(sender, e);
        }

        private void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            TextBoxNumberChecker.TextBoxPasting(sender, e);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtLastName.Focus();
        }
    }
}
