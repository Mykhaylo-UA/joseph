using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Web;
using AntDesign;
using Joseph.Client.Services;
using Joseph.Data.Entities;
using Joseph.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace Joseph.Client.Pages;

public partial class Index: ComponentBase
{
    [Inject] HttpClient HttpClient { get; set; }
    [Inject] IJSRuntime JsRuntime { get; set; }
    [Inject] MessageService MessageService { get; set; }
    [Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; }
    [Inject] IAuthenticationService AuthenticationService { get; set; }
    ClaimsPrincipal user;
    
    List<Job> _jobs;

    List<Answer> _myAnswers = new();
    Job _selectJob = new();
    bool _visible;
    string _answer = "";
    bool _confirmLoading;

    bool _visibleDrawer;
    List<Filter> _filters = new();
        
    protected override async Task OnInitializedAsync()
    {
        _jobs = await HttpClient.GetFromJsonAsync<List<Job>>("api/job");
        _filters = await HttpClient.GetFromJsonAsync<List<Filter>>("api/filter");
        
        string token = await JsRuntime.InvokeAsync<string>("getLocalStorageItem", "authToken");
        if (!string.IsNullOrEmpty(token))
        {
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            HttpResponseMessage responseMessage = await HttpClient.GetAsync("api/answer/getMyAnswers");
            if (responseMessage.IsSuccessStatusCode)
            {
                _myAnswers = await responseMessage.Content.ReadFromJsonAsync<List<Answer>>();   
            }
        }
        
        AuthenticationState authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        user = authState.User;
    }

    void SelectJob(Guid jobId)
    {
        _selectJob = _jobs.First(j => j.Id == jobId);
        _visible = true;
    }
    
    async void HandleOk()
    {
        _confirmLoading = true;
        StateHasChanged();
        Answer answer = new ()
        {
            Description = _answer,
            JobId = _selectJob.Id
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

    List<Filter> chooseFilter = new();
    List<Filter> chooseMinMaxFilter = new();
    
    void ChangeFilters(string[] arg, string filterName, string property, TypeFilter typeFilter)
    {
        Filter filter = chooseFilter.FirstOrDefault(f => f.Name == filterName);
        if (filter is not null) chooseFilter.Remove(filter);

        if (arg.Length > 0)
        {
            Filter newFilter = new()
            {
                Name = filterName, Values = new(), TypeFilter = typeFilter,
                Property = property
            };
            foreach (string s in arg)
            {
                Console.WriteLine(s);
                newFilter.Values.Add(s);
            }
            chooseFilter.Add(newFilter);
        }
    }

    void ChangeMinMaxFilter(int? value, string filterName, string minOrMax, string property)
    {
        if (value is null) return;
        var fil = chooseMinMaxFilter.FirstOrDefault(f => f.Name == filterName);
        if (fil is null)
        {
            fil = new Filter()
            {
                Additional = "Min,Max",
                Name = filterName,
                Property = property,
                TypeFilter = TypeFilter.MinMax,
                Values = new List<string>()
                {
                    "", ""
                }
            };
            chooseMinMaxFilter.Add(fil);
        }
        if (minOrMax == "Min")
        {
            fil.Values[0] = value.ToString();
        }
        else if (minOrMax == "Max")
        {
            fil.Values[1] = value.ToString();
        }
    }

    async void ApplyFilters()
    {
        string filterPath = CreateFilterRequest();
        _jobs = await HttpClient.GetFromJsonAsync<List<Job>>("api/job?"+filterPath);
        StateHasChanged();
    }

    string CreateFilterRequest()
    {
        string result = "";
        for (int a = 0; a < chooseMinMaxFilter.Count; a++)
        {
            result += $"filters[{a}].Name={chooseMinMaxFilter[a].Name}&" +
                      $"filters[{a}].TypeFilter={chooseMinMaxFilter[a].TypeFilter}&" +
                      $"filters[{a}].Property={chooseMinMaxFilter[a].Property}&";
            
            for (int i = 0; i < chooseMinMaxFilter[a].Values.Count; i++)
            {
                result += $"filters[{a}].Values[{i}]={chooseMinMaxFilter[a].Values[i]}&";
            }
        }
        
        for (int a = 0; a < chooseFilter.Count; a++)
        {
            result += $"filters[{a}].Name={chooseFilter[a].Name}&" +
                      $"filters[{a}].TypeFilter={chooseFilter[a].TypeFilter}&" +
                      $"filters[{a}].Property={chooseFilter[a].Property}&";
            
            for (int i = 0; i < chooseFilter[a].Values.Count; i++)
            {
                result += $"filters[{a}].Values[{i}]={chooseFilter[a].Values[i]}&";
            }
        }

        if (!string.IsNullOrEmpty(_searchText))
        {
            result+=$"search={HttpUtility.UrlEncode(_searchText)}&";
        }
        return result;
    }

    bool _searching;
    string _searchText;
    async Task OnSearch()
    {
        _searching = true;
        StateHasChanged();

        string filterPath = CreateFilterRequest();
        _jobs = await HttpClient.GetFromJsonAsync<List<Job>>("api/job?"+filterPath);
        _searching = false;
        StateHasChanged();
    }
}