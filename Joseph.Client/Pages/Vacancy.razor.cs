using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using AntDesign;
using Joseph.Client.Services;
using Joseph.Data.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace Joseph.Client.Pages;

public partial class Vacancy: ComponentBase
{
    [Parameter] public string Id { get; set; }
    [Inject] IJSRuntime JsRuntime { get; set; }
    [Inject] HttpClient HttpClient { get; set; }
    [Inject] MessageService MessageService { get; set; }
    [Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; }
    [Inject] IAuthenticationService AuthenticationService { get; set; }
    ClaimsPrincipal user;
    
    Job job = new();
    List<Answer> _myAnswers = new ();
    bool _visible;
    string _answer = "";
    bool _confirmLoading;

    protected override async Task OnInitializedAsync()
    {
        if (Guid.TryParse(Id, out Guid guid))
        {
            var token = await JsRuntime.InvokeAsync<string>("getLocalStorageItem", "authToken");
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            HttpResponseMessage responseMessage = await HttpClient.GetAsync($"api/job/{guid}");
            job = await responseMessage.Content.ReadFromJsonAsync<Job>();

            if (!string.IsNullOrEmpty(token))
            {
                HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
                HttpResponseMessage responseMessageAnswers = await HttpClient.GetAsync("api/answer/getMyAnswers");
                _myAnswers = await responseMessageAnswers.Content.ReadFromJsonAsync<List<Answer>>();   
            }

        }
    }
    
    async void HandleOk()
    {
        _confirmLoading = true;
        StateHasChanged();
        Answer answer = new ()
        {
            Description = _answer,
            JobId = job.Id
        };
        
        string token = await JsRuntime.InvokeAsync<string>("getLocalStorageItem", "authToken");
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        HttpResponseMessage responseMessage = await HttpClient.PostAsJsonAsync("api/answer", answer);
        if (responseMessage.IsSuccessStatusCode)
        {
            MessageService.Success("Proposal is submitted");
        }
        else
        {
            MessageService.Error("Error");
        }
        _confirmLoading = false;
        _visible = false;
        StateHasChanged();
    }

    void HandleCancel()
    {
        _answer = "";
        _visible = false;
    }
}