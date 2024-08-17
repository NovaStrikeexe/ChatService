using ChatService.Configuration.Models;
using ChatService.Data;
using ChatService.Data.Implementation;
using ChatService.Services.Message;
using ChatService.Services.Message.Implementation;
using ChatService.Services.WebSocket;
using ChatService.Services.WebSocket.Implementation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Npgsql;

namespace ChatService
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<DbSettings>(Configuration.GetSection("DatabaseSettings"));

            services.AddTransient(sp =>
            {
                var dbSettings = sp.GetRequiredService<IOptions<DbSettings>>().Value;
                if (dbSettings == null || string.IsNullOrEmpty(dbSettings.DefaultConnection))
                {
                    throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
                }

                return dbSettings.DefaultConnection;
            });

            services.AddControllers();

            services.AddTransient<IMessageService, MessageService>();
            services.AddScoped<IMessageRepository, MessageRepository>();

            services.AddSingleton(sp =>
            {
                var connectionString = sp.GetRequiredService<string>();
                var connection = new NpgsqlConnection(connectionString);
                connection.Open();
                return connection;
            });

            services.AddSingleton<IWebSocketService, WebSocketService>();

            services.AddWebSockets(options => 
            { 
                options.KeepAliveInterval = TimeSpan.FromSeconds(120); 
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
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

            app.UseCors("AllowAll");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.Map("/health", async context =>
                {
                    context.Response.StatusCode = 200;
                    await context.Response.WriteAsync("Healthy");
                });
                endpoints.Map("/ws", async context =>
                {
                    var webSocketManager = context.RequestServices.GetRequiredService<IWebSocketService>();
                    await webSocketManager.HandleWebSocketAsync(context);
                });
            });
        }
    }
}
