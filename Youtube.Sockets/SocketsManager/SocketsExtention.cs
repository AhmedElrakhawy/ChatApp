using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace Youtube.Sockets.SocketsManager
{
    public static class SocketsExtention
    {
        public static IServiceCollection AddWebSocketsManager(this IServiceCollection services)
        {
            services.AddTransient<ConnectionManager>();
            foreach (var Type in Assembly.GetEntryAssembly().ExportedTypes)
            {
                if (Type.GetTypeInfo().BaseType == typeof(SocketHandler))
                    services.AddSingleton(Type);
            }

            return services;
        }
        public static IApplicationBuilder MapSockets(this IApplicationBuilder app , PathString path , SocketHandler socket)
        {
            return app.Map(path, (x) => x.UseMiddleware<SocketMiddleware>(socket));
        }
    }
}
