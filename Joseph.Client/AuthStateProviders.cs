using System.Net.Http.Headers;
using System.Security.Claims;
using Joseph.Client.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace Joseph.Client;

public class AuthStateProvider : AuthenticationStateProvider
{
    readonly HttpClient _httpClient;
    readonly AuthenticationState _anonymous;
    readonly IJSRuntime  _jsRuntime;
    readonly NavigationManager _navigationManager;
    
    public AuthStateProvider(HttpClient httpClient, IJSRuntime  jsRuntime, NavigationManager navigationManager)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
        _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        _navigationManager = navigationManager;
    }
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _jsRuntime.InvokeAsync<string>("getLocalStorageItem", "authToken");
        if (string.IsNullOrWhiteSpace(token))
            return _anonymous;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        return new AuthenticationState(new ClaimsPrincipal(
            new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwtAuthType")));
    }
    public void NotifyUserAuthentication(string email)
    {
        var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, email) }, "jwtAuthType"));
        var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
        NotifyAuthenticationStateChanged(authState);
    }
    public void NotifyUserLogout()
    {
        var authState = Task.FromResult(_anonymous);
        NotifyAuthenticationStateChanged(authState);
        _navigationManager.NavigateTo("/");
    }
}