namespace DistSysAcwClient.Class
{
    internal class ChangeRole
    {
        public ChangeRole(string userName, string role)
        {
            Username = userName;
            this.Role = role;
        }
        public string Username { get; set; }
        public string Role { get; set; }
    }
}
