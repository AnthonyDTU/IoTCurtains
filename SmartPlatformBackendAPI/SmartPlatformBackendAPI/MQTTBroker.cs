using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MQTTnet.AspNetCore;
using MQTTnet.Server;
using SmartPlatformBackendAPI.Data;

namespace SmartPlatformBackendAPI
{
    public class MQTTBroker
    {
        public static Task Start_Server_With_WebSockets_Support()
        {
            /*
             * This sample starts a minimal ASP.NET Webserver including a hosted MQTT server.
             */
            var host = Host.CreateDefaultBuilder(Array.Empty<string>())
                //.ConfigureServices(
                //    services =>
                //    {
                //        services.AddControllers();
                //        services.AddEndpointsApiExplorer();
                //        services.AddSwaggerGen();
                //        services.AddDbContext<SmartPlatformAPIDbContext>(options => options.UseSqlServer(host.Configuration.GetConnectionString("DefaultConnection")));
                //    })
                .ConfigureWebHostDefaults(
                    webBuilder =>
                    {
                        webBuilder.UseKestrel(
                            o =>
                            {
                                // This will allow MQTT connections based on TCP port 1883.
                                o.ListenAnyIP(1883, l => l.UseMqtt());

                                // This will allow MQTT connections based on HTTP WebSockets with URI "localhost:5000/mqtt"
                                // See code below for URI configuration.
                                o.ListenAnyIP(5000); // Default HTTP pipeline
                            });

                        webBuilder.UseStartup<Startup>();
                    });

            Console.WriteLine("MQTT Broker Started");
            return host.RunConsoleAsync();
        }

        sealed class MqttController
        {
            public MqttController()
            {
                // Inject other services via constructor.
            }

            public Task OnClientConnected(ClientConnectedEventArgs eventArgs)
            {
                Console.WriteLine($"Client '{eventArgs.ClientId}' connected.");
                return Task.CompletedTask;
            }


            public Task ValidateConnection(ValidatingConnectionEventArgs eventArgs)
            {
                Console.WriteLine($"Client '{eventArgs.ClientId}' wants to connect. Accepting!");
                return Task.CompletedTask;
            }

            public Task InterceptionPublishAsync(InterceptingPublishEventArgs eventArgs)
            {
                Console.WriteLine(eventArgs.ApplicationMessage);
                return Task.CompletedTask;
            }
        }

        sealed class Startup
        {
            public void Configure(IApplicationBuilder app, IWebHostEnvironment environment, MqttController mqttController)
            {
                app.UseRouting();

                app.UseEndpoints(
                    endpoints =>
                    {
                        endpoints.MapConnectionHandler<MqttConnectionHandler>(
                            "/mqtt",
                            httpConnectionDispatcherOptions => httpConnectionDispatcherOptions.WebSockets.SubProtocolSelector =
                                protocolList => protocolList.FirstOrDefault() ?? string.Empty);
                    });

                app.UseMqttServer(
                    server =>
                    {
                        /*
                         * Attach event handlers etc. if required.
                         */

                        server.ValidatingConnectionAsync += mqttController.ValidateConnection;
                        server.ClientConnectedAsync += mqttController.OnClientConnected;
                        server.InterceptingPublishAsync += mqttController.InterceptionPublishAsync;
                    });

            }

            public void ConfigureServices(IServiceCollection services)
            {

                //services.AddControllers();
                //services.AddEndpointsApiExplorer();
                //services.AddSwaggerGen();
                //services.AddDbContext<SmartPlatformAPIDbContext>(options => options.UseSqlServer(host.Configuration.GetConnectionString("DefaultConnection")));

                services.AddHostedMqttServer(
                    optionsBuilder =>
                    {
                        optionsBuilder.WithDefaultEndpoint();
                    });

                services.AddMqttConnectionHandler();
                services.AddConnections();

                services.AddSingleton<MqttController>();
            }
        }
    }
}
