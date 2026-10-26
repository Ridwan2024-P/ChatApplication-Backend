using ChatWave.Business.Interfaces.Repositories;
using ChatWave.Business.Interfaces.Services;
using ChatWave.Business.Services;
using ChatWave.Data.Data;
using ChatWave.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using ChatWave.Business.Interfaces.Services;
using ChatWave.Data.Services;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ChatWaveDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOtpRepository, OtpRepository>();
builder.Services.AddScoped<IConversationRepository, ConversationRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddHttpClient<ISmsSender, MoceanSmsSender>(
    client =>
    {
        client.BaseAddress =
            new Uri("https://rest.moceanapi.com/");

        client.Timeout = TimeSpan.FromSeconds(20);
    });
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IConversationService, ConversationService>();
builder.Services.AddScoped<IMessageService, MessageService>();
var cs = builder.Configuration
    .GetConnectionString("DefaultConnection");
var dbConnection =
    new Npgsql.NpgsqlConnectionStringBuilder(cs);

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
