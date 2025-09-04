using Loujico.BL;
using Loujico.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Loujico
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddScoped<IEmployees,ClsEmployees>();
            builder.Services.AddScoped<IProject, ClsProject>();
            builder.Services.AddScoped<Ilog, ClsLogs>();
            builder.Services.AddScoped<IProducts, ClsProducts>();
            builder.Services.AddScoped<IHistory, ClsHistory>();
            builder.Services.AddScoped<ICustomers, ClsCustomers>();
            builder.Services.AddScoped<IInvoices, ClsInvoices>();
            builder.Services.AddDbContext<CompanySystemContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")),
        ServiceLifetime.Scoped);
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
           .AddJwtBearer(options =>
           {
               var config = builder.Configuration.GetSection("JwtSettings");
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   RoleClaimType = ClaimTypes.Role,
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidateLifetime = true,
                   ValidateIssuerSigningKey = true,
                   ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                   ValidAudience = builder.Configuration["JwtSettings:Audience"],
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
               };
               options.Events = new JwtBearerEvents
               {
                   OnAuthenticationFailed = context =>
                   {
                       Console.WriteLine($"Authentication failed: {context.Exception}");
                       return Task.CompletedTask;
                   }
               };
           });
           builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 10;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<CompanySystemContext>();
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddMemoryCache();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();


            app.UseStaticFiles();

            app.UseRouting();
      //      app.UseCors("AllowFrontend");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
