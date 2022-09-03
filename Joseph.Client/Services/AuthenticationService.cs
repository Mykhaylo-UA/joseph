using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ChristyAlsop.Wasm.Models;
using Joseph.Client.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace Joseph.Client.Services;

public interface IAuthenticationService
{
    Task<ResponseApi<AuthModelResponse>> Register(RegisterModelRequest registerModel);
    Task<ResponseApi<AuthModelResponse>> Login(LoginModelRequest loginModel);
    Task Logout();
}

public class AuthenticationService: IAuthenticationService
{
    readonly HttpClient _client;
    readonly JsonSerializerOptions _options;
    readonly AuthenticationStateProvider _authStateProvider; 
    readonly IJSRuntime _jsRuntime;

    public AuthenticationService(
        HttpClient client,
        AuthenticationStateProvider authStateProvider, 
        IJSRuntime jsRuntime)
    {
        _client = client;
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        _authStateProvider = authStateProvider;
        _jsRuntime = jsRuntime;
    }
    
    public async Task<ResponseApi<AuthModelResponse>> Login(LoginModelRequest loginModel)
    {
        var content = JsonSerializer.Serialize(loginModel);
        var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
        var authResult = await _client.PostAsync("api/Authenticate/login", bodyContent);
        var authContent = await authResult.Content.ReadAsStringAsync();

        if (authResult.IsSuccessStatusCode)
        {
            AuthModelResponse result = JsonSerializer.Deserialize<AuthModelResponse>(authContent, _options);
            await _jsRuntime.InvokeAsync<string>("setLocalStorageItem", "authToken", result.Token);
            ((AuthStateProvider)_authStateProvider).NotifyUserAuthentication(loginModel.Email);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", result.Token);
            return new ResponseApi<AuthModelResponse>()
            {
                Entity = result,
                IsSuccess = true,
            };
        }

        if (authResult.StatusCode == HttpStatusCode.Unauthorized)
        {
            return new ResponseApi<AuthModelResponse>()
            {
                IsSuccess = false,
                Message = "Login or password is incorrect"
            };
        }

        return new ResponseApi<AuthModelResponse>()
        {
            IsSuccess = false,
            Message = "Login failed"
        };
    }
    public async Task<ResponseApi<AuthModelResponse>> Register(RegisterModelRequest registerModel)
    {
        var content = JsonSerializer.Serialize(registerModel);
        var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
        var authResult = await _client.PostAsync("api/Authenticate/register", bodyContent);
        var authContent = await authResult.Content.ReadAsStringAsync();
        
        if (authResult.IsSuccessStatusCode)
        {
            AuthModelResponse result = JsonSerializer.Deserialize<AuthModelResponse>(authContent, _options);
            await _jsRuntime.InvokeAsync<string>("setLocalStorageItem", "authToken", result.Token);
            ((AuthStateProvider)_authStateProvider).NotifyUserAuthentication(registerModel.Email);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", result.Token);
            return new ResponseApi<AuthModelResponse>()
            {
                Entity = result,
                IsSuccess = true,
            };
        }

        return new ResponseApi<AuthModelResponse>()
        {
            IsSuccess = false,
            Message = authContent
        };
    }
    public async Task Logout()
    {
        await _jsRuntime.InvokeAsync<string>("removeLocalStorageItem", "authToken");
        ((AuthStateProvider)_authStateProvider).NotifyUserLogout();
        _client.DefaultRequestHeaders.Authorization = null;
    }
}