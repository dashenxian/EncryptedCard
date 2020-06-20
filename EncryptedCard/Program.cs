using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace NetCoreConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                })
                .ConfigureAppConfiguration(configApp =>
                {
                    configApp.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    //添加数据访问组件示例：services.AddTransient<IDbAccesser>(provider =>
                    //{
                    //    string connStr = context.Configuration.GetConnectionString("ConnDbStr");
                    //    return new SqlDapperEasyUtil(connStr);
                    //});

                    //添加HttpClient封装类示例：services.AddHttpClient<GitHubApiClient>()
                    //.AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(3, t => TimeSpan.FromMilliseconds(800)));

                    services.AddHostedService<TaskHostedService>();

                })
                .ConfigureLogging((context, configLogging) =>
                {
                    configLogging.ClearProviders();
                    configLogging.SetMinimumLevel(LogLevel.Trace);
                    configLogging.AddNLog();
                    //需要Serilog.Extensions.Hosting包
                    //configLogging.AddSerilog(dispose: true);
                })
                .UseConsoleLifetime()
                .Build();

            await host.RunAsync();
        }
    }
}