using Microsoft.AspNetCore.Identity;

namespace InsureYou_AI.Entities
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public string City { get; set; }
        public string Title { get; set; }
        public string Education { get; set; }
        public List<Comment> Comments { get; set; }
    }
}
