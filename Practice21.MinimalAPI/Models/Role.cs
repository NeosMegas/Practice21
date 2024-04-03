namespace Practice21.MinimalAPI.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public Role(string name, string displayName)
        {
            Name = name;
            DisplayName = displayName;
        }
        public List<User> Users { get; set; } = new List<User>();
    }
}
