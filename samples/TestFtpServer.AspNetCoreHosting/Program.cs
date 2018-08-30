// <copyright file="Program.cs" company="Fubar Development Junker">
// Copyright (c) Fubar Development Junker. All rights reserved.
// </copyright>

using System;

using FubarDev.FtpServer;
using FubarDev.FtpServer.FileSystem.DotNet;

using Google.Apis.Auth.OAuth2;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using NLog.Extensions.Logging;

namespace TestFtpServer
{
    internal class Program : FtpServerApp
    {
        private static int Main(string[] args)
        {
            return new Program().Run(args);
        }

        /// <inheritdoc />
        protected override void RunWithFileSystem(string rootDir, TestFtpServerOptions options)
        {
            var builder = CreateHostBuilder(options)
                .UseContentRoot(rootDir)
                .ConfigureServices(
                    services => services
                        .AddOptions<DotNetFileSystemOptions>()
                        .Configure<IHostingEnvironment>((opt, env) => opt.RootPath = env.ContentRootPath))
                .AddFtpServer(sb => Configure(sb, options).UseDotNetFileSystem());
            Run(builder);
        }

        /// <inheritdoc />
        protected override void RunWithGoogleDriveUser(UserCredential credential, TestFtpServerOptions options)
        {
            var builder = CreateHostBuilder(options)
                .AddFtpServer(sb => Configure(sb, options).UseGoogleDrive(credential));
            Run(builder);
        }

        /// <inheritdoc />
        protected override void RunWithGoogleDriveService(GoogleCredential credential, TestFtpServerOptions options)
        {
            var builder = CreateHostBuilder(options)
                .AddFtpServer(sb => Configure(sb, options).UseGoogleDrive(credential));
            Run(builder);
        }

        private static void Run(IHostBuilder hostBuilder)
        {
            try
            {
                hostBuilder.RunConsoleAsync().Wait();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
        }

        private static IHostBuilder CreateHostBuilder(TestFtpServerOptions options)
        {
            NLog.LogManager.LoadConfiguration("NLog.config");

            return new HostBuilder()
                .ConfigureLogging(
                    lb => lb
                        .SetMinimumLevel(LogLevel.Trace)
                        .AddNLog(
                            new NLogProviderOptions
                            {
                                CaptureMessageTemplates = true,
                                CaptureMessageProperties = true
                            }))
                .ConfigureServices(services => Configure(services, options));
        }
    }
}
