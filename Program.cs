using Microsoft.AspNetCore.Authentication;
using Scalar.AspNetCore;
using TmsApi;
using TmsApi.Options;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------
// 1. SERVICES REGISTRATION (የአገልግሎቶች ምዝገባ ክፍል)
// ---------------------------------------------------------
builder.Services.AddControllers(); 
builder.Services.AddOpenApi(); // 👈 አሁን ፓኬጁ ስለተጫነ ኤረሩ ይጠፋል!

builder.Services.AddAuthentication("Training")
    .AddScheme<AuthenticationSchemeOptions, TrainingAuthHandler>("Training", null);

builder.Services.AddAuthorization();

// 💣 [ስላይድ 4 & 5] IEnrollmentService Scoped ሲሆን EnrollmentWorker Singleton ነው
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>(); 
builder.Services.AddSingleton<EnrollmentWorker>();                   

// 🛡️ [ስላይድ 1] የዕድሜ መቆላለፍ መከላከያ (The Captive Dependency Detector)
builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateScopes = true;   // 👈 Singleton ውስጥ Scoped እንዳይገባ በጅማሮ ይይዛል
    options.ValidateOnBuild = true;  // 👈 አፑ Build ሲደረግ ወዲያውኑ ይፈትሻል
});

builder.Services.AddOptions<PaymentOptions>()
    .BindConfiguration("Payments")   
    .ValidateDataAnnotations()       
    .ValidateOnStart();              

// 🚨 [Exercise 6] የ ProblemDetails አገልግሎት ምዝገባ
builder.Services.AddProblemDetails(); 

var app = builder.Build();

// ---------------------------------------------------------
// 2. MIDDLEWARE PIPELINE (የጥያቄ ፍሰት መስመር)
// ---------------------------------------------------------
app.UseMiddleware<RequestLoggingMiddleware>();

// 🎯 [Exercise 7] የአካባቢ መቀያየሪያ ሕግ (Dev vs Prod Toggle)
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();               // 👈 በዴቨሎፕመንት ብቻ የኢንተርፌስ ሰነዱን ክፈት
    app.MapScalarApiReference();    // 👈 በዴቨሎፕመንት ብቻ የ Scalar UI ክፈት
    app.UseExceptionHandler();      // 👈 በዴቨሎፕመንትም ProblemDetails ተጠቀም
}
else
{
    app.UseExceptionHandler();      // 👈 በፕሮዳክሽን ስታክ ትሬስን ደብቅ
}

app.UseStatusCodePages();          // 👈 ባዶ 404 ስታተሶችን ወደ JSON ይቀይራል
app.UseRouting();
app.UseAuthentication(); 
app.UseAuthorization();  

// ---------------------------------------------------------
// 3. ENDPOINT MAPPING (የኤፒአይ አድራሻዎች)
// ---------------------------------------------------------

//  (ProblemDetails ፍተሻ)
app.MapGet("/api/error", () =>
{
    throw new TmsDataException("Simulated database failure for ProblemDetails testing");
});

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

app.MapControllers(); // 👈 ኮንትሮለሮቹን ማገናኛ

app.Run();