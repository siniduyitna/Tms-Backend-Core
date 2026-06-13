using Microsoft.AspNetCore.Authentication;
var builder = WebApplication.CreateBuilder(args);
// 1. Services Registeration
builder.Services.AddAuthentication("Training")
    .AddScheme<AuthenticationSchemeOptions, TrainingAuthHandler>("Training", null);
builder.Services.AddAuthorization();

var app = builder.Build();

// 2. Middleware Pipeline (ቅደም ተከተል ወሳኝ ነው!)
app.UseExceptionHandler(); // ለወደፊት Errors ለመያዝ
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication(); // መጀመሪያ ተጠቃሚውን ለይ
app.UseAuthorization();  // ቀጥሎ የደረሱበትን መብት አረጋግጥ

// 3. Endpoint Map
app.MapGet("/api/assessments/results", () => Results.Ok(new
{
    courseCode = "CS-101",
    studentId = "S-001",
    letterGrade = "A"
}))
.RequireAuthorization(); // ይህንን መስመር መርሳት የለብንም!

app.Run();