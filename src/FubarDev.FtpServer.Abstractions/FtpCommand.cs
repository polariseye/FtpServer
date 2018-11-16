//-----------------------------------------------------------------------
// <copyright file="FtpCommand.cs" company="Fubar Development Junker">
//     Copyright (c) Fubar Development Junker. All rights reserved.
// </copyright>
// <author>Mark Junker</author>
//-----------------------------------------------------------------------

using System;

using JetBrains.Annotations;

namespace FubarDev.FtpServer
{
    /// <summary>
    /// FTP command with argument.
    /// </summary>
    public sealed class FtpCommand
    {
        private static readonly char[] _whiteSpaces = { ' ', '\t' };

        /// <summary>
        /// Initializes a new instance of the <see cref="FtpCommand"/> class.
        /// </summary>
        /// <param name="connection">连接对象</param>
        /// <param name="commandName">The command name.</param>
        /// <param name="commandArgument">The command argument.</param>
        public FtpCommand(IFtpConnection connection, [NotNull] string commandName, [CanBeNull] string commandArgument)
        {
            Name = commandName;
            Argument = commandArgument ?? string.Empty;
            this.Connection = connection;
        }

        /// <summary>
        /// Gets the command name.
        /// </summary>
        [NotNull]
        public string Name { get; }

        /// <summary>
        /// Gets the command argument.
        /// </summary>
        [NotNull]
        public string Argument { get; }

        /// <summary>
        /// 当前的连接对象
        /// </summary>
        public IFtpConnection Connection { get; private set; }

        /// <summary>
        /// Splits the <paramref name="command"/> into the name and its arguments.
        /// </summary>
        /// <param name="command">The command to split into name and arguments.</param>
        /// <returns>The created <see cref="FtpCommand"/>.</returns>
        [NotNull]
        public static FtpCommand Parse(IFtpConnection connection, [NotNull] string command)
        {
            var spaceIndex = command.IndexOfAny(_whiteSpaces);
            var commandName = spaceIndex == -1 ? command : command.Substring(0, spaceIndex);
            var commandArguments = spaceIndex == -1 ? string.Empty : command.Substring(spaceIndex + 1);
            return new FtpCommand(connection, commandName, commandArguments);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var message =
                Name.StartsWith("PASS", StringComparison.OrdinalIgnoreCase)
                    ? "PASS **************** (password omitted)"
                    : $"{Name} {Argument}";
            return message;
        }
    }
}
