using CapstoneProject.Models;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddScoped<UserAccessLayer>();
builder.Services.AddSingleton<CharacterAccessLayer>();
builder.Services.AddScoped<PostAccessLayer>();
builder.Services.AddScoped<CommunityDataAccessLayer>();
builder.Services.AddScoped<MessagesAccessLayer>();
builder.Services.AddScoped<UserAccessLayer>();
builder.Services.AddSignalR();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.MapRazorPages();
app.MapHub<CapstoneProject.NewFolder.ChatHub>("/chatHub");


app.Run();