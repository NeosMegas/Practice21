using Practice21.MinimalAPI.Models;
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

namespace Practice21.WPFClient
{
    public partial class UsersWindow : Window
    {
        public ViewModel ViewModel { get; set; }

        public UsersWindow(ViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            UserInfoWindow userInfoWindow = new UserInfoWindow(ViewModel) {
                User = dgUsers.SelectedItem as User
            };
            if (userInfoWindow.ShowDialog() == true)
            {
                if (userInfoWindow.User != null && !ViewModel.UpdateUser(userInfoWindow.User))
                    MessageBox.Show("Ошибка редактирования пользователя");
                CollectionViewSource.GetDefaultView(dgUsers.ItemsSource).Refresh();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (DataGridColumn c in dgUsers.Columns)
                c.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            UserInfoWindow userInfoWindow = new UserInfoWindow(ViewModel)
            {
                User = new User()
            };

            if (userInfoWindow.ShowDialog() == true)
                if (!ViewModel.AddUser(userInfoWindow.User))
                    MessageBox.Show("Ошибка добавления нового пользователя.");
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsers.SelectedItem != null)
            {
                User? selectedUser;
                selectedUser = dgUsers.SelectedItem as User;
                if (selectedUser != null)
                    if (MessageBox.Show($"Удалить {selectedUser.Login}?", "Удаление пользователя", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        if(!ViewModel.DeleteUser(selectedUser))
                            MessageBox.Show("Ошибка удаления пользователя.");
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    DialogResult = true;
                    Close();
                    break;
            }
        }
    }
}
