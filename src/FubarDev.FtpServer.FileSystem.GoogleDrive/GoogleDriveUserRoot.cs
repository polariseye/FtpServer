// <copyright file="GoogleDriveUserRoot.cs" company="Fubar Development Junker">
// Copyright (c) Fubar Development Junker. All rights reserved.
// </copyright>

using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;

namespace FubarDev.FtpServer.FileSystem.GoogleDrive
{
    /// <summary>
    /// Returns information about the user root entry.
    /// </summary>
    public class GoogleDriveUserRoot
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleDriveUserRoot"/> class.
        /// </summary>
        /// <param name="service">The Google Drive service.</param>
        /// <param name="rootEntry">The root entry.</param>
        public GoogleDriveUserRoot(DriveService service, File rootEntry)
        {
            Service = service;
            RootEntry = rootEntry;
        }

        /// <summary>
        /// Gets the Google Drive service.
        /// </summary>
        public DriveService Service { get; }

        /// <summary>
        /// Gets the root entry.
        /// </summary>
        public File RootEntry { get; }

        /// <summary>
        /// Support deconstruction of the type.
        /// </summary>
        /// <param name="service">The Google Drive service.</param>
        /// <param name="rootEntry">The root entry.</param>
        public void Deconstruct(out DriveService service, out File rootEntry)
        {
            service = Service;
            rootEntry = RootEntry;
        }
    }
}
