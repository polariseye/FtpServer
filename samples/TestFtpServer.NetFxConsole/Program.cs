using System;
using System.Threading;
using System.Threading.Tasks;

using FubarDev.FtpServer;
using FubarDev.FtpServer.FileSystem.DotNet;

using Google.Apis.Auth.OAuth2;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Serilog;

namespace TestFtpServer
{
    internal class Program : FtpServerApp
    {
        public static int Main(string[] args)
        {
            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            return new Program().Run(args);
        }

        /// <inheritdoc />
        protected override void RunWithFileSystem(string rootDir, TestFtpServerOptions options)
        {
            var services = CreateServices(options)
                .Configure<DotNetFileSystemOptions>(opt => opt.RootPath = rootDir)
                .AddFtpServer(sb => Configure(sb, options).UseDotNetFileSystem());
            Run(services).Wait();
        }

        /// <inheritdoc />
        protected override void RunWithGoogleDriveUser(UserCredential credential, TestFtpServerOptions options)
        {
            var services = CreateServices(options)
                .AddFtpServer(sb => Configure(sb, options).UseGoogleDrive(credential));
            Run(services).Wait();
        }

        /// <inheritdoc />
        protected override void RunWithGoogleDriveService(GoogleCredential credential, TestFtpServerOptions options)
        {
            var services = CreateServices(options)
                .AddFtpServer(sb => Configure(sb, options).UseGoogleDrive(credential));
            Run(services).Wait();
        }

        private static async Task Run(IServiceCollection services)
        {
            using (var serviceProvider = services.BuildServiceProvider())
            {
                var ftpServer = serviceProvider.GetRequiredService<FtpServer>();

                try
                {
                    // Start the FTP server
                    await ftpServer.StartAsync(CancellationToken.None)
                        .ConfigureAwait(false);

                    Console.WriteLine("Press ENTER/RETURN to close the test application.");
                    Console.ReadLine();

                    // Stop the FTP server
                    await ftpServer.StopAsync(CancellationToken.None)
                        .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
            }
        }

        private static IServiceCollection CreateServices(TestFtpServerOptions options)
        {
            var services = new ServiceCollection();

            // Add Serilog as logger provider
            services.AddLogging(cfg => cfg.AddSerilog().SetMinimumLevel(LogLevel.Trace));

            Configure(services, options);
            return services;
        }
    }
}
