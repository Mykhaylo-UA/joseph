using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Joseph.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;

namespace Joseph.Client.Pages;

public partial class Account:ComponentBase
{
    [Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; }
    [Inject] IAuthenticationService AuthenticationService { get; set; }
    ClaimsPrincipal user;
    protected override async Task OnInitializedAsync()
    {
        AuthenticationState authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        user = authState.User;
    }
}