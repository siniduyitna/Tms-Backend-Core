using Microsoft.AspNetCore.Authentication;
using TmsApi;
using TmsApi.Options;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------
// 1. SERVICES REGISTRATION (የአገልግሎቶች ምዝገባ ክፍል)
// ---------------------------------------------------------
builder.Services.AddControllers();

// 🔐 የሙከራ ማረጋገጫ አገልግሎትን መመዝገብ
builder.Services.AddAuthentication("Training")
    .AddScheme<AuthenticationSchemeOptions, TrainingAuthHandler>("Training", null);

builder.Services.AddAuthorization();

// 💣 [Exercise 2 & 4] የ TMS ሰርቪስ እና ወርከር ምዝገባ (Lifetimes)
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>(); 
builder.Services.AddSingleton<EnrollmentWorker>();                   

// 🛡️ [Exercise 2] የዕድሜ መቆላለፍ መከላከያ (The Captive Dependency Detector)
builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateScopes = true;   
    options.ValidateOnBuild = true;  
});

// 🛑 [Exercise 3] የኮንፊገሬሽን ማረጋገጫ በጅማሮ ላይ (Options Pattern Validation)
builder.Services.AddOptions<PaymentOptions>()
    .BindConfiguration("Payments")   
    .ValidateDataAnnotations()       
    .ValidateOnStart();              


var app = builder.Build();

// ---------------------------------------------------------
// 2. MIDDLEWARE PIPELINE (የጥያቄ ፍሰት መስመር)
// ---------------------------------------------------------
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseExceptionHandler("/error"); 
app.UseRouting();
app.UseAuthentication(); 
app.UseAuthorization();  

// ---------------------------------------------------------
// 3. ENDPOINT MAPPING (የኤፒአይ አድራሻዎች)
// ---------------------------------------------------------
app.MapGet("/api/enrollments/worker-smoke", (EnrollmentWorker worker) =>
{
    worker.ProcessBatch();
    return Results.Ok("processed");
});

app.MapGet("/api/assessments/results", () => Results.Ok(new
{
    courseCode = "CS-101", 
    studentId = "S-001",
    letterGrade = "A"
})).RequireAuthorization(); 

app.Run();