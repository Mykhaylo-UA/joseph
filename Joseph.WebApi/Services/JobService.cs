using Joseph.Context;
using Joseph.Data.Entities;
using Joseph.WebApi.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Joseph.WebApi.Services;

public interface IJobService
{
    public Task<Job> GetById(Guid id);
    public Task<List<Job>> GetAll();
    public Task<Job> Add(Job job);
    public Task<Job> Update(Job job);
    public Task<bool> Remove(Guid id);
}

public class JobService:IJobService
{
    readonly ApplicationContext _context;

    public JobService(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<Job> GetById(Guid id)
    {
        var job = await _context.Jobs
            .Include(j=>j.Answers)
                .ThenInclude(j=>j.User)
            .Include(j=>j.JobTypes)
            .AsNoTracking()
            .FirstOrDefaultAsync(j=>j.Id == id);
        if (job is null) return null;
        
        foreach (Answer jobAnswer in job.Answers)
        {
            jobAnswer.UserDisplay = UserDisplay.Create(jobAnswer.User);
        }

        return job;
    }

    public async Task<List<Job>> GetAll()
    {
        var jobs =  await _context.Jobs
            .Include(j=>j.Answers)
                .ThenInclude(a=>a.User)
            .Include(j=>j.JobTypes)
            .AsNoTracking()
            .ToListAsync();

        foreach (Job job in jobs)
        {
            foreach (Answer jobAnswer in job.Answers)
            {
                jobAnswer.UserDisplay = UserDisplay.Create(jobAnswer.User);
            }
        }

        return jobs;
    }

    public async Task<Job> Add(Job job)
    {
        if (job is null) return null;
        
        job.Id = Guid.NewGuid();
        job.JobTypes ??= new();
        for (int i = 0; i < job.JobTypes.Count; i++)
        {
            job.JobTypes[i] = await _context.JobTypes.FindAsync(job.JobTypes[i].Id);
        }
        
        EntityEntry<Job> entity = await _context.Jobs.AddAsync(job);
        await _context.SaveChangesAsync();
        
        return entity.Entity;
    }
    
    public async Task<Job> Update(Job job)
    {
        if (job is null) return null;

        Job oldJob = await GetById(job.Id);

        await Remove(oldJob.Id);
        
        for (int i = 0; i < job.JobTypes.Count; i++)
        {
            job.JobTypes[i] = await _context.JobTypes.FindAsync(job.JobTypes[i].Id);
        }
        
        EntityEntry<Job> entity = await _context.Jobs.AddAsync(job);
        await _context.SaveChangesAsync();

        return entity.Entity;
    }

    public async Task<bool> Remove(Guid id)
    {
        Job job = await GetById(id);
        if (job is null) return false;

        _context.Jobs.Remove(job);
        await _context.SaveChangesAsync();
        return true;
    }
}