using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace TmsApi; // 👈 ወደ TmsApi ቀይረነዋል

public class EnrollmentWorker
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<EnrollmentWorker> _logger;

    public EnrollmentWorker(IServiceScopeFactory scopeFactory, ILogger<EnrollmentWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public void ProcessBatch()
    {
        _logger.LogInformation("Enrollment background batch processing started at {Time}", DateTime.UtcNow);

        using var scope = _scopeFactory.CreateScope();
        var enrollmentService = scope.ServiceProvider.GetRequiredService<IEnrollmentService>();

        try
        {
            var enrollments = enrollmentService.GetAllAsync().GetAwaiter().GetResult();
            _logger.LogInformation("Successfully processed {Count} enrollment records in this batch.", enrollments.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing enrollment batch.");
        }
    }
}