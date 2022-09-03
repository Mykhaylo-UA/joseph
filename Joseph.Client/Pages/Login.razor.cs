using AntDesign;
using Joseph.Client.Models;
using Joseph.Client.Services;
using Joseph.Client.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Joseph.Client.Pages;

public partial class Login: ComponentBase
{
    string Email { get; set; }
    string Password { get; set; }
    IEnumerable<string> ErrorsEmail { get; set; } = new List<string>();
    IEnumerable<string> ErrorsPassword { get; set; } = new List<string>();
    [Inject] IAuthenticationService AuthenticationService { get; set; }
    [Inject] NavigationManager NavigationManager { get; set; }
    [Inject] MessageService MessageService { get; set; }
    
    bool Loading { get; set; }
    
    async Task ExecuteLogin()
    {
        Loading = true;
        ResponseApi<AuthModelResponse> result = await AuthenticationService.Login(
            new LoginModelRequest() {Email = Email, Password = Password});
        
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
        LoginModelRequest authRequest = new() {Email = $"{args.Value}", Password = Password};
        ErrorsEmail = CustomValidator.Validate(authRequest)["Email"];
        StateHasChanged();
    }
    
    void ChangePassword(ChangeEventArgs args)
    {
        LoginModelRequest authRequest = new() {Email = Email, Password = $"{args.Value}"};
        ErrorsPassword = CustomValidator.Validate(authRequest)["Password"];
        StateHasChanged();
    }

    async void EnterKeyDown(KeyboardEventArgs arg)
    {
        if (arg.Key != "Enter") return;
        
        if (!($"{Email}".Trim() == "" || $"{Password}".Trim() == "" || ErrorsEmail.Count() != 0 ||
              ErrorsPassword.Count() != 0))
        {
            await ExecuteLogin();
        }
    }
}