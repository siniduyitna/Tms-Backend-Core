using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------
// 1. SERVICES REGISTRATION (የአገልግሎቶች ምዝገባ ክፍል)
// ---------------------------------------------------------
builder.Services.AddControllers();

// የሙከራ ማረጋገጫ አገልግሎትን ከእኛ 'TrainingAuthHandler' ጋር መመዝገብ
builder.Services.AddAuthentication("Training")
    .AddScheme<AuthenticationSchemeOptions, TrainingAuthHandler>("Training", null);

builder.Services.AddAuthorization();

var app = builder.Build();

// ---------------------------------------------------------
// 2. MIDDLEWARE PIPELINE (የጥያቄ ፍሰት መስመር - ቅደም ተከተሉ ወሳኝ ነው!)
// ---------------------------------------------------------

// 🎯 ከሁሉም በፊት የእኛን መከታተያ እንሰካለን (የውጪው መጠቅለያ - Stopwatch እና ID የሚጀምርበት)
app.UseMiddleware<RequestLoggingMiddleware>();

// በማዕቀፉ ውስጥ አደጋ ወይም ክራሽ ቢኖር የሚቀልብልን መከላከያ
app.UseExceptionHandler("/error"); 

// አድራሻ መሪ (Routing)
app.UseRouting();

// የደህንነት በሮች (ቅድመ-ሁኔታዎች)
app.UseAuthentication(); // ማንነትን መለየት (X-Training-User ሄደር መኖሩን ማየት)
app.UseAuthorization();  // ፈቃድ መኖሩን ማረጋገጥ

// ---------------------------------------------------------
// 3. ENDPOINT MAPPING (የኤፒአይ አድራሻዎች)
// ---------------------------------------------------------
app.MapGet("/api/assessments/results", () => Results.Ok(new
{
    courseCode = "CS-101", 
    studentId = "S-001",
    letterGrade = "A"
})).RequireAuthorization(); // ይህ አድራሻ ያለ መታወቂያ እንዳይከፈት በጥብቅ ይከለክላል!

// 🛑 የሰርቨሩ ማጠቃለያ እና ማሳረጊያ መስመር (The Finish Line)
app.Run();