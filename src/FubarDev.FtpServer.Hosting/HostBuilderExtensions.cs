// <copyright file="HostBuilderExtensions.cs" company="Fubar Development Junker">
// Copyright (c) Fubar Development Junker. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FubarDev.FtpServer;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Hosting
{
    /// <summary>
    /// Extension methods for <see cref="IHostBuilder"/>.
    /// </summary>
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// Adds the FTP server to the host builder.
        /// </summary>
        /// <param name="builder">The host builder to add the FTP host service to.</param>
        /// <param name="configure">Configuration of the FTP server services.</param>
        /// <returns>The host builder.</returns>
        public static IHostBuilder AddFtpServer(
            this IHostBuilder builder,
            [NotNull] Action<IFtpServerBuilder> configure)
        {
            return builder.ConfigureServices(
                (_, services) =>
                {
                    services.AddFtpServer(configure)
                        .AddSingleton<IHostedService, FtpServerHostingWrapper>();
                });
        }

        private class FtpServerHostingWrapper : IHostedService
        {
            private readonly IEnumerable<IFtpService> _ftpServices;

            public FtpServerHostingWrapper(IEnumerable<IFtpService> ftpServices)
            {
                _ftpServices = ftpServices;
            }

            /// <inheritdoc />
            public async Task StartAsync(CancellationToken cancellationToken)
            {
                foreach (var ftpService in _ftpServices)
                {
                    await ftpService.StartAsync(cancellationToken);
                }
            }

            /// <inheritdoc />
            public async Task StopAsync(CancellationToken cancellationToken)
            {
                foreach (var ftpService in _ftpServices.Reverse())
                {
                    await ftpService.StopAsync(cancellationToken);
                }
            }
        }
    }
}
