using Newtonsoft.Json;
using ObjectDiscordAPI.CacheData;
using ObjectDiscordAPI.Extensions;
using ObjectDiscordAPI.GatewayData;
using ObjectDiscordAPI.GatewayData.GatewayCommands;
using ObjectDiscordAPI.GatewayData.GatewayEvents;
using ObjectDiscordAPI.GatewayOperations;
using ObjectDiscordAPI.Resources;
using ObjectDiscordAPI.Resources.GuildResources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
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
        /// <summary>
        /// Cached guild list
        /// </summary>
        private static List<GatewayEventGuildCreateArgs> guildList = new List<GatewayEventGuildCreateArgs>();
        /// <summary>
        /// Cached messages within all channels
        /// </summary>
        private static List<MessagesCache> messagesCache { get; set; } = new List<MessagesCache>();
        /// <summary>
        /// Web client for sending API requests
        /// </summary>
        static WebClient client { get; set; }
        /// <summary>
        /// Whether Discord client is configured
        /// </summary>
        static bool isConfigured { get; set; } = false;
        /// <summary>
        /// Web socket client for maintaining gateway connection
        /// </summary>
        static ClientWebSocket socket { get; set; }
        /// <summary>
        /// Cancellation token source
        /// </summary>
        static CancellationTokenSource cancellationTokenSource { get; set; }
        /// <summary>
        /// Timer for sending heartbeat payloads
        /// </summary>
        static Timer heartbeatTimer { get; set; }
        /// <summary>
        /// Last payload sequence for heartbeat
        /// </summary>
        static int? lastSequence { get; set; } = null;
        /// <summary>
        /// Bot token
        /// </summary>
        static string BotToken { get; set; }
        /// <summary>
        /// Last session ID
        /// </summary>
        static string SessionID { get; set; }
        /// <summary>
        /// Whether client is ready for work
        /// </summary>
        private bool IsReady { get; set; } = false;

        #region Events and delegates
        /// <summary>
        /// HELLO event handler
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        delegate Task HelloHandler(GatewayPayload payload);
        /// <summary>
        /// Defines the heartbeat interval
        /// </summary>
        event HelloHandler OnHello;

        /// <summary>
        /// READY event handler
        /// </summary>
        /// <returns></returns>
        private delegate Task InnerReadyHandler();
        /// <summary>
        /// Contains the initial state information
        /// </summary>
        private event InnerReadyHandler InnerReady;

        /// <summary>
        /// GUILD_CREATE event handler
        /// </summary>
        /// <param name="e">Guild created event parameters</param>
        /// <returns></returns>
        private delegate Task InnerCreateGuildHandler(GatewayEventGuildCreateArgs e);
        /// <summary>
        /// Lazy-load for unavailable guild, guild became available, or user joined a new guild
        /// </summary>
        private event InnerCreateGuildHandler InnerGuildCreated;

        /// <summary>
        /// GUILD_UPDATE event handler
        /// </summary>
        /// <param name="e">Guild updated event parameters</param>
        /// <returns></returns>
        public delegate Task UpdateGuildHandler(GatewayEventGuildCreateArgs e);
        /// <summary>
        /// Guild was updated
        /// </summary>
        public event UpdateGuildHandler GuildUpdated;

        /// <summary>
        /// MESSAGE_CREATE event handler
        /// </summary>
        /// <param name="e">Message created event parameters</param>
        /// <returns></returns>
        public delegate Task MessageCreatedHandler(Message e);
        /// <summary>
        /// Message was created
        /// </summary>
        public event MessageCreatedHandler MessageCreated;

        /// <summary>
        /// MESSAGE_UPDATE event handler
        /// </summary>
        /// <param name="e">Message updated event parameters</param>
        /// <returns></returns>
        public delegate Task MessageUpdatedHandler(Message e);
        /// <summary>
        /// Message was edited
        /// </summary>
        public event MessageUpdatedHandler MessageUpdated;

        /// <summary>
        /// MESSAGE_DELETE event handler
        /// </summary>
        /// <param name="e">Message deletion event parameters</param>
        /// <returns></returns>
        public delegate Task MessageDeletedHandler(GatewayEventMessageDeleteArgs e);
        /// <summary>
        /// Message was deleted
        /// </summary>
        public event MessageDeletedHandler MessageDeleted;

        /// <summary>
        /// MESSAGE_DELETE_BULK event handler
        /// </summary>
        /// <param name="e">Message bulk deletion event parameters</param>
        /// <returns></returns>
        public delegate Task MessageBulkDeletedHandler(MessageDeleteBulk e);
        /// <summary>
        /// Multiple messages were deleted at once
        /// </summary>
        public event MessageBulkDeletedHandler MessageBulkDeleted;

        /// <summary>
        /// CHANNEL_CREATE event handler
        /// </summary>
        /// <param name="e">Channel created event parameters</param>
        /// <returns></returns>
        public delegate Task ChannelCreatedHandler(Channel e);
        /// <summary>
        /// New channel created
        /// </summary>
        public event ChannelCreatedHandler ChannelCreated;

        /// <summary>
        /// CHANNEL_UPDATE event handler
        /// </summary>
        /// <param name="e">Channel update event parameters</param>
        /// <returns></returns>
        public delegate Task ChannelUpdatedHandler(Channel e);
        /// <summary>
        /// Channel was updated
        /// </summary>
        public event ChannelUpdatedHandler ChannelUpdated;

        /// <summary>
        /// CHANNEL_DELETE event handler
        /// </summary>
        /// <param name="e">Channel deletion event parameters</param>
        /// <returns></returns>
        public delegate Task ChannelDeletedHandler(Channel e);
        /// <summary>
        /// Channel was deleted
        /// </summary>
        public event ChannelDeletedHandler ChannelDeleted;

        /// <summary>
        /// CHANNEL_PINS_UPDATE event handler
        /// </summary>
        /// <param name="e">Channel pins update event parameters</param>
        /// <returns></returns>
        public delegate Task ChannelPinsUpdatedHandler(GatewayEventChannelPinsUpdateArgs e);
        /// <summary>
        /// Message was pinned or unpinned
        /// </summary>
        public event ChannelPinsUpdatedHandler ChannelPinsUpdated;

        /// <summary>
        /// GUILD_DELETE event handler
        /// </summary>
        /// <param name="e">Guild deletion event parameters</param>
        /// <returns></returns>
        public delegate Task GuildDeletedHandler(UnavailableGuild e);
        /// <summary>
        /// Guild became unavailable, or user left/was removed from a guild
        /// </summary>
        public event GuildDeletedHandler GuildDeleted;

        /// <summary>
        /// MESSAGE_REACTION_ADD event handler
        /// </summary>
        /// <param name="e">Message reaction added event parameters</param>
        /// <returns></returns>
        public delegate Task MessageReactionAddedHandler(GatewayEventMessageReactionAddedArgs e);
        /// <summary>
        /// User reacted to a message
        /// </summary>
        public event MessageReactionAddedHandler MessageReactionAdded;

        /// <summary>
        /// MESSAGE_REACTION_REMOVE event handler
        /// </summary>
        /// <param name="e">Message reaction removed event parameters</param>
        /// <returns></returns>
        public delegate Task MessageReactionRemovedHandler(GatewayEventMessageReactionRemovedArgs e);
        /// <summary>
        /// User removed a reaction from a message
        /// </summary>
        public event MessageReactionRemovedHandler MessageReactionRemoved;

        /// <summary>
        /// MESSAGE_REACTION_REMOVE_ALL event handler
        /// </summary>
        /// <param name="e">Message all reaction removed event parameters</param>
        /// <returns></returns>
        public delegate Task MessageAllReactionRemovedHandler(GatewayEventMessageAllReactionRemovedArgs e);
        /// <summary>
        /// All reactions were explicitly removed from a message
        /// </summary>
        public event MessageAllReactionRemovedHandler MessageAllReactionRemoved;

        /// <summary>
        /// PRESENCE_UPDATE event handler
        /// </summary>
        /// <param name="e">Presence update event parameters</param>
        /// <returns></returns>
        public delegate Task PresenceUpdatedHandler(PresenceUpdate e);
        /// <summary>
        /// User was updated
        /// </summary>
        public event PresenceUpdatedHandler PresenceUpdated;

        /// <summary>
        /// MESSAGE_REACTION_REMOVE_EMOJI event handler
        /// </summary>
        /// <param name="e">Message emoji reaction removed event parameters</param>
        /// <returns></returns>
        public delegate Task MessageReactionEmojiRemovedHandler(GatewayEventMessageReactionEmojiRemovedArgs e);
        /// <summary>
        /// All reactions were explicitly removed from a message
        /// </summary>
        public event MessageReactionEmojiRemovedHandler MessageReactionEmojiRemoved;

        /// <summary>
        /// TYPING_START event handler
        /// </summary>
        /// <param name="e">Typing start event parameters</param>
        /// <returns></returns>
        public delegate Task TypingStartHandler(GatewayEventTypingStartArgs e);
        /// <summary>
        /// User started typing in a channel
        /// </summary>
        public event TypingStartHandler TypingStart;

        /// <summary>
        /// USER_UPDATE event handler
        /// </summary>
        /// <param name="e">User updated event parameters</param>
        /// <returns></returns>
        public delegate Task UserUpdatedHandler(User e);
        /// <summary>
        /// Properties about the user changed
        /// </summary>
        public event UserUpdatedHandler UserUpdated;

        /// <summary>
        /// VOICE_STATE_UPDATE event handler
        /// </summary>
        /// <param name="e">Voice state updated event parameters</param>
        /// <returns></returns>
        public delegate Task VoiceStateUpdatedHandler(VoiceState e);
        /// <summary>
        /// Someone joined, left, or moved a voice channel
        /// </summary>
        public event VoiceStateUpdatedHandler VoiceStateUpdated;

        /// <summary>
        /// VOICE_SERVER_UPDATE event handler
        /// </summary>
        /// <param name="e">Voice server updated event parameters</param>
        /// <returns></returns>
        public delegate Task VoiceServerUpdatedHandler(GatewayEventsVoiceServerUpdatedArgs e);
        /// <summary>
        /// Guild's voice server was updated
        /// </summary>
        public event VoiceServerUpdatedHandler VoiceServerUpdated;

        /// <summary>
        /// WEBHOOKS_UPDATE event handler
        /// </summary>
        /// <param name="e">Webhooks updated event parameters</param>
        /// <returns></returns>
        public delegate Task WebhooksUpdatedHandler(GatewayEventWebhookUpdatedArgs e);
        /// <summary>
        /// Guild channel webhook was created, update, or deleted
        /// </summary>
        public event WebhooksUpdatedHandler WebhooksUpdated;

        /// <summary>
        /// GUILD_BAN_ADD event handler
        /// </summary>
        /// <param name="e">Guild ban added event parameters</param>
        /// <returns></returns>
        public delegate Task GuildBanAddedHandler(GatewayEventGuildBanArgs e);
        /// <summary>
        /// User was banned from a guild
        /// </summary>
        public event GuildBanAddedHandler GuildBanAdded;

        /// <summary>
        /// GUILD_BAN_REMOVE event handler
        /// </summary>
        /// <param name="e">Guild ban added event parameters</param>
        /// <returns></returns>
        public delegate Task GuildBanRemovedHandler(GatewayEventGuildBanArgs e);
        /// <summary>
        /// User was unbanned from a guild
        /// </summary>
        public event GuildBanRemovedHandler GuildBanRemoved;

        /// <summary>
        /// GUILD_EMOJIS_UPDATE event handler
        /// </summary>
        /// <param name="e">Guild emojis updated event parameters</param>
        /// <returns></returns>
        public delegate Task GuildEmojisUpdatedHandler(GatewayEventGuildEmojisUpdateArgs e);
        /// <summary>
        /// Guild emojis were updated
        /// </summary>
        public event GuildEmojisUpdatedHandler GuildEmojisUpdated;

        /// <summary>
        /// GUILD_INTEGRATIONS_UPDATE event handler
        /// </summary>
        /// <param name="e">Guild integrations updated event parameters</param>
        /// <returns></returns>
        public delegate Task GuildIntegrationsUpdatedHandler(GatewayEventGuildIntegrationsUpdatedArgs e);
        /// <summary>
        /// Guild integration was updated
        /// </summary>
        public event GuildIntegrationsUpdatedHandler GuildIntegrationsUpdated;

        /// <summary>
        /// GUILD_MEMBER_ADD event handler
        /// </summary>
        /// <param name="e">Guild member added event parameters</param>
        /// <returns></returns>
        public delegate Task GuildMemberAddedHandler(GatewayEventGuildMemberArgs e);
        /// <summary>
        /// New user joined a guild
        /// </summary>
        public event GuildMemberAddedHandler GuildMemberAdded;

        #endregion

        /// <summary>
        /// Set up all client settings to run bot
        /// </summary>
        /// <param name="botToken">Bot token for authentification</param>
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
            InnerReady += InnerReadyTask;

            InnerGuildCreated += InnerGuildCreatedTask;
            GuildUpdated += InnerGuildUpdatedTask;
            GuildDeleted += InnerGuildDeletedTask;
            GuildBanAdded += InnerGuildBanAddedTask;
            GuildBanRemoved += InnerGuildBanRemovedTask;
            GuildEmojisUpdated += InnerGuildEmojisUpdatedTask;
            GuildIntegrationsUpdated += InnerGuildIntegrationsUpdatedTask;
            GuildMemberAdded += InnerGuildMemberAddedTask;


            MessageCreated += InnerMessageCreatedTask;
            MessageUpdated += InnerMessageUpdatedTask;
            MessageDeleted += InnerMessageDeletedTask;
            MessageBulkDeleted += InnerMessageBulkDeletedTask;
            MessageReactionAdded += InnerMessageReactionAddedTask;
            MessageReactionRemoved += InnerMessageReactionRemovedTask;
            MessageAllReactionRemoved += InnerMessageAllReactionRemovedTask;
            MessageReactionEmojiRemoved += InnerMessageReactionEmojiRemovedTask;

            ChannelCreated += InnerChannelCreatedTask;
            ChannelUpdated += InnerChannelUpdatedTask;
            ChannelDeleted += InnerChannelDeletedTask;
            ChannelPinsUpdated += InnerChannelPinsUpdatedTask;

            PresenceUpdated += InnerPresenceUpdatedTask;
            TypingStart += InnerTypingStartTask;
            UserUpdated += InnerUserUpdatedTask;

            VoiceStateUpdated += InnerVoiceStateUpdatedTask;
            VoiceServerUpdated += InnerVoiceServerUpdatedTask;

            WebhooksUpdated += InnerWebhooksUpdatedTask;

            isConfigured = true;
        }

        public static async Task<string> GET(string parameters)
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

        public static async Task<byte[]> GETFile(string parameters)
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

        public static async Task PUT(string parameters, string data)
        {
            if (isConfigured)
            {
                try
                {
                    await Task.Run(() => client.UploadStringAsync(new Uri(client.BaseAddress + parameters), "PUT", data));
                }
                catch (Exception ex)
                {
                    throw new Exception("PUT string operation error", ex);
                }
            }
        }

        public static async Task PATCH(string parameters, string data)
        {
            if (isConfigured)
            {
                try
                {
                    await Task.Run(() => client.UploadStringAsync(new Uri(client.BaseAddress + parameters), "PATCH", data));
                }
                catch (Exception ex)
                {
                    throw new Exception("PATCH string operation error", ex);
                }
            }
        }

        public static async Task DELETE(string parameters, string data)
        {
            if (isConfigured)
            {
                try
                {
                    await Task.Run(() => client.UploadStringAsync(new Uri(client.BaseAddress + parameters), "DELETE", data));
                }
                catch (Exception ex)
                {
                    throw new Exception("DELETE string operation error", ex);
                }
            }
        }

        public static async Task POST(string parameters, string data)
        {
            if (isConfigured)
            {
                try
                {
                    await Task.Run(() => client.UploadStringAsync(new Uri(client.BaseAddress + parameters), "POST", data));
                }
                catch (Exception ex)
                {
                    throw new Exception("POST string operation error", ex);
                }
            }
        }

        /// <summary>
        /// Task for connecting to Discord gateway and managing all events
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// Task for managing HELLO gateway payload
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Task for processing gateway Dispatch payloads
        /// </summary>
        /// <param name="gatewayPayload">Gateway payload data object</param>
        /// <returns></returns>
        private async Task ProcessEvent(GatewayPayload gatewayPayload)
        {
            try
            {
                switch (gatewayPayload.EventName)
                {
                    case "READY":
                        var readyData = await Task.Run(() => JsonConvert.DeserializeObject<GatewayEventReadyArgs>(gatewayPayload.JSONEventData.ToString()));
                        SessionID = readyData.SessionID;
                        await InnerReady?.Invoke();
                        break;
                    case "GUILD_CREATE":
                        var guildCreateData = await Task.Run(() => JsonConvert.DeserializeObject<GatewayEventGuildCreateArgs>(gatewayPayload.JSONEventData.ToString()));
                        await InnerGuildCreated?.Invoke(guildCreateData);
                        break;
                    case "CHANNEL_CREATE":
                        var channelCreatedData = await Task.Run(() => JsonConvert.DeserializeObject<Channel>(gatewayPayload.JSONEventData.ToString()));
                        await ChannelCreated?.Invoke(channelCreatedData);
                        break;
                    case "CHANNEL_UPDATE":
                        var channelUpdatedData = await Task.Run(() => JsonConvert.DeserializeObject<Channel>(gatewayPayload.JSONEventData.ToString()));
                        await ChannelUpdated?.Invoke(channelUpdatedData);
                        break;
                    case "CHANNEL_DELETE":
                        var channelDeletedData = await Task.Run(() => JsonConvert.DeserializeObject<Channel>(gatewayPayload.JSONEventData.ToString()));
                        await ChannelDeleted?.Invoke(channelDeletedData);
                        break;
                    case "CHANNEL_PINS_UPDATE":
                        var channelPinsUpdateData = await Task.Run(() => JsonConvert.DeserializeObject<GatewayEventChannelPinsUpdateArgs>(gatewayPayload.JSONEventData.ToString()));
                        await ChannelPinsUpdated?.Invoke(channelPinsUpdateData);
                        break;
                    case "GUILD_UPDATE":
                        var guildUpdatedData = await Task.Run(() => JsonConvert.DeserializeObject<GatewayEventGuildCreateArgs>(gatewayPayload.JSONEventData.ToString()));
                        await GuildUpdated?.Invoke(guildUpdatedData);
                        break;
                    case "GUILD_DELETE":
                        var guildDeletedData = await Task.Run(() => JsonConvert.DeserializeObject<UnavailableGuild>(gatewayPayload.JSONEventData.ToString()));
                        await GuildDeleted?.Invoke(guildDeletedData);
                        break;
                    case "GUILD_BAN_ADD":
                        var guildBanAddedData = await Task.Run(() => JsonConvert.DeserializeObject<GatewayEventGuildBanArgs>(gatewayPayload.JSONEventData.ToString()));
                        await GuildBanAdded?.Invoke(guildBanAddedData);
                        break;
                    case "GUILD_BAN_REMOVE":
                        var guildBanRemovedData = await Task.Run(() => JsonConvert.DeserializeObject<GatewayEventGuildBanArgs>(gatewayPayload.JSONEventData.ToString()));
                        await GuildBanRemoved?.Invoke(guildBanRemovedData);
                        break;
                    case "GUILD_EMOJIS_UPDATE":
                        var guildEmojisUpdatedData = await Task.Run(() => JsonConvert.DeserializeObject<GatewayEventGuildEmojisUpdateArgs>(gatewayPayload.JSONEventData.ToString()));
                        await GuildEmojisUpdated?.Invoke(guildEmojisUpdatedData);
                        break;
                    case "GUILD_INTEGRATIONS_UPDATE":
                        var guildIntegrationsUpdatedData = await Task.Run(() => JsonConvert.DeserializeObject<GatewayEventGuildIntegrationsUpdatedArgs>(gatewayPayload.JSONEventData.ToString()));
                        await GuildIntegrationsUpdated?.Invoke(guildIntegrationsUpdatedData);
                        break;
                    case "GUILD_MEMBER_ADD":
                        var guildMemberAddedData = await Task.Run(() => JsonConvert.DeserializeObject<GatewayEventGuildMemberArgs>(gatewayPayload.JSONEventData.ToString()));
                        GuildMemberAdded?.Invoke(guildMemberAddedData);
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
                        var deletedMessagesBulkData = await Task.Run(() => JsonConvert.DeserializeObject<MessageDeleteBulk>(gatewayPayload.JSONEventData.ToString()));
                        await MessageBulkDeleted?.Invoke(deletedMessagesBulkData);
                        break;
                    case "MESSAGE_REACTION_ADD":
                        var messageReactionAddedData = await Task.Run(() => JsonConvert.DeserializeObject<GatewayEventMessageReactionAddedArgs>(gatewayPayload.JSONEventData.ToString()));
                        await MessageReactionAdded?.Invoke(messageReactionAddedData);
                        break;
                    case "MESSAGE_REACTION_REMOVE":
                        var messageReactionRemovedData = await Task.Run(() => JsonConvert.DeserializeObject<GatewayEventMessageReactionRemovedArgs>(gatewayPayload.JSONEventData.ToString()));
                        await MessageReactionRemoved?.Invoke(messageReactionRemovedData);
                        break;
                    case "MESSAGE_REACTION_REMOVE_ALL":
                        var messageAllReactionRemovedData = await Task.Run(() => JsonConvert.DeserializeObject<GatewayEventMessageAllReactionRemovedArgs>(gatewayPayload.JSONEventData.ToString()));
                        await MessageAllReactionRemoved?.Invoke(messageAllReactionRemovedData);
                        break;
                    case "MESSAGE_REACTION_REMOVE_EMOJI":
                        var messageReactionEmojiRemovedData = JsonConvert.DeserializeObject<GatewayEventMessageReactionEmojiRemovedArgs>(gatewayPayload.JSONEventData.ToString());
                        await MessageReactionEmojiRemoved?.Invoke(messageReactionEmojiRemovedData);
                        break;
                    case "PRESENCE_UPDATE":
                        var presenceUpdateData = JsonConvert.DeserializeObject<PresenceUpdate>(gatewayPayload.JSONEventData.ToString());
                        await PresenceUpdated?.Invoke(presenceUpdateData);
                        break;
                    case "TYPING_START":
                        var typingStartData = JsonConvert.DeserializeObject<GatewayEventTypingStartArgs>(gatewayPayload.JSONEventData.ToString());
                        await TypingStart?.Invoke(typingStartData);
                        break;
                    case "USER_UPDATE":
                        var userUpdatedData = JsonConvert.DeserializeObject<User>(gatewayPayload.JSONEventData.ToString());
                        await UserUpdated?.Invoke(userUpdatedData);
                        break;
                    case "VOICE_STATE_UPDATE":
                        var voiceStateUpdateData = JsonConvert.DeserializeObject<VoiceState>(gatewayPayload.JSONEventData.ToString());
                        await VoiceStateUpdated?.Invoke(voiceStateUpdateData);
                        break;
                    case "VOICE_SERVER_UPDATE":
                        var voiceServerUpdatedData = JsonConvert.DeserializeObject<GatewayEventsVoiceServerUpdatedArgs>(gatewayPayload.JSONEventData.ToString());
                        await VoiceServerUpdated?.Invoke(voiceServerUpdatedData);
                        break;
                    case "WEBHOOKS_UPDATE":
                        var webhookUpdatedData = JsonConvert.DeserializeObject<GatewayEventWebhookUpdatedArgs>(gatewayPayload.JSONEventData.ToString());
                        await WebhooksUpdated?.Invoke(webhookUpdatedData);
                        break;
                }
            }
            catch (Exception e)
            {
                await Task.Run(() => Console.WriteLine($"{e.Message}: {e.InnerException?.Message} "));
            }
        }
        /// <summary>
        /// Task for sending IDENTIDY payload to Discord gateway
        /// </summary>
        /// <returns></returns>
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
                    DoGuildSubscriptions = false                   
                }
            };
            Console.WriteLine($"{DateTime.Now}: Sending identity data");
            await socket.SendAsync(await identifyOperation.ConvertObjectToArraySegment(false), WebSocketMessageType.Text, true, cancellationTokenSource.Token);
        }
        /// <summary>
        /// Task for sending HEARTBEAT payload to Discord gateway
        /// </summary>
        /// <param name="seq"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Task for sending RESUME payload to Discord gateway
        /// </summary>
        /// <returns></returns>
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



        #region Inner event tasks
        /// <summary>
        /// Task for managing READY gateway event before user actions
        /// </summary>
        /// <returns></returns>
        private static async Task InnerReadyTask()
        {
            await Task.Run(() => Console.WriteLine($"[{DateTime.Now}]: Client is ready."));
        }
        /// <summary>
        /// Task for managing GUILD_CREATE gateway event before user actions
        /// </summary>
        /// <param name="e">Discord guild object</param>
        /// <returns></returns>
        private async Task InnerGuildCreatedTask(GatewayEventGuildCreateArgs e)
        {
            await Task.Run(async () =>
            {
                Console.WriteLine($"Guild {e.Name} is available for use");
                
                if (guildList.Where(x => x.ID == e.ID).Count() == 0)
                {
                    e.Members = await this.GetGuildMembersAsync(e.ID, e.MemberCount);
                    guildList.Add(e);
                    Console.WriteLine($"Downloading message cache for {e.Name}");
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    foreach (var channel in e.Channels)
                    {
                        if (messagesCache.Where(x => x.ChannelID == channel.ID).Count() == 0 && channel.ChannelType == ChannelType.GuildText)
                        {
                            Console.WriteLine($"Downloading message cache for {e.Name} - {channel.Name}");
                            List<Message> messages = (await this.GetChannelMessagesAsync(channel.ID, 100)).ToList();
                            messagesCache.Add(new MessagesCache() { GuildID = e.ID, ChannelID = channel.ID, cachedData = messages });
                        }
                    }
                    sw.Stop();
                    Console.WriteLine($"Time to download all messages from {e.Name}: {sw.Elapsed.TotalSeconds} seconds");
                }                          
            });
        }
        /// <summary>
        /// Task for managing GUILD_DELETE gateway event before user actions
        /// </summary>
        /// <param name="e">Unavailable guild object</param>
        /// <returns></returns>
        private static async Task InnerGuildDeletedTask(UnavailableGuild e)
        {
            await Task.Run(() =>
            {
                Console.WriteLine($"Guild {e.ID} is no longer unavailable");
                var targetGuild = guildList.Where(x => x.ID == e.ID).SingleOrDefault();
                foreach (var guildChannel in targetGuild.Channels) 
                {
                    var targetChannelCache = messagesCache.Where(x => x.ChannelID == targetGuild.ID && x.GuildID == targetGuild.ID).SingleOrDefault();
                    if (targetChannelCache != null)
                        messagesCache.Remove(targetChannelCache);
                }
                if (targetGuild != null)
                    guildList.Remove(targetGuild);
                
            });
        }
        /// <summary>
        /// Task for managing GUILD_UPDATE gateway event before user actions
        /// </summary>
        /// <param name="e">Discord guild object</param>
        /// <returns></returns>
        private static async Task InnerGuildUpdatedTask(GatewayEventGuildCreateArgs e)
        {
            await Task.Run(async () => 
            { 
                Console.WriteLine($"Guild {e.Name} was updated");
                var guildToReplace = guildList.Where(x => x.ID == e.ID).SingleOrDefault();
                guildToReplace = await ObjectComparer.CompareAndUpdate(guildToReplace, e);
            });
        }
        /// <summary>
        /// Task for managing MESSAGE_CREATE gateway event before user actions
        /// </summary>
        /// <param name="e">Discord message object</param>
        /// <returns></returns>
        private static async Task InnerMessageCreatedTask(Message e)
        {
            await Task.Run(() => 
            {                
                messagesCache.Where(x => x.GuildID == e.GuildID && x.ChannelID == e.ChannelID).FirstOrDefault()?.cachedData.Add(e);
            });
        }
        /// <summary>
        /// Task for managing MESSAGE_UPDATE gateway event before user actions
        /// </summary>
        /// <param name="e">Discord message object</param>
        /// <returns></returns>
        private static async Task InnerMessageUpdatedTask(Message e)
        {
            await Task.Run(async () => 
            {
                MessagesCache targetCache = messagesCache.Where(x => x.GuildID == e.GuildID && x.ChannelID == e.ChannelID).SingleOrDefault();
                Message targetMessage = targetCache.cachedData.Where(x => x.ID == e.ID).SingleOrDefault();
                targetMessage = await ObjectComparer.CompareAndUpdate(targetMessage, e);
            });
        }
        /// <summary>
        /// Task for managing MESSAGE_DELETE gateway event before user actions
        /// </summary>
        /// <param name="e">Params of message deletion</param>
        /// <returns></returns>
        private static async Task InnerMessageDeletedTask(GatewayEventMessageDeleteArgs e)
        {
            await Task.Run(() => 
            {
                var targetCache = messagesCache.Where(x => x.GuildID == e.GuildID && x.ChannelID == e.ChannelID).SingleOrDefault();
                targetCache.cachedData.Remove(targetCache.cachedData.Where(x => x.ID == e.ID).FirstOrDefault());
            });
        }
        /// <summary>
        /// Task for managing MESSAGE_DELETE_BULK gateway event before user actions
        /// </summary>
        /// <param name="e">Params of message bulk deletion</param>
        /// <returns></returns>
        private static async Task InnerMessageBulkDeletedTask(MessageDeleteBulk e)
        {
            await Task.Run(() => 
            {
                var targetCache = messagesCache.Where(x => x.GuildID == e.GuildID && x.ChannelID == e.ChannelID).SingleOrDefault();
                foreach (var message in e.IDs)
                {
                    targetCache.cachedData.Remove(targetCache.cachedData.Where(x => x.ID == message).FirstOrDefault());
                }
            });
        }
        /// <summary>
        /// Task for managing CHANNEL_CREATE gateway event before user actions
        /// </summary>
        /// <param name="e">Discord channel object</param>
        /// <returns></returns>
        private static async Task InnerChannelCreatedTask(Channel e)
        {
            await Task.Run(() => 
            {
                messagesCache.Add(new MessagesCache() { GuildID = e.GuildID, ChannelID = e.ID, cachedData = new List<Message>() });
            });
        }
        /// <summary>
        /// Task for managing CHANNEL_UPDATE gateway event before user actions
        /// </summary>
        /// <param name="e">Discord channel object</param>
        /// <returns></returns>
        private static async Task InnerChannelUpdatedTask(Channel e)
        {
            await Task.Run(() => 
            { 
                if (e.GuildID != null)
                {
                    var targetChannel = guildList.Where(x => x.ID == e.GuildID).SingleOrDefault()?.Channels.Where(x => x.ID == e.ID).SingleOrDefault();
                    targetChannel = e;
                }
            });
        }
        /// <summary>
        /// Task for managing CHANNEL_DELETE gateway event before user actions
        /// </summary>
        /// <param name="e">Params of deleted channel</param>
        /// <returns></returns>
        private static async Task InnerChannelDeletedTask(Channel e)
        {
            await Task.Run(() => 
            {
                guildList.Where(x => x.ID == e.GuildID).SingleOrDefault()?.Channels.Remove(guildList.Where(x => x.ID == e.GuildID).SingleOrDefault()?.Channels.Where(x => x.ID == e.ID).SingleOrDefault());
            });
        }
        /// <summary>
        /// Task for managing CHANNEL_PINS_UPDATE gateway event before user actions
        /// </summary>
        /// <param name="e">Params of updated channel pin</param>
        /// <returns></returns>
        private static async Task InnerChannelPinsUpdatedTask(GatewayEventChannelPinsUpdateArgs e)
        {
            await Task.Run(()=> 
            {
                if (e.GuildID != null)
                {
                    var targetTimeStamp = guildList.Where(x => x.ID == e.GuildID).SingleOrDefault().Channels.Where(x => x.ID == e.ChannelID).SingleOrDefault().LastPinTimestamp;
                    targetTimeStamp = e.LastPinTimestamp;
                }
            });
        }
        /// <summary>
        /// Task for managing MESSAGE_REACTION_ADD gateway event before user actions
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static async Task InnerMessageReactionAddedTask(GatewayEventMessageReactionAddedArgs e)
        {
            await Task.Run(()=> { });
        }
        /// <summary>
        /// Task for managing MESSAGE_REACTION_REMOVE gateway event before user actions
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static async Task InnerMessageReactionRemovedTask(GatewayEventMessageReactionRemovedArgs e)
        {
            await Task.Run(() => { });
        }
        /// <summary>
        /// Task for managing MESSAGE_REACTION_REMOVE_ALL gateway event before user actions
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static async Task InnerMessageAllReactionRemovedTask(GatewayEventMessageAllReactionRemovedArgs e)
        {
            await Task.Run(() => { });
        }
        /// <summary>
        /// Task for managing MESSAGE_REACTION_REMOVE_EMOJI gateway event before user actions
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static async Task InnerMessageReactionEmojiRemovedTask(GatewayEventMessageReactionEmojiRemovedArgs e)
        {
            await Task.Run(() => { });
        }
        /// <summary>
        /// Task for managing PRESENCE_UPDATE gateway event before user actions
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static async Task InnerPresenceUpdatedTask(PresenceUpdate e)
        {
            await Task.Run(() => { });
        }
        /// <summary>
        /// Task for managing TYPING_START gateway event before user actions
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static async Task InnerTypingStartTask(GatewayEventTypingStartArgs e)
        {
            await Task.Run(() => { });
        }
        /// <summary>
        /// Task for managing USER_UPDATE gateway event before user actions
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static async Task InnerUserUpdatedTask(User e)
        {
            await Task.Run(() => 
            {
                foreach (var guild in guildList)
                {
                    foreach (var member in guild.Members)
                    {
                        if (member.User.ID == e.ID)
                        {
                            member.User = e;
                            return;
                        }
                    }
                }
            });
        }
        /// <summary>
        /// Task for managing VOICE_STATE_UPDATE gateway event before user actions
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static async Task InnerVoiceStateUpdatedTask(VoiceState e)
        {
            await Task.Run(() =>
            {

            });
        }
        /// <summary>
        /// Task for managing VOICE_SERVER_UPDATE gateway event before user actions
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static async Task InnerVoiceServerUpdatedTask(GatewayEventsVoiceServerUpdatedArgs e)
        {
            await Task.Run(() =>
            {

            });
        }
        /// <summary>
        /// Task for managing WEBHOOKS_UPDATE gateway event before user actions
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static async Task InnerWebhooksUpdatedTask(GatewayEventWebhookUpdatedArgs e)
        {
            await Task.Run(() =>
            {

            });
        }
        /// <summary>
        /// Task for managing GUILD_BAN_ADD gateway event before user actions
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static async Task InnerGuildBanAddedTask(GatewayEventGuildBanArgs e)
        {
            await Task.Run(() =>
            {

            });
        }
        /// <summary>
        /// Task for managing GUILD_BAN_REMOVE gateway event before user actions
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static async Task InnerGuildBanRemovedTask(GatewayEventGuildBanArgs e)
        {
            await Task.Run(() =>
            {

            });
        }
        /// <summary>
        /// Task for managing GUILD_EMOJIS_UPDATE gateway event before user actions
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static async Task InnerGuildEmojisUpdatedTask(GatewayEventGuildEmojisUpdateArgs e)
        {
            await Task.Run(() =>
            {

            });
        }
        /// <summary>
        /// Task for managing GUILD_INTEGRATIONS_UPDATE gateway event before user actions
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static async Task InnerGuildIntegrationsUpdatedTask(GatewayEventGuildIntegrationsUpdatedArgs e)
        {
            await Task.Run(() =>
            {

            });
        }
        /// <summary>
        /// Task for managing GUILD_MEMBER_ADD gateway event before user actions
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static async Task InnerGuildMemberAddedTask(GatewayEventGuildMemberArgs e)
        {
            await Task.Run(() =>
            {
                guildList.Where(x => x.ID == e.GuildID).SingleOrDefault()?.Members.Add(
                    new GuildMember() 
                    { 
                        IsDeaf = e.IsDeaf,
                        IsMuted = e.IsMuted,
                        JoinedAt = e.JoinedAt,
                        Nickname = e.Nickname,
                        PremiumSince = e.PremiumSince,
                        RolesIDs = e.RolesIDs,
                        User = e.User
                    });
            });
        }
        #endregion



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
