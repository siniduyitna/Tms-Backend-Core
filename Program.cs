// 🚀 1. ሁሉም USING መስመሮች ሁልጊዜም እዚህ አናት ላይ መሆን አለባቸው!
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics; 

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------
// 2. SERVICES REGISTRATION (የአልክተሮኒክስ አገልግሎቶች ምዝገባ)
// ---------------------------------------------------------------------

// የ DI lifetimes እና Container Host Validation ማረጋገጫ
builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateScopes = true;     
    options.ValidateOnBuild = true;    
});

// የሴሽን 1 የሙከራ Auth
builder.Services.AddAuthentication("Training")
    .AddScheme<AuthenticationSchemeOptions, TrainingAuthHandler>("Training", null);
builder.Services.AddAuthorization();

// የሴሽን 2 አገልግሎቶች ምзыገባ
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddSingleton<EnrollmentWorker>();

// Options Pattern ከ Startup Validation ጋር
builder.Services.AddOptions<PaymentOptions>()
    .BindConfiguration("Payments")
    .ValidateDataAnnotations()
    .ValidateOnStart(); 

var app = builder.Build();

// ---------------------------------------------------------------------
// 3. MIDDLEWARE PIPELINE (የነገሮች ቅደም ተከተል)
// ---------------------------------------------------------------------

// የሴሽን 1 Custom Logging - ሁልጊዜም መጀመሪያ
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseExceptionHandler("/error");
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// ---------------------------------------------------------------------
// 4. ENDPOINTS (የመገናኛ መስመሮች)
// ---------------------------------------------------------------------

// የሴሽን 1 የፈተና መስመር
app.MapGet("/api/assessments/results", () => Results.Ok(new
{
    courseCode = "CS-101", 
    studentId = "S-001",
    letterGrade = "A"
})).RequireAuthorization();

// የጀርባ ሠራተኛውን መፈተኛ መስመር
app.MapGet("/api/enrollments/worker-smoke", (EnrollmentWorker worker) =>
{
    worker.ProcessBatch();
    return Results.Ok("processed");
});

app.Run();