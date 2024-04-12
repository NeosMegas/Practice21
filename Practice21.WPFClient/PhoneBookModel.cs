using Newtonsoft.Json;
using Practice21.MinimalAPI.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;

namespace Practice21.WPFClient
{
    public class PhoneBookModel
    {
        public ObservableCollection<PhoneBookEntry> PhoneBookEntries { get; set; }
        public ObservableCollection<User> Users { get; set; }
        public ObservableCollection<Role> Roles { get; set; }
        private readonly User anonymousUser = new User()
        {
            Id = 0,
            Login = "гость",
            Password = "",
            RoleId = 0
        };
        public User CurrentUser { get; set; }
        private string userRole = "гость";
        public string UserRole {
            get => userRole;
            set { if (userRole != value)
                    userRole = value;
            }
        }
        private string baseUrl = @"https://localhost:7243/";

        private HttpClient httpClient = new HttpClient();

        public PhoneBookModel()
        {
            CurrentUser = anonymousUser;
            if (!GetEntries())
                throw new Exception("Ошибка связи с базой данных.");
            PhoneBookEntries ??= [];
            Users = [];
            Roles = [];
        }

        #region InitializeCollections
        public bool GetEntries()
        {
            var httpResponse = httpClient.GetAsync(baseUrl + "api/PhoneBookEntry");
            if (httpResponse.Result.StatusCode == HttpStatusCode.OK)
            {
                string json = httpResponse.Result.Content.ReadAsStringAsync().Result;
                PhoneBookEntries = JsonConvert.DeserializeObject<ObservableCollection<PhoneBookEntry>>(json) ?? new ObservableCollection<PhoneBookEntry>();
                return true;
            }
            return false;
        }

        private bool GetUsers()
        {
            var httpResponse = httpClient.GetAsync(baseUrl + "api/Users");
            if (httpResponse.Result.StatusCode == HttpStatusCode.OK)
            {
                string json = httpResponse.Result.Content.ReadAsStringAsync().Result;
                Users = JsonConvert.DeserializeObject<ObservableCollection<User>>(json) ?? new ObservableCollection<User>();
            }
            else
                return false;

            httpResponse = httpClient.GetAsync(baseUrl + "api/Users/Roles");
            if (httpResponse.Result.StatusCode == HttpStatusCode.OK)
            {
                string json = httpResponse.Result.Content.ReadAsStringAsync().Result;
                Roles = JsonConvert.DeserializeObject<ObservableCollection<Role>>(json) ?? new ObservableCollection<Role>();
                return true;
            }
            return false;
        }
        #endregion

        #region WorkWithEntries
        public bool AddEntry(PhoneBookEntry entry)
        {
            if (entry != null)
            {
                CheckEntry(ref entry);
                var httpResponse = httpClient.PostAsJsonAsync(baseUrl + "api/PhoneBookEntry", entry);
                Debug.WriteLine(httpResponse.Result.Content.ReadAsStringAsync().Result);
                if (!httpResponse.Result.IsSuccessStatusCode) return false;
                string json = httpResponse.Result.Content.ReadAsStringAsync().Result;
                if (json == string.Empty) return false;
                PhoneBookEntry? newEntry = JsonConvert.DeserializeObject<PhoneBookEntry>(json);
                if (newEntry == null) return false;
                PhoneBookEntries.Add(newEntry);
                return true;
            }
            return false;
        }

        public bool DeleteEntry(PhoneBookEntry entry)
        {
            if (entry != null && PhoneBookEntries.Where(e => e.Id == entry.Id).Any())
            {
                var httpResponse = httpClient.DeleteAsync(baseUrl + "api/PhoneBookEntry/" + entry.Id);
                Debug.WriteLine(httpResponse.Result.Content.ReadAsStringAsync().Result);
                if (!httpResponse.Result.IsSuccessStatusCode) return false;
                PhoneBookEntries.Remove(entry);
                return true;
            }
            return false;
        }

        public bool UpdateEntry(PhoneBookEntry entry)
        {
            if (entry != null && PhoneBookEntries.Where(e => e.Id == entry.Id).Any())
            {
                CheckEntry(ref entry);
                var httpResponse = httpClient.PutAsJsonAsync(baseUrl + "api/PhoneBookEntry/" + entry.Id, entry);
                Debug.WriteLine(httpResponse.Result.Content.ReadAsStringAsync().Result);
                if (!httpResponse.Result.IsSuccessStatusCode) return false;
                PhoneBookEntry editingEntry = PhoneBookEntries.Where(e => e.Id == entry.Id).First();
                editingEntry.LastName = entry.LastName;
                editingEntry.FirstName = entry.FirstName;
                editingEntry.MiddleName = entry.MiddleName;
                editingEntry.PhoneNumber = entry.PhoneNumber;
                editingEntry.Address = entry.Address;
                editingEntry.Description = entry.Description;
                return true;
            }
            return false;
        }

        void CheckEntry(ref PhoneBookEntry entry)
        {
            entry.LastName ??= "";
            entry.FirstName ??= "";
            entry.MiddleName ??= "";
            entry.Address ??= "";
            entry.Description ??= "";
            if (entry.LastName == "" && entry.FirstName == "" && entry.MiddleName == "") entry.FirstName = "RandomName" + Random.Shared.Next();
        }
        #endregion

        #region WorkWithUsers

        public string GetUserRole(User user)
        {
            switch (user.RoleId)
            {
                case 0:
                    return "гость";
                default:
                    var role = Roles.Where(r => r.Id == user.RoleId);
                    if (role.Any())
                        return role.First().DisplayName;
                    else
                        return "гость";

            }
        }

        public bool LoginUser(string login, string password)
        {
            var userInfo = new List<KeyValuePair<string, string>>
            {
                new("login", login),
                new("password", password)
            };
            var formContent = new FormUrlEncodedContent(userInfo);
            var httpResponse = httpClient.PostAsync(baseUrl + "login", formContent);
            Debug.WriteLine(httpResponse.Result.Content.ReadAsStringAsync().Result);
            if (httpResponse.Result.StatusCode != HttpStatusCode.OK) return false;
            string json = httpResponse.Result.Content.ReadAsStringAsync().Result;
            if (json == string.Empty) return false;
            User loggingInUser = JsonConvert.DeserializeObject<User>(json) ?? anonymousUser;
            if (loggingInUser == anonymousUser || loggingInUser == null) return false;
            if (loggingInUser.RoleId == 1)
                GetUsers();
            if (loggingInUser != null && loggingInUser.Id != 0)
            {
                CurrentUser = loggingInUser;
                UserRole = GetUserRole(loggingInUser);
                return true;
            }
            else
                return false;
        }
        public bool LogoutUser()
        {
            var httpResponse = httpClient.GetAsync(baseUrl + "logout");
            Debug.WriteLine(httpResponse.Result.Content.ReadAsStringAsync().Result);
            if (httpResponse.Result.StatusCode != HttpStatusCode.OK) return false;
            if (CurrentUser.Id == 0)
                return false;
            else
            {
                CurrentUser = anonymousUser;
                UserRole = GetUserRole(CurrentUser);
                return true;
            }
        }
        
        public bool AddUser(User user)
        {
            if (user != null)
            {
                CheckUser(ref  user);
                var httpResponse = httpClient.PostAsJsonAsync(baseUrl + "api/Users", user);
                Debug.WriteLine(httpResponse.Result.Content.ReadAsStringAsync().Result);
                if (!httpResponse.Result.IsSuccessStatusCode) return false;
                string json = httpResponse.Result.Content.ReadAsStringAsync().Result;
                if (json == string.Empty) return false;
                User? newUser = JsonConvert.DeserializeObject<User>(json);
                if (newUser == null) return false;
                Users.Add(newUser);
                return true;
            }
            return false;
        }

        public bool DeleteUser(User user)
        {
            if (user != null && Users.Where(u => u.Id == user.Id).Any())
            {
                var httpResponse = httpClient.DeleteAsync(baseUrl + "api/Users/" + user.Id);
                Debug.WriteLine(httpResponse.Result.Content.ReadAsStringAsync().Result);
                if (!httpResponse.Result.IsSuccessStatusCode) return false;
                Users.Remove(user);
                return true;
            }
            return false;
        }

        public bool UpdateUser(User user)
        {
            if (user != null && Users.Where(e => e.Id == user.Id).Any())
            {
                CheckUser(ref user);
                var httpResponse = httpClient.PutAsJsonAsync(baseUrl + "api/Users/" + user.Id, user);
                Debug.WriteLine(httpResponse.Result.Content.ReadAsStringAsync().Result);
                if (!httpResponse.Result.IsSuccessStatusCode) return false;
                User editingUser = Users.Where(e => e.Id == user.Id).First();
                editingUser.Login = user.Login;
                editingUser.Password = user.Password;
                editingUser.RoleId = user.RoleId;
                return true;
            }
            return false;
        }

        void CheckUser(ref User user)
        {
            string currentUserLogin = user.Login ?? "";
            if (currentUserLogin == null || currentUserLogin == string.Empty) currentUserLogin = "user" + Random.Shared.Next();
            while (Users.Where(u=>u.Login == currentUserLogin).Any()) currentUserLogin = currentUserLogin + Random.Shared.Next();
            user.Login = currentUserLogin;
            if (user.Password == null || user.Password == string.Empty) user.Password = Random.Shared.Next(0, 99999999).ToString("00000000");
        }

        #endregion

    }
}