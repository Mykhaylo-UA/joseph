using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Reflection;
using ChristyAlsop.Wasm.Data.Attributes;

namespace Joseph.Client.Utils;

public static class CustomValidator
{
    public static Dictionary<string, IEnumerable<string>> Validate(object validateObject)
    {
        Dictionary<string, IEnumerable<string>> errors = new Dictionary<string, IEnumerable<string>>();
        foreach (PropertyInfo propertyInfo in validateObject.GetType().GetProperties())
        {
            errors.Add(propertyInfo.Name, CheckAttributes(propertyInfo, validateObject));
        }

        return errors;
    }

    static IEnumerable<string> CheckAttributes(PropertyInfo propertyInfo, object validateObject)
    {
        foreach (object attribute in propertyInfo.GetCustomAttributes())
            {
                switch (attribute)
                {
                    case RequiredAttribute requiredAttribute:
                        object value = propertyInfo.GetValue(validateObject);
                        if (value is null || ($"{value}").Trim() == "")
                        {
                            yield return requiredAttribute.ErrorMessage ?? $"{propertyInfo.Name} is required property.";
                        }
                        break;
                    case EmailAddressAttribute emailAddressAttribute:
                        object valueEmail = propertyInfo.GetValue(validateObject);
                        if (!IsValidEmail($"{valueEmail}"))
                        {
                            yield return emailAddressAttribute.ErrorMessage ?? $"{propertyInfo.Name} is not valid.";
                        }
                        bool IsValidEmail(string email)
                        {
                            string trimmedEmail = email.Trim();

                            if (trimmedEmail.EndsWith(".")) {
                                return false; // suggested by @TK-421
                            }
                            try {
                                MailAddress addr = new(email);
                                return addr.Address == trimmedEmail;
                            }
                            catch {
                                return false;
                            }
                        }
                        break;
                    case MinLengthAttribute minLengthAttribute:
                        object valueMinLength = propertyInfo.GetValue(validateObject);
                        if (($"{valueMinLength}").Length < minLengthAttribute.Length)
                        {
                            yield return minLengthAttribute.ErrorMessage ??
                                         $"{propertyInfo.Name} must have {minLengthAttribute.Length} letters.";
                        }
                        break;
                    case RequiredDigitAttribute requiredDigitAttribute:
                        object valueRequiredDigit = propertyInfo.GetValue(validateObject);
                        bool notHaveDigit = true;
                        foreach (char letter in $"{valueRequiredDigit}")
                        {
                            if(Char.IsNumber(letter))
                            {
                                notHaveDigit = false;
                                break;
                            }
                        }
                        if(notHaveDigit) yield return requiredDigitAttribute.ErrorMessage ?? $"{propertyInfo.Name} must have digit.";
                        break;
                }
            }
    }
}