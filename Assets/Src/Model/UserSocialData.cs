public class UserSocialData
{
    public readonly string Uid;
    public readonly string FirstName;
    public readonly string LastName;
    public readonly string Picture50Url;

    public UserSocialData(string uid, string firstName, string lastName, string picture50Url)
    {
        Uid = uid;
        FirstName = firstName;
        LastName = lastName;
        Picture50Url = picture50Url;
    }
}
