using System.Reflection;
using ChatService.Configuration;
using ChatService.Data.DataConnect;
using ChatService.Data.DataConnect.Implementation;
using ChatService.Data.MessageRepository;
using ChatService.Data.MessageRepository.Implementation;
using ChatService.Services;
using ChatService.Services.Message;
using ChatService.Services.Message.Implementation;
using ChatService.SignalR.Hubs;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace ChatService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<DbSettings>(Configuration.GetSection("DatabaseSettings"));

            // Добавляем конфигурацию подключения к базе данных
            services.AddTransient(sp =>
            {
                var dbSettings = sp.GetRequiredService<IOptions<DbSettings>>().Value;
                if (string.IsNullOrEmpty(dbSettings.DefaultConnection))
                {
                    throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
                }

                return dbSettings.DefaultConnection;
            });

            services.AddSignalR();
            services.AddControllers();
            services.AddTransient<IMessageService, MessageService>();
            services.AddScoped<IMessageRepository, MessageRepository>();

            services.AddSingleton<IDataConnect>(sp =>
            {
                var connectionString = sp.GetRequiredService<string>();
                return new DataConnect(connectionString);
            });
            
            services.AddWebSockets(options =>
            {
                options.KeepAliveInterval = TimeSpan.FromSeconds(120);
            });

            services.Configure<CorsSettings>(Configuration.GetSection("Cors"));
            services.AddCors(options =>
            {
                var corsSettings = Configuration.GetSection("Cors").Get<CorsSettings>();
                options.AddPolicy("AllowSpecificOrigin", builder =>
                {
                    builder.WithOrigins(corsSettings.AllowedOrigin)
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            services.AddSwaggerGen(c =>
            {
                var swaggerSettings = Configuration.GetSection("Swagger").Get<SwaggerSettings>();
                c.SwaggerDoc(swaggerSettings.Version, new OpenApiInfo
                {
                    Title = swaggerSettings.Title,
                    Version = swaggerSettings.Version
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                var swaggerSettings = Configuration.GetSection("Swagger").Get<SwaggerSettings>();
                c.SwaggerEndpoint(swaggerSettings.Endpoint, $"{swaggerSettings.Title} {swaggerSettings.Version}");
                c.RoutePrefix = "swagger";
            });

            app.UseRouting();

            app.UseWebSockets();
            app.UseCors("AllowSpecificOrigin");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.Map("/health", async context =>
                {
                    context.Response.StatusCode = 200;
                    await context.Response.WriteAsync("Healthy");
                });
                
                endpoints.MapHub<ChatHub>("/chathub");
            });
        }
    }
}
