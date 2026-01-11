/*using Google.GenAI.Types;
using Loujico.BL;
using Loujico.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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
            builder.Services.AddScoped<ITasks, ClsTasks>();
           // builder.Services.AddScoped<ICustomers, ClsCustomer>();
            builder.Services.AddScoped<ICompanys, ClsCompany>();
            builder.Services.AddScoped<Isettings, ClsSettings>();
          //  builder.Services.AddScoped<IInvoices, ClsInvoices>();
            builder.Services.AddScoped<IFiles, ClsFiles>();
            builder.Services.AddScoped<IAuthorizationHandler, EditTaskStatusHandler>();
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("EditTaskStatus", policy =>
                    policy.Requirements.Add(new EditTaskStatusRequirement()));
            });


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
            // ﬁ»· builder.Build()
            // ›Ì ConfigureServices
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy", policy =>
                {
                    policy.WithOrigins(
                        "http://localhost:3000",
                        "https://localhost:3000",
                        "http://yousefallouji.com",
                        "https://yousefallouji.com",
                        "http://www.yousefallouji.com",
                        "https://www.yousefallouji.com"
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
                });
            });

            // ≈–« ﬂ«‰  API ›ﬁÿ »œÊ‰ Ã·”« 
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });





            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 10;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<CompanySystemContext>().AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>("Default"); ;
            // Add services to the container.
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(7176);
            });
            builder.WebHost.UseUrls("http://0.0.0.0:7176");
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

                // ≈÷«›… Security Definition
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "«ﬂ » 'Bearer' „ »Ê⁄… »«·‹ Token. „À«·: Bearer eyJhbGciOiJIUzI1NiIs..."
                });

                // ≈÷«›… Security Requirement
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
            });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddMemoryCache();
            builder.Services.AddScoped<IGeminiService, GeminiService>();
            // ⁄‰œ ≈‰‘«¡ «·ÿ·»
     
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

          //    app.UseHttpsRedirection();
            app.UseErrorHandlingMiddleware();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors("MyPolicy");


            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
*/
using Google.GenAI.Types;
using Loujico.BL;
using Loujico.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

namespace Loujico
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // 1. √Ê·«: ≈‰‘«¡ builder „⁄ ≈⁄œ«œ«  Œ«’… ·„‰⁄ HTTPS
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                Args = args,
                ContentRootPath = Directory.GetCurrentDirectory(),
                WebRootPath = "wwwroot"
            });

            // 2. Â–« ÂÊ «·”ÿ— «·√Â„: ≈Ã»«— HTTP ›ﬁÿ
            builder.WebHost.ConfigureKestrel(options =>
            {
                // HTTP ›ﬁÿ »œÊ‰ √Ì ≈‘«—… ·‹ HTTPS
                options.ListenAnyIP(7176);
            });

            // 3. «· √ﬂÌœ ⁄·Ï HTTP ›ﬁÿ
            builder.WebHost.UseUrls("http://*:7176");

            // 4. ≈÷«›… services
            builder.Services.AddScoped<IEmployees, ClsEmployees>();
            builder.Services.AddScoped<IProject, ClsProject>();
            builder.Services.AddScoped<Ilog, ClsLogs>();
            builder.Services.AddScoped<IProducts, ClsProducts>();
            builder.Services.AddScoped<IHistory, ClsHistory>();
            builder.Services.AddScoped<ITasks, ClsTasks>();
            builder.Services.AddScoped<ICompanys, ClsCompany>();
            builder.Services.AddScoped<Isettings, ClsSettings>();
            builder.Services.AddScoped<IFiles, ClsFiles>();
            builder.Services.AddScoped<IAuthorizationHandler, EditTaskStatusHandler>();

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("EditTaskStatus", policy =>
                    policy.Requirements.Add(new EditTaskStatusRequirement()));
            });

            // 5. Database
            builder.Services.AddDbContext<CompanySystemContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")),
                ServiceLifetime.Scoped);

            // 6. Authentication „⁄  ⁄ÿÌ· HTTPS requirement
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
           .AddJwtBearer(options =>
           {
               // Â–« «·”ÿ— „Â„ Ãœ«:  ⁄ÿÌ· HTTPS requirement
               options.RequireHttpsMetadata = false;

               options.TokenValidationParameters = new TokenValidationParameters
               {
                   RoleClaimType = ClaimTypes.Role,
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidateLifetime = true,
                   ValidateIssuerSigningKey = true,
                   ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                   ValidAudience = builder.Configuration["JwtSettings:Audience"],
                   IssuerSigningKey = new SymmetricSecurityKey(
                       Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
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

            // 7. CORS „»”ÿ
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            // 8. Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 10;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<CompanySystemContext>()
            .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>("Default");

            // 9. Controllers
            builder.Services.AddControllers();

            // 10. Swagger («Œ Ì«—Ì)
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "«ﬂ » 'Bearer' „ »Ê⁄… »«·‹ Token"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            // 11. Œœ„«  √Œ—Ï
            builder.Services.AddMemoryCache();
            builder.Services.AddScoped<IGeminiService, GeminiService>();

            // 12. »‰«¡ «· ÿ»Ìﬁ
            var app = builder.Build();

            // 13. Configure pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // 14. ·«  ” Œœ„ UseHttpsRedirection √»œ«
            // app.UseHttpsRedirection();

            app.UseErrorHandlingMiddleware();
            app.UseStaticFiles();
            app.UseRouting();

            // 15. «” Œœ„ CORS policy «·„»”ÿ
            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            // 16. Run
            app.Run();
        }
    }
}