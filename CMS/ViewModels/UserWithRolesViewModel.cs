namespace CMS.ViewModels
{
    public class UserWithRolesViewModel
    {
        public string UserId { get; set; } 
        public string FullName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; } = new();
        public string SelectedRole { get; set; }
    }
}
