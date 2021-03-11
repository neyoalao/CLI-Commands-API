using System;
using System.Linq;
using CommandsAPI.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Newtonsoft.Json.Serialization;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using CommandsAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using CommandAPI.Token;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CommandsAPI
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public const string PolicyName = "AllowedOrigins";
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {

            // services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //                 .AddJwtBearer(options =>
            //                 {
            //                     options.Audience = Configuration["AAD:ResourceId"];
            //                     options.Authority = $"{Configuration["AAD:Instance"]}{Configuration["AAD:TenantId"]}";
            //                 });
            // services.AddAuthentication(AzureADDefaults.BearerAuthenticationScheme).AddAzureADBearer(options => Configuration.Bind("AzureActiveDirectory", options));
            services.AddScoped<IJWTTokenGenerator, JWTTokenGenerator>();
            services.AddDbContext<CommandsContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("CommandsConnection")));

            services.AddIdentity<IdentityUser, IdentityRole>(opt =>
            {
                opt.Password.RequireDigit = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequiredLength = 4;

                opt.User.RequireUniqueEmail = true;
            }

            ).AddEntityFrameworkStores<CommandsContext>();

            services.AddControllers().AddNewtonsoftJson(s =>
            {
                s.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddScoped<ICommandsAPIRepo, SqlCommandsAPIRepo>();

            services.AddAuthentication(
                //setup the job token schema
                config =>
            {
                config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }
            ).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Token:Key"])),
                    ValidIssuer = Configuration["Token:Issuer"],
                    ValidateIssuer = true,
                    ValidateAudience = false,
                };
            });

            var corsOrigins = Configuration.GetSection(PolicyName)
                    .GetChildren()
                    .Select(x => x.Value)
                    .ToArray();

            services.AddCors(options =>
            {
                options.AddPolicy(
                            PolicyName,
                            builder =>
                            {
                                builder
                                .WithOrigins(corsOrigins)
                                .AllowAnyMethod()
                                .AllowAnyHeader();
                            });
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "ToDo API",
                    Description = "ASP.NET Core web api for useful coding commands",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Olaniyi Alao",
                        Email = string.Empty,
                        Url = new Uri("https://github.com/neyoalao"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under MIT Open Liecense",
                        Url = new Uri("https://example.com/license"),
                    }
                });
            });
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // it has to be called here before other functions
            app.UseCors(PolicyName);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //creates migrations on the database
            // PrepDB.CreateMigration(app);

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                // To serve the Swagger UI at the app's root (http://localhost:<port>/)
                c.RoutePrefix = string.Empty;
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
