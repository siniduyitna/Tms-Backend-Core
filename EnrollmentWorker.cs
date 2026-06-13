using Microsoft.Extensions.DependencyInjection;

public class EnrollmentWorker
{
    private readonly IServiceScopeFactory _scopeFactory;

    // 🚀 IServiceScopeFactory መርጫችንን እዚህ ጋር እንወጋለን (Inject እናደርጋለን)
    public EnrollmentWorker(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public void ProcessBatch()
    {
        // 🎯 አጭር ዕድሜ ያለው scope መፍጠር - 'using' በራሱ በሂደቱ ማብቂያ ያጠፋዋል
        using var scope = _scopeFactory.CreateScope();
        
        // 🎯 የ Scoped አገልግሎቱን ከዚሁ scope መፍለቅ
        var enrollmentService = scope.ServiceProvider.GetRequiredService<IEnrollmentService>();

        // የሙከራ ሥራ ማስኬጃ
        _ = enrollmentService.GetAllAsync();
    }
}