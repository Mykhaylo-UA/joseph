using Joseph.Context;
using Joseph.Data.Entities;
using Joseph.Enums;
using Microsoft.EntityFrameworkCore;

namespace Joseph.WebApi.Services;

public interface IFilterService
{
    public Task<List<Filter>> GetFilters();
}
public class FilterService: IFilterService
{
    readonly ApplicationContext _context;

    public FilterService(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<List<Filter>> GetFilters()
    {
        List<Filter> filters = new();
        List<Job> jobs = await _context.Jobs.Include(j =>j.JobTypes).ToListAsync();
        if (jobs.Count == 0)
        {
            return filters;
        }
        
        // Location filter

        Filter locationFilter = new() {Name = "Location", 
            Values = new(), TypeFilter = TypeFilter.CheckBox, Property = "Location"};
        IEnumerable<IGrouping<string, Job>> groupJob = jobs.GroupBy(j => j.Location);
        foreach (IGrouping<string,Job> grouping in groupJob)
        {
            locationFilter.Values.Add(grouping.Key);
        }
        if(locationFilter.Values.Count > 1) filters.Add(locationFilter);

        
        // Job Types Filter
        Filter jobTypesFilter = new()
        {
            Name = "Job Types", Values = new(), TypeFilter = TypeFilter.CheckBox,
            Property = "JobTypes"
        };
        int jobTypesCount = (await _context.JobTypes.ToListAsync()).Count;
        foreach (Job job in jobs)
        {
            foreach (JobType jobType in job.JobTypes)  
            {
                if (!jobTypesFilter.Values.Contains(jobType.Name))
                {
                    jobTypesFilter.Values.Add(jobType.Name);
                }

                if (jobTypesFilter.Values.Count >= jobTypesCount)
                {
                    break;
                }
            }
        }
        if(jobTypesFilter.Values.Count > 1 && jobs.Count > 1) filters.Add(jobTypesFilter);
        
        //Hours a week filter
        int minHoursAWeek = jobs.Min(j => j.HoursWeek);
        int maxHoursAWeek = jobs.Max(j => j.HoursWeek);
        Filter hoursAWeekFilter = new() {Name = "Hours A Week", Values = new(), TypeFilter = TypeFilter.MinMax, 
            Additional = "Min,Max", 
            Property = "HoursWeek"};
        hoursAWeekFilter.Values.Add(minHoursAWeek.ToString());
        hoursAWeekFilter.Values.Add(maxHoursAWeek.ToString());
        if(minHoursAWeek != maxHoursAWeek) filters.Add(hoursAWeekFilter);
        
        // Number Of Hired People
        Filter numberOfHiredPeopleFilter = new()
        {
            Name = "Number Of Hired People", Values = new(), TypeFilter = TypeFilter.Enums,
            Property = "NumberOfHiredPeople"
        };
        IEnumerable<IGrouping<NumberOfHiredPeople, Job>> groupJobByNumberOfHired =
            jobs.GroupBy(j => j.NumberOfHiredPeople);
        
        foreach (IGrouping<NumberOfHiredPeople,Job> grouping in groupJobByNumberOfHired)
        {
            numberOfHiredPeopleFilter.Values.Add(grouping.Key.ToString());
        }
        if(numberOfHiredPeopleFilter.Values.Count > 1) filters.Add(numberOfHiredPeopleFilter);
        
        
        return filters;
    }
}