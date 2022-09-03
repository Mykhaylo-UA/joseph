using Joseph.Context;
using Joseph.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Joseph.WebApi.Services;

public interface IJobTypeService
{
    public Task<JobType> GetById(Guid id);
    public Task<List<JobType>> GetAll();
    public Task<JobType> Add(JobType jobType);
    public Task<JobType> Update(JobType jobType);
    public Task<bool> Delete(Guid id);
}

public class JobTypeService: IJobTypeService
{
    readonly ApplicationContext _context;

    public JobTypeService(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<JobType> GetById(Guid id) => await _context.JobTypes.FirstOrDefaultAsync(a => a.Id == id);
    
    public async Task<List<JobType>> GetAll() => await _context.JobTypes.ToListAsync();

    public async Task<JobType> Add(JobType jobType)
    {
        if (jobType is null) return null;
        EntityEntry<JobType> entity = await _context.JobTypes.AddAsync(jobType);
        await _context.SaveChangesAsync();
        return entity.Entity;
    }

    public Task<JobType> Update(JobType jobType)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Delete(Guid id)
    {
        throw new NotImplementedException();
    }
}