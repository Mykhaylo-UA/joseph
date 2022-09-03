using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using AntDesign;
using Joseph.Data.Entities;
using Joseph.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace Joseph.Client.Pages;

[Authorize(Roles = "Employer")]
public partial class Employer: ComponentBase
{
    [Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; }
    [Inject] HttpClient HttpClient { get; set; }
    [Inject] IJSRuntime JsRuntime { get; set; }
    

    Job _newJob = new();
    bool _isEdit;
    List<JobType> _jobTypes = new();
    List<Job> _jobs = new();

    bool _loadingDrawer;
    bool _loadingDelete;
    bool _visible;

    Job _selectJob = new ();
    
    protected override async Task OnInitializedAsync()
    {
        var token = await JsRuntime.InvokeAsync<string>("getLocalStorageItem", "authToken");
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        _jobTypes = await HttpClient.GetFromJsonAsync<List<JobType>>("api/jobType");
        _jobs = await HttpClient.GetFromJsonAsync<List<Job>>("api/job/getMyJobs");
    }
    
    void HandleItemsChange(IEnumerable<string> value)
    {
        _newJob.JobTypes = new List<JobType>();
        foreach (string s in value)
        {
            _newJob.JobTypes.Add(_jobTypes.First(j => j.Id == Guid.Parse(s)));
        }
    }
    
    void HandleItemsHiredChange(NumberOfHiredPeople value)
    {
        _newJob.NumberOfHiredPeople = value;
    }

    async Task AddVacancies()
    {
        _loadingDrawer = true;
        var token = await JsRuntime.InvokeAsync<string>("getLocalStorageItem", "authToken");
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        HttpResponseMessage responseMessage = await HttpClient.PostAsJsonAsync("api/job", _newJob);
        _jobs = await responseMessage.Content.ReadFromJsonAsync<List<Job>>();
        _newJob = new Job();
        _loadingDrawer = false;
        close();
    }

    bool visible;

    void open()
    {
        visible = true;
    }

    void close()
    {
        visible = false;
        _newJob = new Job(){JobTypes = new(), NumberOfHiredPeople = NumberOfHiredPeople.OneThree, HoursWeek = 10};
    }

    void EditJob(Guid id)
    {
        _newJob = JsonSerializer.Deserialize<Job>(JsonSerializer.Serialize(_jobs.First(j => j.Id == id)));
        _isEdit = true;
        open();
    }

    async Task EditVacancies()
    {
        _loadingDrawer = true;
        var token = await JsRuntime.InvokeAsync<string>("getLocalStorageItem", "authToken");
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        HttpResponseMessage responseMessage = await HttpClient.PutAsJsonAsync("api/job", _newJob);
        _jobs = await responseMessage.Content.ReadFromJsonAsync<List<Job>>();
        _newJob = new Job();
        _loadingDrawer = false;
        close();
    }

    async Task DeleteVacancy(Guid id)
    {
        _loadingDelete = true;
        StateHasChanged();
        var token = await JsRuntime.InvokeAsync<string>("getLocalStorageItem", "authToken");
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        HttpResponseMessage responseMessage = await HttpClient.DeleteAsync($"api/job?id={id}");
        if (responseMessage.IsSuccessStatusCode)
        {
            _jobs.Remove(_jobs.Find(j=> j.Id == id));
        }

        _loadingDelete = false;
    }

    void SelectJobAnswers(Guid id)
    {
        _selectJob = _jobs.First(j => j.Id == id);
        _visible = true;
    }

    BreakpointType _breakpointType;
    void BreakpointHandler(BreakpointType obj)
    {
        _breakpointType = obj;
    }
}