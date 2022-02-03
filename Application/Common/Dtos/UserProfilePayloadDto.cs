namespace CleanArchitecture.Application.Common.Dtos
{
    public class UserProfilePayloadDto
    {
        public UserProfile UserProfile { get; set; }
    }

    public class UserProfile
    {
        public string Password { get; set; }
        public string Application { get; set; } = "MobileFleetApp";
        public string Expires { get; set; } = "0";
        public string ReturnProfile { get; set; } = "0";
    }
}
