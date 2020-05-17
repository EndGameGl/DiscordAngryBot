using Newtonsoft.Json;
using ObjectDiscordAPI.Extensions;
using ObjectDiscordAPI.GatewayData;
using ObjectDiscordAPI.GatewayData.GatewayCommands;
using ObjectDiscordAPI.GatewayData.GatewayEvents;
using ObjectDiscordAPI.GatewayOperations;
using ObjectDiscordAPI.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ObjectDiscordAPI
{
    public enum StatusCodes
    {
        Dispatch = 0,
        Heartbeat = 1,
        Identify = 2,
        PresenceUpdate = 3,
        VoiceStateUpdate = 4,
        Resume = 6,
        Reconnect = 7,
        RequestGuildMembers = 8,
        InvalidSession = 9,
        Hello = 10,
        HeartbeatACK = 11
    }
    public class DiscordClient
    {
        private static List<GatewayEventGuildCreateArgs> guildList = new List<GatewayEventGuildCreateArgs>();
        WebClient client { get; set; }
        bool isConfigured { get; set; } = false;
        static ClientWebSocket socket { get; set; }
        static CancellationTokenSource cancellationTokenSource { get; set; }
        static Timer heartbeatTimer { get; set; }
        static int? lastSequence { get; set; } = null;
        static string BotToken { get; set; }
        static string SessionID { get; set; }
        private bool IsReady { get; set; } = false;

        delegate Task HelloHandler(GatewayPayload payload);
        event HelloHandler OnHello;

        public delegate Task ReadyHandler();
        public event ReadyHandler Ready;

        public delegate Task CreateGuildHandler(GatewayEventGuildCreateArgs e);
        public event CreateGuildHandler GuildCreated;

        public delegate Task MessageCreatedHandler(Message e);
        public event MessageCreatedHandler MessageCreated;

        public delegate Task MessageUpdatedHandler(Message e);
        public event MessageUpdatedHandler MessageUpdated;

        public delegate Task MessageDeletedHandler(GatewayEventMessageDeleteArgs e);
        public event MessageDeletedHandler MessageDeleted;

        public void SetSettings(string botToken)
        {
            client = new WebClient();
            client.BaseAddress = apiPath.DiscordAPIPath;
            client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            client.Headers.Add(HttpRequestHeader.Authorization, $"Bot {botToken}");
            BotToken = botToken;
            socket = new ClientWebSocket();
            cancellationTokenSource = new CancellationTokenSource();

            OnHello += InnerHelloTask;
            Ready += InnerReadyTask;
            GuildCreated += InnerGuildCreatedTask;
            MessageCreated += InnerMessageCreatedTask;
            MessageUpdated += InnerMessageUpdatedTask;
            MessageDeleted += InnerMessageDeletedTask;
            isConfigured = true;
        }

        public async Task<string> GET(string parameters)
        {
            if (isConfigured)
            {
                try
                {
                    return await client.DownloadStringTaskAsync(new Uri(client.BaseAddress + parameters));
                }
                catch (Exception ex)
                {
                    throw new Exception("GET string operation error", ex);
                }
            }
            else
                return null;
        }

        public async Task<byte[]> GETFile(string parameters)
        {
            if (isConfigured)
            {
                try
                {
                    return await client.DownloadDataTaskAsync(new Uri(client.BaseAddress + parameters));
                }
                catch (Exception ex)
                {
                    throw new Exception("GET File operation error", ex);
                }
            }
            else
                return null;
        }

        public async Task ConnectAsync()
        {
            var gateway = await this.GetGatewayAsync();

            await socket.ConnectAsync(new Uri($"{gateway.URL}/?v=6&encoding=json"), cancellationTokenSource.Token);

            await Task.Factory.StartNew(
                async () =>
                {
                    ArraySegment<Byte> buffer = new ArraySegment<byte>(new Byte[4096]);
                    WebSocketReceiveResult result = null;
                    while (true)
                    {
                        using (var ms = new MemoryStream())
                        {
                            do
                            {
                                result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                                ms.Write(buffer.Array, buffer.Offset, result.Count);
                            }
                            while (!result.EndOfMessage);

                            ms.Seek(0, SeekOrigin.Begin);

                            if (result.MessageType == WebSocketMessageType.Text)
                            {
                                using (var reader = new StreamReader(ms, Encoding.UTF8))
                                {
                                    var payloadData = await Task.Run(async () => JsonConvert.DeserializeObject<GatewayPayload>(await reader.ReadToEndAsync()));                                   
                                    await Task.Run(() => Console.WriteLine($"{DateTime.Now}: Payload received:\n    Payload code: {payloadData.OperationCode}: {(StatusCodes)payloadData.OperationCode}\n" +
                                        $"    Payload sequence number: {payloadData.SequenceNumber}\n    Payload event name: {payloadData.EventName}\n"));

                                    switch ((StatusCodes)payloadData.OperationCode)
                                    {
                                        case StatusCodes.Dispatch:
                                            lastSequence = payloadData.SequenceNumber;
                                            await ProcessEvent(payloadData);
                                            break;
                                        case StatusCodes.Heartbeat:
                                            await SendHeartbeat(lastSequence);
                                            break;
                                        case StatusCodes.Identify:
                                            // Placeholder, send only
                                            break;
                                        case StatusCodes.PresenceUpdate:
                                            // Placeholder, send only
                                            break;
                                        case StatusCodes.VoiceStateUpdate:
                                            // Placeholder, send only
                                            break;
                                        case StatusCodes.Resume:
                                            // Placeholder, send only
                                            break;
                                        case StatusCodes.Reconnect:
                                            await Task.Run(() => Console.WriteLine("Discord gateway requested reconnect."));
                                            await Task.Run(() => Console.WriteLine("Closing connection..."));
                                            await socket.CloseAsync(WebSocketCloseStatus.Empty, "Reconnect requested", cancellationTokenSource.Token);
                                            await Task.Run(() => Console.WriteLine("Connecting..."));
                                            await socket.ConnectAsync(new Uri($"{gateway.URL}/?v=6&encoding=json"), cancellationTokenSource.Token);
                                            await SendResume();
                                            break;
                                        case StatusCodes.RequestGuildMembers:
                                            // Placeholder, send only
                                            break;
                                        case StatusCodes.InvalidSession:
                                            await Task.Run(() => Console.WriteLine("Invalid session. Sending identify"));
                                            await Identify();
                                            break;
                                        case StatusCodes.Hello:
                                            OnHello?.Invoke(payloadData);
                                            break;
                                        case StatusCodes.HeartbeatACK:
                                            // Receiving response heartbeat
                                            break;
                                    }
                                }
                            }
                        }                     
                    }
                }, cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);           
        }

        private static async Task InnerHelloTask(GatewayPayload payload)
        {
            var op = await Task.Run( () => JsonConvert.DeserializeObject<GatewayEventHelloArgs>(payload.JSONEventData.ToString()));
            await SendHeartbeat(lastSequence);
            await Identify();
            heartbeatTimer = new Timer( 
                async (obj) => 
                {
                    await SendHeartbeat(lastSequence);
                }, null, op.HeartbeatInterval, op.HeartbeatInterval);
        }

        private async Task ProcessEvent(GatewayPayload gatewayPayload)
        {
            switch (gatewayPayload.EventName)
            {
                case "READY":
                    var readyData = await Task.Run(() => JsonConvert.DeserializeObject<GatewayEventReadyArgs>(gatewayPayload.JSONEventData.ToString()));
                    SessionID = readyData.SessionID;
                    await Ready?.Invoke();
                    break;
                case "GUILD_CREATE":
                    var guildCreateData = await Task.Run(() => JsonConvert.DeserializeObject<GatewayEventGuildCreateArgs>(gatewayPayload.JSONEventData.ToString()));
                    guildList.Add(guildCreateData);
                    await GuildCreated?.Invoke(guildCreateData);
                    break;
                case "CHANNEL_CREATE":
                    break;
                case "CHANNEL_UPDATE":
                    break;
                case "CHANNEL_DELETE":
                    break;
                case "CHANNEL_PINS_UPDATE":
                    break;
                case "GUILD_UPDATE":
                    break;
                case "GUILD_DELETE":
                    break;
                case "GUILD_BAN_ADD":
                    break;
                case "GUILD_BAN_REMOVE":
                    break;
                case "GUILD_EMOJIS_UPDATE":
                    break;
                case "GUILD_INTEGRATIONS_UPDATE":
                    break;
                case "GUILD_MEMBER_ADD":
                    break;
                case "GUILD_MEMBER_REMOVE":
                    break;
                case "GUILD_MEMBER_UPDATE":
                    break;
                case "GUILD_MEMBERS_CHUNK":
                    break;
                case "GUILD_ROLE_CREATE":
                    break;
                case "GUILD_ROLE_UPDATE":
                    break;
                case "GUILD_ROLE_DELETE":
                    break;
                case "INVITE_CREATE":
                    break;
                case "INVITE_DELETE":
                    break;
                case "MESSAGE_CREATE":
                    var createdMessage = await Task.Run(() => JsonConvert.DeserializeObject<Message>(gatewayPayload.JSONEventData.ToString()));
                    await MessageCreated?.Invoke(createdMessage);
                    break;
                case "MESSAGE_UPDATE":
                    var updatedMessage = await Task.Run(() => JsonConvert.DeserializeObject<Message>(gatewayPayload.JSONEventData.ToString()));
                    await MessageUpdated?.Invoke(updatedMessage);
                    break;
                case "MESSAGE_DELETE":
                    var deletedMessageData = await Task.Run(() => JsonConvert.DeserializeObject<GatewayEventMessageDeleteArgs>(gatewayPayload.JSONEventData.ToString()));
                    await MessageDeleted?.Invoke(deletedMessageData);
                    break;
                case "MESSAGE_DELETE_BULK":
                    break;
                case "MESSAGE_REACTION_ADD":
                    break;
                case "MESSAGE_REACTION_REMOVE":
                    break;
                case "MESSAGE_REACTION_REMOVE_ALL":
                    break;
                case "MESSAGE_REACTION_REMOVE_EMOJI":
                    break;
                case "PRESENCE_UPDATE":
                    break;
                case "TYPING_START":
                    break;
                case "USER_UPDATE":
                    break;
                case "VOICE_STATE_UPDATE":
                    break;
                case "VOICE_SERVER_UPDATE":
                    break;
                case "WEBHOOKS_UPDATE":
                    break;
            }
        }

        private static async Task Identify()
        {
            var identifyOperation = new
            {
                op = 2,
                d = new IdentifyData()
                {
                    Token = "Bot " + BotToken,
                    IdentityConnectionProperties = new IdentityConnectionProperties()
                    {
                        Browser = "library",
                        Device = "library",
                        OS = "Windows"
                    },
                    DoGuildSubscriptions = true
                }
            };
            Console.WriteLine($"{DateTime.Now}: Sending identity data");
            await socket.SendAsync(await identifyOperation.ConvertObjectToArraySegment(false), WebSocketMessageType.Text, true, cancellationTokenSource.Token);
        }

        private static async Task SendHeartbeat(int? seq)
        {
            GatewayHeartbeat heartbeat = new GatewayHeartbeat()
            {
                LastSequence = seq,
                Operation = 1
            };
            Console.WriteLine($"{DateTime.Now}: Sending heartbeat {seq}");
            await socket.SendAsync(await heartbeat.ConvertObjectToArraySegment(true), WebSocketMessageType.Text, true, cancellationTokenSource.Token);
        }

        private static async Task SendResume()
        {
            GatewayResume gatewayResume = new GatewayResume()
            {
                SequenceNumber = lastSequence,
                SessionID = SessionID,
                Token = "Bot " + BotToken
            };
            GatewayPayload payload = new GatewayPayload() 
            { 
                EventName = null,
                JSONEventData = gatewayResume,
                OperationCode = (int)StatusCodes.Resume,
                SequenceNumber = lastSequence
            };
            Console.WriteLine($"{DateTime.Now}: Resuming... {gatewayResume.SessionID}");
            await socket.SendAsync(await payload.ConvertObjectToArraySegment(true), WebSocketMessageType.Text, true, cancellationTokenSource.Token);
        }

        private static async Task InnerReadyTask()
        {
            await Task.Run(() => Console.WriteLine($"[{DateTime.Now}]: Client is ready."));
        }

        private static async Task InnerGuildCreatedTask(GatewayEventGuildCreateArgs e)
        {
            await Task.Run(() => Console.WriteLine($"Guild {e.Name} is available for use"));
        }

        private static async Task InnerMessageCreatedTask(Message e)
        {

        }

        private static async Task InnerMessageUpdatedTask(Message e)
        {

        }

        private static async Task InnerMessageDeletedTask(GatewayEventMessageDeleteArgs e)
        {

        }

        public async Task<GatewayEventGuildCreateArgs> GetGuildByNameAsync(string Name)
        {
            var guild = await Task.Run(() => guildList.Where(x => x.Name == Name).SingleOrDefault());
            return guild;
        }

        public async Task<GatewayEventGuildCreateArgs> GetGuildByIDAsync(ulong ID)
        {
            var guild = await Task.Run(() => guildList.Where(x => x.ID == ID).SingleOrDefault());
            return guild;
        }
    }
}
