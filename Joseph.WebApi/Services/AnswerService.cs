using Joseph.Context;
using Joseph.Data.Entities;
using Joseph.WebApi.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Joseph.WebApi.Services;

public interface IAnswerService
{
    public Task<Answer> GetById(Guid id);
    public Task<List<Answer>> GetAllForJob(Guid jobId);
    public Task<List<Answer>> GetAll();
    public Task<List<Answer>> GetUserAnswers(string userId);
    public Task<Answer> Add(Answer answer);
    public Task<bool> Delete(Guid id);
}

public class AnswerService: IAnswerService
{
    readonly ApplicationContext _context;

    public AnswerService(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<Answer> GetById(Guid id)
    {
        Answer answer = await _context.Answers.Include(a=>a.User).FirstOrDefaultAsync(a => a.Id == id);
        if (answer is null) return null;
        
        answer.UserDisplay = UserDisplay.Create(answer.User);
        return answer;
    }

    public async Task<List<Answer>> GetAllForJob(Guid jobId)
    {
        var answers = await _context.Answers.Include(a=>a.User).Where(a => a.JobId == jobId).ToListAsync();
        foreach (Answer answer in answers)
        {
            answer.UserDisplay = UserDisplay.Create(answer.User);
        }

        return answers;
    }

    public  async Task<List<Answer>> GetAll()
    {
        var answers = await _context.Answers.Include(a=>a.User).ToListAsync();
        foreach (Answer answer in answers)
        {
            answer.UserDisplay = UserDisplay.Create(answer.User);
        }

        return answers;
    }

    public async Task<List<Answer>> GetUserAnswers(string userId)
    {
        var answers = await _context.Answers.Include(a=>a.User).Where(a => a.UserId == userId).ToListAsync();
        foreach (Answer answer in answers)
        {
            answer.UserDisplay = UserDisplay.Create(answer.User);
        }

        return answers;
    }

    public async Task<Answer> Add(Answer answer)
    {
        if (answer is null) throw new Exception("Answer can't be null");

        EntityEntry<Answer> entity = await _context.Answers.AddAsync(answer);
        await _context.SaveChangesAsync();

        entity.Entity.UserDisplay = UserDisplay.Create(answer.User);

        return entity.Entity;
    }

    public async Task<bool> Delete(Guid id)
    {
        if (id == Guid.Empty) return false;

        Answer answer = await GetById(id);
        if (answer is null) return false;
        
        _context.Answers.Remove(answer);
        await _context.SaveChangesAsync();
        return true;
    }
}