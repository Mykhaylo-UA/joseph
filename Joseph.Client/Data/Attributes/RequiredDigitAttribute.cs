namespace ChristyAlsop.Wasm.Data.Attributes;

public class RequiredDigitAttribute: Attribute
{
    public string ErrorMessage { get; set; }

    public RequiredDigitAttribute()
    {
        
    }

    public RequiredDigitAttribute(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }
}