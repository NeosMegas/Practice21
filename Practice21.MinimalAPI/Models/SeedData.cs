using Microsoft.EntityFrameworkCore;
using Practice21.MinimalAPI.Data;

namespace Practice21.MinimalAPI.Models
{
    public static class SeedData
    {
        public static void InitializePhoneBook(IServiceProvider serviceProvider)
        {
            using (var context = new PhoneBookContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<PhoneBookContext>>()))
            {
                if (context.PhoneBookEntries.Any()) return;
                context.PhoneBookEntries.AddRange(
                    new PhoneBookEntry
                    {
                        LastName = "Иванов",
                        FirstName = "Иван",
                        MiddleName = "Иванович",
                        PhoneNumber = 79871234567,
                        Address = "450000, г. Уфа, ул. Ленина, 1, кв. 1",
                        Description = ""
                    },
                    new PhoneBookEntry
                    {
                        LastName = "Петров",
                        FirstName = "Пётр",
                        MiddleName = "Петрович",
                        PhoneNumber = 79658901234,
                        Address = "450001, г. Уфа, проспект Октября, 2, кв. 2",
                        Description = ""
                    },
                    new PhoneBookEntry
                    {
                        LastName = "Сидоров",
                        FirstName = "Сидр",
                        MiddleName = "Сидорович",
                        PhoneNumber = 79435678901,
                        Address = "450002, г. Уфа, проспект Салавата Юлаева, 3, кв. 3",
                        Description = ""
                    },
                    new PhoneBookEntry
                    {
                        LastName = "Бэггинс",
                        FirstName = "Фродо",
                        MiddleName = "Дрогович",
                        PhoneNumber = 79212345678,
                        Address = "450003, г. Уфа, ул. Бэг Энд, 4",
                        Description = "Выдающийся хоббит из Шира, племянник Бильбо Бэггинса."
                    }
                );
                context.SaveChanges();
            }
        }
        
        public static void InitializeUsers(IServiceProvider serviceProvider)
        {
            using (var context = new UserContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<UserContext>>()))
            {
                if (context.Users.Any()) return;
                //context.Users.AddRange(
                //    new User() { Login = "admin", Password = "12345", RoleId = 1 },
                //    new User() { Login = "user", Password = "12345", RoleId = 2 }
                //    );
                //context.SaveChanges();
            }
        }

    }
}
