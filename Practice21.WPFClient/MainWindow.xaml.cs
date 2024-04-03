using Practice21.MinimalAPI.Models;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Practice21.WPFClient
{
    public partial class MainWindow : Window, IView, INotifyPropertyChanged
    {
        private bool isLoginButtonEnabled = false;
        public bool IsLoginButtonEnabled {
            get => isLoginButtonEnabled;
            set
            {
                if (isLoginButtonEnabled != value)
                {
                    isLoginButtonEnabled = value || IsUserLoggedIn;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool isUserLoggedIn = false;
        public bool IsUserLoggedIn {
            get => isUserLoggedIn;
            set {
                if (isUserLoggedIn != value) {
                    isUserLoggedIn = value;
                    IsLoginButtonEnabled = isUserLoggedIn;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool userCanReadOnly = true;
        public bool UserCanReadOnly { get => userCanReadOnly;
            private set {
                if (userCanReadOnly != value)
                {
                    userCanReadOnly = !(currentUser.RoleId == 1 || currentUser.RoleId == 2);
                    NotifyPropertyChanged();
                }
            }
        }
        
        public ViewModel ViewModel { get; set; }
        private User currentUser;
        public User CurrentUser {
            get => currentUser;
            set {
                if (currentUser != value)
                {
                    currentUser = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string userRole = string.Empty;
        public string UserRole { get { return userRole; }
            set {
                if (userRole != value)
                {
                    userRole = value;
                    NotifyPropertyChanged();
                }
            } }

        private bool canUserAddEntry;
        public bool CanUserAddEntry
        {
            get { return canUserAddEntry; }
            set {
                if (canUserAddEntry != value)
                canUserAddEntry = value;
                NotifyPropertyChanged();
            }
        }

        private bool canUserDeleteEntry;
        public bool CanUserDeleteEntry
        {
            get { return canUserDeleteEntry; }
            set
            {
                if (canUserDeleteEntry != value)
                    canUserDeleteEntry = value;
                NotifyPropertyChanged();
            }
        }

        private bool canUserEditEntry;
        public bool CanUserEditEntry
        {
            get { return canUserEditEntry; }
            set
            {
                if (canUserEditEntry != value)
                    canUserEditEntry = value;
                NotifyPropertyChanged();
            }
        }

        private bool userIsAdmin;
        public bool UserIsAdmin
        {
            get { return userIsAdmin; }
            set
            {
                if (userIsAdmin != value)
                    userIsAdmin = value;
                NotifyPropertyChanged();
            }
        }

        private PhoneBookEntry? currentEntry = null;

        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new ViewModel();
            CurrentUser = ViewModel.CurrentUser;
            UserRole = ViewModel.UserRole;
            currentUser = ViewModel.CurrentUser;
            CanUserAddEntry = ViewModel.CanUserAddEntry;
            CanUserEditEntry = ViewModel.CanUserEditEntry;
            CanUserDeleteEntry = ViewModel.CanUserDeleteEntry;
            UserCanReadOnly = ViewModel.UserCanReadOnly;
            UserIsAdmin = ViewModel.UserIsAdmin;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            bool success;
            if (!IsUserLoggedIn)
            {
                success = ViewModel.LoginUser(txtLogin.Text, txtPassword.Text);
                if (success)
                {
                    btnLogin.Content = "Выйти";
                    txtLogin.Clear();
                    txtPassword.Clear();
                    txtLogin.Visibility = Visibility.Collapsed;
                    txtPassword.Visibility = Visibility.Collapsed;
                    IsUserLoggedIn = true;
                }
                else
                    FlashTextBoxes();
            }
            else
            {
                success = ViewModel.LogOutUser();
                if (success)
                {
                    btnLogin.Content = "Войти";
                    txtLogin.Visibility = Visibility.Visible;
                    txtPassword.Visibility = Visibility.Visible;
                    IsUserLoggedIn = false;
                }
            }
            CurrentUser = ViewModel.CurrentUser;
            UserRole = ViewModel.UserRole;
            UserCanReadOnly = ViewModel.UserCanReadOnly;
            CanUserAddEntry = ViewModel.CanUserAddEntry;
            CanUserEditEntry = ViewModel.CanUserEditEntry;
            CanUserDeleteEntry = ViewModel.CanUserDeleteEntry;
            UserCanReadOnly = ViewModel.UserCanReadOnly;
            UserIsAdmin = ViewModel.UserIsAdmin;
            ViewModel.GetEntries();
        }

        private void btnAddEntry_Click(object sender, RoutedEventArgs e)
        {
            if (!CanUserAddEntry) return;
            AddEntryWindow addEntryWindow = new AddEntryWindow();
            if (addEntryWindow.ShowDialog() == true)
            {
                if(!ViewModel.AddEntry(new PhoneBookEntry(){
                    LastName = addEntryWindow.txtLastName.Text,
                    FirstName = addEntryWindow.txtFirstName.Text,
                    MiddleName = addEntryWindow.txtMiddleName.Text,
                    PhoneNumber = addEntryWindow.PhoneNumber,
                    Address = addEntryWindow.txtAddress.Text,
                    Description = addEntryWindow.txtDescription.Text
                }))
                    MessageBox.Show("Ошибка добавления записи.");
            }
        }


        private void btnDeleteEntry_Click(object sender, RoutedEventArgs e)
        {
            if (!CanUserDeleteEntry) return;
            if (lstEntries.SelectedItems.Count > 0)
            {
                PhoneBookEntry? selectedEntry = lstEntries.SelectedItem as PhoneBookEntry;
                if (MessageBox.Show($"Удалить запись {selectedEntry?.Name}?", "Удаление записи", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    if (lstEntries.SelectedIndex > 0) lstEntries.SelectedIndex--;
                    if(!ViewModel.DeleteEntry(selectedEntry))
                        MessageBox.Show("Ошибка удаления записи.");
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!CanUserEditEntry) return;
            PhoneBookEntry? selectedEntry = lstEntries.SelectedItem as PhoneBookEntry;
            if (selectedEntry != currentEntry)
                if (!ViewModel.UpdateEntry(selectedEntry))
                    MessageBox.Show("Ошибка обновления записи.");
        }

        private void btnEditUsers_Click(object sender, RoutedEventArgs e)
        {
            UsersWindow usersWindow = new UsersWindow(ViewModel);
            usersWindow.ShowDialog();
        }

        private void lstEntries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstEntries.SelectedItem != null)
                currentEntry = new PhoneBookEntry(lstEntries.SelectedItem as PhoneBookEntry);
        }

        #region TextBoxEvents

        private void txtPhoneNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBoxNumberChecker.txtPhoneNumber_PreviewTextInput(sender, e);
        }

        private void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            TextBoxNumberChecker.TextBoxPasting(sender, e);
        }

        private void TextBox_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            DependencyObject parent = VisualTreeHelper.GetParent((DependencyObject)sender);
            while (parent != null)
            {
                if (parent is ListBoxItem)
                    break;
                parent = VisualTreeHelper.GetParent(parent);
            }
            if (parent != null)
                ((ListBoxItem)parent).IsSelected = true;
        }

        private void LoginTextBoxes_GotFocus(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).SelectAll();
        }

        private void FlashTextBoxes()
        {
            Storyboard storyboard = new Storyboard();
            ColorAnimation colorAnimation = new ColorAnimation();
            colorAnimation.To = Colors.Red;
            colorAnimation.Duration = TimeSpan.FromSeconds(0.2);
            colorAnimation.AutoReverse = true;
            Storyboard.SetTargetProperty(colorAnimation, new PropertyPath("(TextBox.Background).(SolidColorBrush.Color)", null));
            storyboard.Children.Add(colorAnimation);
            storyboard.Begin(txtLogin);
            storyboard.Begin(txtPassword);
        }

        private void txtLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            IsLoginButtonEnabled = IsUserLoggedIn || (txtLogin.Text.Length > 0 && txtPassword.Text.Length > 0);
        }
        #endregion

    }

}