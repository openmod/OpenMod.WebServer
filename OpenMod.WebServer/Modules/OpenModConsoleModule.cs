using System;
using System.Linq;
using System.Threading.Tasks;
using EmbedIO.WebSockets;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenMod.API.Commands;
using OpenMod.API.Permissions;
using OpenMod.API.Users;
using OpenMod.Core.Helpers;
using OpenMod.WebServer.Authentication;
using OpenMod.WebServer.Dtos;
using OpenMod.WebServer.Logging;

namespace OpenMod.WebServer.Modules
{
    public class OpenModConsoleModule : WebSocketModule
    {
        private readonly ILogger<OpenModConsoleModule> _logger;
        private readonly IAuthenticationService _authenticationService;
        private readonly IPermissionChecker _permissionChecker;
        private readonly ICommandExecutor _commandExecutor;

        private const byte PacketAuthenticate = 0;
        private const byte PacketMessage = 1;

        public OpenModConsoleModule(string urlPath,
            ILogger<OpenModConsoleModule> logger,
            IAuthenticationService authenticationService,
            IPermissionChecker permissionChecker,
            ICommandExecutor commandExecutor) : base(urlPath, true)
        {
            _logger = logger;
            _authenticationService = authenticationService;
            _permissionChecker = permissionChecker;
            _commandExecutor = commandExecutor;
            LoggerProxy.Logged += OnLog;
        }

        public const string PermissionAccessConsole = "webconsole";
        public const string PermissionAccessLogs = "webconsole.logs";

        private void OnLog(LogLevel loglevel, EventId eventid, object? state, Exception? exception,
            Func<object, Exception?, string>? formatter)
        {
            if (state == null)
            {
                return;
            }

            Task.Run(() =>
            {
                var message = new LogMessageDto
                {
                    Message = (formatter != null ? formatter(state, exception) : state.ToString()),
                    LogLevel = loglevel
                };

                if (exception != null)
                {
                    message.Exception = new ExceptionDto
                    {
                        Type = exception.GetType().FullName,
                        Message = exception.Message,
                        StackTrace = exception.StackTrace
                    };
                }

                var buffer = SerializeMessage(PacketMessage, message);
                return BroadcastAsync(buffer, ctx =>
                    ctx.Items.ContainsKey("user")
                    && (bool)ctx.Items["canReadLogs"]);
            });
        }

        private byte[] SerializeMessage(byte id, object message)
        {
            var json = JsonConvert.SerializeObject(message);
            var bytes = System.Text.Encoding.UTF8.GetByteCount(json);
            var buffer = new byte[bytes + 1];
            buffer[0] = id;
            System.Text.Encoding.UTF8.GetBytes(json, charIndex: 0, json.Length, buffer, byteIndex: 1);
            _logger.LogDebug($"Serialized {buffer.Length} bytes:");
            _logger.LogDebug(message.ToString());
            return buffer;
        }

        protected override async Task OnMessageReceivedAsync(IWebSocketContext context, byte[] buffer,
            IWebSocketReceiveResult result)
        {
            _logger.LogDebug($"Received {buffer.Length} bytes.");
            _logger.LogDebug(System.Text.Encoding.UTF8.GetString(buffer));
            _logger.LogDebug(string.Join(" ", buffer.Select(d => d.ToString("X"))));

            if (buffer.Length < 1)
            {
                return;
            }

            var type = buffer[0];
            switch (type)
            {
                case PacketAuthenticate:
                    {
                        if (context.Items.ContainsKey("user"))
                        {
                            return;
                        }

                        var token = System.Text.Encoding.UTF8.GetString(buffer, 1, buffer.Length - 1);
                        if (string.IsNullOrEmpty(token))
                        {
                            _logger.LogDebug($"Closing socket.Invalid token: {token}.");
                            await CloseAsync(context);
                            return;
                        }

                        var user = await _authenticationService.AuthenticateAsync(token);
                        if (user == null)
                        {
                            _logger.LogDebug("Closing socket. User could not authenticate.");
                            await CloseAsync(context);
                            return;
                        }

                        if (await _permissionChecker.CheckPermissionAsync(user, PermissionAccessConsole) !=
                            PermissionGrantResult.Grant)
                        {
                            _logger.LogDebug("Closing socket. User does not have permission.");

                            var msg = SerializeMessage(PacketMessage, new LogMessageDto
                            {
                                Message = $"Missing \"OpenMod.WebServer:{PermissionAccessConsole}\" permission to access console."
                            });

                            await SendAsync(context, msg);
                            await CloseAsync(context);
                            return;
                        }

                        var canReadLogs = false; // await _permissionChecker.CheckPermissionAsync(user, PermissionAccessLogs) == PermissionGrantResult.Grant;
                        context.Items.Add("user", canReadLogs ? user : new WebConsoleUser(user, context, this));
                        context.Items.Add("canReadLogs", canReadLogs);

                        _logger.LogDebug($"User accepted: {user.FullActorName} (canReadLogs: {canReadLogs})");
                        break;
                    }

                case PacketMessage:
                    {
                        if (!context.Items.ContainsKey("user"))
                        {
                            _logger.LogDebug("Closing socket. User not authenticated.");
                            await CloseAsync(context);
                            return;
                        }

                        var user = (IUser)context.Items["user"];
                        var message = System.Text.Encoding.UTF8.GetString(buffer, 1, buffer.Length - 1);
                        var args = ArgumentsParser.ParseArguments(message);

                        await _commandExecutor.ExecuteAsync(user, args, string.Empty);
                        break;
                    }
            }
        }

        public Task OnActorMessage(WebConsoleUser user, string message)
        {
            var serializedMessage = new LogMessageDto { Message = message, LogLevel = LogLevel.Information };
            var json = SerializeMessage(PacketMessage, serializedMessage);
            return SendAsync(user.Context, json);
        }
    }
}

