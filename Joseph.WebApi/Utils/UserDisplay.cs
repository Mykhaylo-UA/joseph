using Joseph.Data.Entities;

namespace Joseph.WebApi.Utils;

public static class UserDisplay
{
    public static string Create(User user)
    {
        string result = "";
        if (!string.IsNullOrEmpty(user.Surname))
        {
            result += user.Surname + " ";
            if (!string.IsNullOrEmpty(user.Name))
            {
                result += user.Name + " ";
                if (!string.IsNullOrEmpty(user.Other))
                {
                    result += user.Other;
                }
            }
            
        }
        else
        {
            result = $"{user.Email}";
        }

        return result;
    }
}