﻿using System;
using System.Text;
using JWT.Server.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using JWT.Server.Middlewares.Cache;

namespace JWT.Server
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
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = true;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Encoding.ASCII.GetBytes(Configuration.GetSection("JWTSettings:SecretKey").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });


            bool flag;
            bool.TryParse(Configuration.GetSection("SessionStore:Redis:enable").Value, out flag);
            if (flag)
            {
                
                services.AddStackExchangeRedisCache(options => { options.Configuration = Configuration.GetSection("SessionStore:Redis:server").Value; });

                /*services.AddDistributedRedisCache(options =>
                {
                    options.Configuration = Configuration.GetSection("SessionStore:Redis:server").Value;
                    //options.InstanceName = Configuration.GetSection("SessionStore:Redis:dbname").Value;
                });*/
            }
            try
            {

            }
            catch(Exception e)
            {
                Console.WriteLine(" Exception :" +e.Message);
            }
            services.AddSession(options => {

                options.IdleTimeout = TimeSpan.FromMinutes(60);
            });
            services.AddSingleton<IAuthManager>(
                new AuthManager(Configuration.GetSection("JWTSettings:SecretKey").Value));
            services.AddSingleton<MemCache>();
            services.AddControllers();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSession();
            app.UseHttpsRedirection();

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
