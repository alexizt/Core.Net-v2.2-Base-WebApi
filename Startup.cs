using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;

namespace corewebapi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(options => options.UseInMemoryDatabase("MyDB"));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // Configuring Authentication middleware for JWT
            // ValidateAudience: le decimos que debe validar el audience que hemos definido.
            // ValidateLifeTime: como bien sabes nuestro token puede tener un tiempo de expiración, por lo tanto le diremos que valide esto.
            // ValidateIssuerSigningKey: que tome en cuenta la validación de nuestro secretkey.
            // ValidIssuer: seteamos nuestro valor que definimos en el appsettings.
            // ValidAudience: seteamos nuestro valor que definimos en el appsettings.
            // IssuerSigningKey: seteamos nuestro secret key para generar un TOKEN único que evita que sea usado por otra persona.
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwtBearerOptions =>
            {
                jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateActor = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["ApiAuth:Issuer"],
                    ValidAudience = Configuration["ApiAuth:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["ApiAuth:SecretKey"]))
                };
            });

            //Adding CORS to enable authentiaction from other domain
            services.AddCors(options =>
            {
                options.AddPolicy("EnableCORS", builder =>
                {
                builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowCredentials().Build();
                });
            });

            // Adding authorization Policies
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Administrator", policy => policy.RequireClaim("IsAdmin"));
            });

            // Adding swagger service
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Core WebAPI",
                    Description = "ASP.NET Core 2.2 Web API",
                    TermsOfService = "None",
                    Contact = new Contact()
                    {
                        Name = "Alexis Triemstra",
                        Email = "alexizt@yahoo.com.ar",
                        Url = "https://github.com/alexizt/"
                    }
                });
            });

            services.AddAuthorization(options =>
            {
                // If Claim "IsAdmin" is "IsAdmin" or "Si" or "Yes" then autohrize Administrator Policy
                options.AddPolicy("Administrator", policy =>
                                policy.RequireClaim("IsAdmin", "IsAdmin", "Si", "Yes" ));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // Injected DataContext
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, DataContext dataContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Seeding Datatabase
            DBSeeder.AddTestData(dataContext);

            // Adding JWT Authentication middleware
            app.UseAuthentication();

            // Adding CORS middleware
            app.UseCors("EnableCORS");

            app.UseHttpsRedirection();
            app.UseMvc();

            // Initializing Swagger Endpoint
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
        }


    }
}
