using AntDesign;
using Joseph.Client.Models;
using Joseph.Client.Utils;
using Joseph.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Joseph.Client.Pages;

public partial class Register: ComponentBase
{
    string Email { get; set; }
    string Password { get; set; }
    string ConfirmPassword { get; set; }
    string Role { get; set; } = "Candidate";
    List<string> ErrorsEmail { get; set; } = new ();
    List<string> ErrorsPassword { get; set; } = new ();
    [Inject]
    IAuthenticationService AuthenticationService { get; set; }
    [Inject]
    NavigationManager NavigationManager { get; set; }
    [Inject] MessageService MessageService { get; set; }

    bool Loading { get; set; }
    async Task ExecuteRegister()
    {
        RegisterModelRequest authRequest = new() { Email = Email, Password = Password, Role=Role };

        Loading = true;
        ResponseApi<AuthModelResponse> result = await AuthenticationService.Register(authRequest);
        Loading = false;
        StateHasChanged();
        if (result.IsSuccess)
        {
            NavigationManager.NavigateTo("/");
            MessageService.Success("Welcome to Joseph!");
        }
        else
        {
            Console.WriteLine(result.Message);
            MessageService.Error(result.Message);
        }
    }
    
    void ChangeEmail(ChangeEventArgs args)
    {
        RegisterModelRequest authRequest = new() {Email = $"{args.Value}", Password = Password};
        ErrorsEmail = CustomValidator.Validate(authRequest)["Email"].ToList();
        StateHasChanged();
    }
    
    void ChangePassword(ChangeEventArgs args)
    {
        RegisterModelRequest authRequest = new() {Email = Email, Password = $"{args.Value}"};
        ErrorsPassword = CustomValidator.Validate(authRequest)["Password"].ToList();
        StateHasChanged();
    }
    
    async void EnterKeyDown(KeyboardEventArgs arg)
    {
        if (arg.Key != "Enter") return;
        
        if (!($"{Email}".Trim() == "" || $"{Password}".Trim() == "" || ErrorsEmail.Count() != 0 ||
              ErrorsPassword.Count() != 0))
        {
            await ExecuteRegister();
        }
    }
}