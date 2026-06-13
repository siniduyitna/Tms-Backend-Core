using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

// --- The data shape --- 
public record EnrollmentRecord(
    string Id, 
    string StudentId, 
    string CourseCode, 
    DateTime EnrolledAt);

// --- The contract --- 
public interface IEnrollmentService
{
    Task<EnrollmentRecord> EnrollAsync(string studentId, string courseCode);
    Task<EnrollmentRecord?> GetByIdAsync(string id);
    Task<IReadOnlyList<EnrollmentRecord>> GetAllAsync();
    Task<bool> DeleteAsync(string id);
}

// --- The in-memory implementation with Structured Logging --- 
public class EnrollmentService : IEnrollmentService
{
    // ባለብዙ-ክሮች (Thread-safe) እንዲሆን ConcurrentDictionary እንጠቀም
    private readonly ConcurrentDictionary<string, EnrollmentRecord> _store = new();
    private readonly ILogger<EnrollmentService> _logger;

    public EnrollmentService(ILogger<EnrollmentService> logger)
    { 
        _logger = logger;
    }

    public Task<EnrollmentRecord> EnrollAsync(string studentId, string courseCode)
    {
        // 🎯 Exercise 4: Duplicate Check with [Warning] Log Level
        var existing = _store.Values
            .FirstOrDefault(e => e.StudentId == studentId && e.CourseCode == courseCode);

        if (existing is not null)
        { 
            _logger.LogWarning( 
                "Duplicate enrollment attempt {StudentId} already in {CourseCode} (record {EnrollmentId})", 
                studentId, courseCode, existing.Id);
            return Task.FromResult(existing);
        }

        var id = Guid.NewGuid().ToString("N")[..8];
        var record = new EnrollmentRecord(id, studentId, courseCode, DateTime.UtcNow); 
        _store[id] = record; 

        // 🎯 [Information] Log Level - Structured Pattern
        _logger.LogInformation( 
            "Enrolled {StudentId} in {CourseCode} record {EnrollmentId}", 
            studentId, courseCode, id);

        return Task.FromResult(record);
    }

    public Task<EnrollmentRecord?> GetByIdAsync(string id)
    { 
        _store.TryGetValue(id, out var record);
        if (record is null)
        { 
            // 🎯 Exercise 4: [Warning] for missing records
            _logger.LogWarning("Enrollment {EnrollmentId} not found", id);
        }
        return Task.FromResult(record);
    }

    public Task<IReadOnlyList<EnrollmentRecord>> GetAllAsync()
    {
        IReadOnlyList<EnrollmentRecord> all = _store.Values.ToList();
        return Task.FromResult(all);
    }

    public Task<bool> DeleteAsync(string id)
    {
        var removed = _store.TryRemove(id, out _);
        if (removed) 
            _logger.LogInformation("Deleted enrollment {EnrollmentId}", id);
        else
            _logger.LogWarning("Delete failed enrollment {EnrollmentId} not found", id);

        return Task.FromResult(removed);
    }
}