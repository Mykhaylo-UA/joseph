using System.Net.Http.Headers;
using System.Net.Http.Json;
using AntDesign;
using Joseph.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Joseph.Client.Pages;

[Authorize(Roles = "Candidate")]
public partial class Candidate: ComponentBase
{
    [Inject] IJSRuntime JsRuntime { get; set; }
    [Inject] HttpClient HttpClient { get; set; }
    [Inject] MessageService MessageService { get; set; }
    
    User user = null;
    bool _loading;

    protected override async Task OnInitializedAsync()
    {
        var token = await JsRuntime.InvokeAsync<string>("getLocalStorageItem", "authToken");
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        user = await HttpClient.GetFromJsonAsync<User>("api/user");
    }

    async Task UpdateUser()
    {
        _loading = true;
        StateHasChanged();
        var token = await JsRuntime.InvokeAsync<string>("getLocalStorageItem", "authToken");
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        HttpResponseMessage responseMessage = await HttpClient.PostAsJsonAsync("api/user", user);
        if (responseMessage.IsSuccessStatusCode)
        {
            MessageService.Success("User is updated");
        }
        else
        {
            MessageService.Error("User update error");
        }
        _loading = false;
    }
}