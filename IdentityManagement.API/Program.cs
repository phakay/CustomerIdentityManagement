using IdentityManagement.API.Config;
using IdentityManagement.API.Core.Infrastructure;
using IdentityManagement.API.Core.Repositories;
using IdentityManagement.API.Core.Security;
using IdentityManagement.API.Core.Services;
using IdentityManagement.API.Extensions;
using IdentityManagement.API.Infrastructure;
using IdentityManagement.API.Persistence;
using IdentityManagement.API.Security;
using IdentityManagement.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt => opt.AddSecuritySupport());
//builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite("Data Source=identity-management-db.sqlite"));
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

builder.Services.AddMemoryCache();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddSingleton<IPasswordService, PasswordService>();
builder.Services.AddSingleton<ISecurityProvider, SecurityProvider>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ILgaRepository, LgaRepository>();
builder.Services.AddScoped<IStateRepository, StateRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ISmsService, SmsService>();
builder.Services.AddSingleton<ITokenSigningProvider, SymmetricSigningTokenProvider>(sp =>
{
    var secret = sp.GetRequiredService<IOptions<TokenConfigOptions>>().Value.Secret;
    return new SymmetricSigningTokenProvider(secret);
});
builder.Services.Configure<TokenConfigOptions>(builder.Configuration.GetSection("TokenConfigOptions"));
builder.Services.ConfigureOptions<JwtBearerOptionsSetup>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

builder.Services.AddHttpClient<BankService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


using (var scope = app.Services.CreateScope())
{
    var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    appDbContext.Database.Migrate();

}

app.Run();
