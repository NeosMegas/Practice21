using Microsoft.AspNetCore.Identity;
using Practice21.MinimalAPI.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Practice21.WPFClient
{
    public class ViewModel : INotifyPropertyChanged
    {
        private User currentUser;

        private ObservableCollection<PhoneBookEntry> phoneBookEntries = new ObservableCollection<PhoneBookEntry>();
        public ObservableCollection<PhoneBookEntry> PhoneBookEntries
        {
            get => phoneBookEntries;
            set
            {
                phoneBookEntries = value;
                NotifyPropertyChanged();
            }
        }
        public ObservableCollection<User> Users { get; set; }
        public ObservableCollection<Role> Roles { get; set; }
        private bool userCanReadOnly = true;
        public bool UserCanReadOnly {
            get => userCanReadOnly;
            private set
            {
                if (userCanReadOnly != value)
                {
                    userCanReadOnly = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private PhoneBookModel model { get; set; }
        public User CurrentUser {
            get => currentUser;
            private set
            {
                if (currentUser.Id != value.Id)
                {
                    currentUser = value;
                    UserCanReadOnly = !(currentUser.RoleId == 1 || currentUser.RoleId == 2);
                    NotifyPropertyChanged();
                }
            }
        }

        public bool GetEntries()
        {
            if (model != null)
                return model.GetEntries();
            else return false;
        }

        private string userRole = string.Empty;
        public string UserRole
        {
            get => userRole;
            private set
            {
                if (userRole != value)
                {
                    userRole = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool canUserAddEntry;
        public bool CanUserAddEntry
        {
            get { return canUserAddEntry; }
            set
            {
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

        public ViewModel()
        {
            model = new PhoneBookModel();
            CurrentUser = currentUser = model.CurrentUser;
            UserRole = model.UserRole;
            PhoneBookEntries = model.PhoneBookEntries;
            Users = model.Users;
            Roles = model.Roles;
            CanUserAddEntry = CurrentUser.RoleId == 1 || CurrentUser.RoleId == 2;
            CanUserEditEntry = CurrentUser.RoleId == 1;
            CanUserDeleteEntry = CurrentUser.RoleId == 1;
            UserCanReadOnly = !(CurrentUser.RoleId == 1);
            UserIsAdmin = CurrentUser.RoleId == 1;
        }

        private void LinkToModel()
        {
            if (model != null)
            {
                CurrentUser = currentUser = model.CurrentUser;
                UserRole = model.UserRole;
                PhoneBookEntries = model.PhoneBookEntries;
                Users = model.Users;
                Roles = model.Roles;
                CanUserAddEntry = CurrentUser.RoleId == 1 || CurrentUser.RoleId == 2;
                CanUserEditEntry = CurrentUser.RoleId == 1;
                CanUserDeleteEntry = CurrentUser.RoleId == 1;
                UserCanReadOnly = !(CurrentUser.RoleId == 1);
                UserIsAdmin = CurrentUser.RoleId == 1;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool LoginUser(string login, string password)
        {
            bool success = model.LoginUser(login, password);
            LinkToModel();
            return success;
        }

        public bool LogOutUser()
        {
            bool success = model.LogoutUser();
            LinkToModel();
            return success;
        }

        public bool AddEntry(PhoneBookEntry entry)
        {
            bool result = false;
            if (entry != null)
            {
                result = model.AddEntry(entry);
                LinkToModel();
            }
            return result;
        }

        public bool DeleteEntry(PhoneBookEntry? entry)
        {
            bool result = false;
            if (entry != null)
            {
                result = model.DeleteEntry(entry);
                LinkToModel();
            }
            return result;
        }

        public bool UpdateEntry(PhoneBookEntry? entry)
        {
            bool result = false;
            if (entry != null)
            {
                result = model.UpdateEntry(entry);
                LinkToModel();
            }
            return result;
        }

        public bool AddUser(User user)
        {
            bool result = false;
            if (user != null)
            {
                LinkToModel();
                result = model.AddUser(user);
            }
            return result;
        }

        public bool DeleteUser(User user)
        {
            bool result = false;
            if (user != null)
            {
                LinkToModel();
                result = model.DeleteUser(user);
            }
            return result;
        }

        public bool UpdateUser(User user)
        {
            bool result = false;
            if (user != null)
            {
                LinkToModel();
                result = model.UpdateUser(user);
            }
            return result;
        }

    }
}