using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RealTimeChat.BLL;
using RealTimeChat.BLL.ChatDataservice;
using RealTimeChat.BLL.Repository;
using RealTimeChat.DAL;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Load configuration from appsettings.json
var jwtSettings = builder.Configuration.GetSection("JWT");
builder.Services.AddAuthentication(opt => {
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "*",
            ValidAudience = "*",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]))
        };
    });


builder.Services.AddCors(options =>
{
    options.AddPolicy("reactApp",
        builder =>
        {
            builder
            .AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();

        });
});
builder.Services.AddSignalR();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//Add support to logging with SERILOG
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));


builder.Services.AddDbContext<ChatDbContext>
    (o => o.UseSqlite(builder.Configuration.GetConnectionString("SqliteConnection")));

//Adding DI
builder.Services.AddScoped<IUsermasterCore, UsermasterCore>();
builder.Services.AddScoped<IChathistoryCore, ChathistoryCore>();

builder.Services.AddSingleton<SharedDb>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//Add support to logging request with SERILOG
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/Chat");
app.UseCors(x => x
        .AllowAnyMethod()
        .AllowAnyHeader()
        .SetIsOriginAllowed(origin => true)
        .AllowCredentials());
app.Run();
