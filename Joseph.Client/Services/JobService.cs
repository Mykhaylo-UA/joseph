using System.Net.Http.Json;
using Joseph.Client.Models;
using Joseph.Data.Entities;

namespace Joseph.Client.Services;

public interface IJobService
{
    public Task<List<Job>> GetAll();
}
public class JobService : IJobService
{
    readonly HttpClient _httpClient;

    public JobService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Job>> GetAll()
    {
        List<Job> jobs = await _httpClient.GetFromJsonAsync<List<Job>>("api/job");

        return jobs;
    }
}