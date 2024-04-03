using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
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
using Practice21.MinimalAPI.Models;

namespace Practice21.WPFClient
{
    public partial class UserInfoWindow : Window, INotifyPropertyChanged
    {
        public ViewModel ViewModel { get; set;}
        public User? User { get; set;}
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

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public UserInfoWindow(ViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (User != null)
                User.RoleId = ((Role)cmbRoles.SelectedItem).Id;
            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void txt_TextChanged(object sender, TextChangedEventArgs e) => CheckMinimalInfo();
        private void cmbRoles_SelectionChanged(object sender, SelectionChangedEventArgs e) => CheckMinimalInfo();

        private void txt_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtLogin.Focus();
            if (User?.RoleId > 0)
                cmbRoles.SelectedItem = ViewModel.Roles.Where(r => r.Id == User?.RoleId).FirstOrDefault();
            else
                cmbRoles.SelectedItem = ViewModel.Roles.Where(r => r.Id == 2).FirstOrDefault();
        }

        private void CheckMinimalInfo()
        {
            MinimalInfo = txtLogin.Text.Length > 0 && txtPassword.Text.Length > 0 && cmbRoles.SelectedIndex >= 0;
        }
    }
}
