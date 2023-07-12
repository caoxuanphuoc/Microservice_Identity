namespace IdentityServer.Models
{
    public class user
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public IList<string> Role { get; set; }
    }
}
