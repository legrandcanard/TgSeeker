﻿using TdLib;
using TgSeeker.EventHandlers.Messages;
using TgSeeker.Util;
using TgSeeker.Persistent.Entities;
using TgSeeker.Persistent.Repositiories;
using static TdLib.TdApi;
using static TdLib.TdApi.AuthorizationState;
using static TdLib.TdApi.MessageContent;
using static TdLib.TdApi.Update;

namespace TgSeeker
{
    public class TgSeekerService : IDisposable
    {
        private TdClient? _client;
        private readonly IMessagesRepository _messagesRepository;
        private readonly ISettingsRepository _settingsRepository;
        private readonly ITgsServiceLogger? _logger;
        private int _apiId;
        private string _apiHash;
        private bool _isServiceReady = false;
        private Dictionary<long, (TgsMessage cachedSourceMessage, TdApi.Message pendingMessage)> _pendingMessages = new Dictionary<long, (TgsMessage cachedSourceMessage, TdApi.Message pendingMessage)>();

        public ServiceStates ServiceState { get; protected set; }
        public AuthStates AuthorizationState { get; protected set; }
        public User? CurrentUser { get; protected set; }

        public TgSeekerService(IMessagesRepository messagesRepository, ISettingsRepository settingsRepository, ITgsServiceLogger? logger = null)
        {
            _messagesRepository = messagesRepository;
            _settingsRepository = settingsRepository;
            _logger = logger;
        }

        private async void HandleUpdate(object? sender, TdApi.Update e)
        {
            // Important: do not await any calls to client here, that will result in HandleUpdate circle call. Doing so will result in deadlock

            try
            {
                if (e is TdApi.Update.UpdateAuthorizationState updateAuthState)
                {
                    if (updateAuthState.AuthorizationState is AuthorizationState.AuthorizationStateWaitTdlibParameters)
                    {
                        await SetTdlibParamsAsync();
                    }
                    else if (updateAuthState.AuthorizationState is AuthorizationState.AuthorizationStateReady)
                    {
                        CurrentUser = await _client.GetMeAsync();
                        ServiceState = ServiceStates.Running;
                        AuthorizationState = AuthStates.AuthComplete;
                    }
                    else if (updateAuthState.AuthorizationState is AuthorizationState.AuthorizationStateWaitCode)
                    {
                        AuthorizationState = AuthStates.AuthCodeValidationPending;
                    }
                    else if (updateAuthState.AuthorizationState is AuthorizationStateWaitPhoneNumber)
                    {
                        AuthorizationState = AuthStates.AuthRequired;
                    }
                    else if (updateAuthState.AuthorizationState is AuthorizationStateClosed)
                    {
                        // Restart instance
                        Task.Run(async delegate
                        {
                            await StopAsync();
                            await StartAsync();
                        });
                    }
                }
                else if (e is TdApi.Update.UpdateNewMessage updateNewMessage)
                {
                    await HandleNewMessageAsync(updateNewMessage);
                }
                else if (e is TdApi.Update.UpdateDeleteMessages updateDeleteMessages)
                {
                    await HandleDeleteMessagesAsync(updateDeleteMessages);
                }
                else if (e is TdApi.Update.UpdateMessageSendSucceeded updateMessageSendSucceeded)
                {
                    await HandleMessageSendSucceededAsync(updateMessageSendSucceeded);
                }
                else if (e is TdApi.Update.UpdateMessageSendFailed updateMessageSendFailed)
                {
                    await HandleMessageSendFailedAsync(updateMessageSendFailed);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.Message);
            }
        }

        #region Messages

        public async Task HandleNewMessageAsync(UpdateNewMessage updateNewMessage)
        {
            if (CurrentUser == null)
                throw new ArgumentException("CurrentUser is not set");

            var message = updateNewMessage.Message;

            if (CurrentUser.Id == message.ChatId)
                return;

            if (message.IsChannelPost || message.IsTopicMessage || message.ChatId < 0)
                return;

            var options = new TgsEventHandlerOptions { CurrentUser = CurrentUser };

            await EnsureServiceReadyAsync();

            if (message.Content is MessageVoiceNote)
            {
                await new VoiceMessageEventHandler(options, _client, _messagesRepository).HandleCreateAsync(message);
            }
            else if (message.Content is MessageText)
            {
                await new TextMessageEventHandler(options, _client, _messagesRepository).HandleCreateAsync(message);
            }
            else if (message.Content is MessageVideoNote)
            {
                await new VideoNoteMessageEventHandler(options, _client, _messagesRepository).HandleCreateAsync(message);
            }
            else
            {
                //todo: implement other message types
            }
        }

        public async Task HandleDeleteMessagesAsync(UpdateDeleteMessages updateDeleteMessages)
        {
            if (CurrentUser == null)
                throw new ArgumentException("CurrentUser is not set");

            if (updateDeleteMessages.FromCache)
                return;

            if (updateDeleteMessages.ChatId == CurrentUser.Id)
                return;

            var options = new TgsEventHandlerOptions { CurrentUser = CurrentUser };
            
            var messages = await _messagesRepository.GetMessagesAsync(updateDeleteMessages.ChatId, updateDeleteMessages.MessageIds);
            foreach (var messageId in updateDeleteMessages.MessageIds)
            {
                var message = messages.FirstOrDefault(m => m.Id == messageId);
                if (message == null)
                {
                    continue;
                }

                if (message is TgsVoiceMessage voiceMessage)
                {
                    var pendingMessage = await new VoiceMessageEventHandler(options, _client, _messagesRepository).HandleDeleteAsync(voiceMessage);
                    _pendingMessages.Add(pendingMessage.Id, (voiceMessage, pendingMessage));
                }
                else if (message is TgsTextMessage textMessage)
                {
                    await new TextMessageEventHandler(options, _client, _messagesRepository).HandleDeleteAsync(textMessage);
                }
                else if (message is TgsVideoNoteMessage videoNoteMessage)
                {
                    var pendingMessage = await new VideoNoteMessageEventHandler(options, _client, _messagesRepository).HandleDeleteAsync(videoNoteMessage);
                    _pendingMessages.Add(pendingMessage.Id, (videoNoteMessage, pendingMessage));
                }
            }
        }

        public async Task HandleMessageSendSucceededAsync(UpdateMessageSendSucceeded updateMessageSendSucceeded)
        {
            var options = new TgsEventHandlerOptions { CurrentUser = CurrentUser };

            (TgsMessage cachedSourceMessage, TdApi.Message pendingMessage) = _pendingMessages[updateMessageSendSucceeded.OldMessageId];
            try
            {
                if (cachedSourceMessage is TgsVideoNoteMessage cachedVideoNoteMessage)
                {
                    var handler = new VideoNoteMessageEventHandler(options, _client, _messagesRepository);
                    await handler.HandleMessageCopySentCompleteAsync(updateMessageSendSucceeded.Message, cachedVideoNoteMessage);
                }
                else if (cachedSourceMessage is TgsVoiceMessage cachedVoiceMessage)
                {
                    var handler = new VoiceMessageEventHandler(options, _client, _messagesRepository);
                    await handler.HandleMessageCopySentCompleteAsync(updateMessageSendSucceeded.Message, cachedVoiceMessage);
                }
            }
            catch (Exception e)
            {
                _logger?.LogError("Failed to purche cache for sended message.");
            }
            finally
            {
                _pendingMessages.Remove(updateMessageSendSucceeded.OldMessageId);
            }
        }

        public Task HandleMessageSendFailedAsync(UpdateMessageSendFailed updateMessageSendFailed)
        {
            _logger?.LogError("Failed to send message.");
            _pendingMessages.Remove(updateMessageSendFailed.OldMessageId);
            return Task.CompletedTask;
        }

        #endregion

        #region Auth
        public async Task SendAuthenticationCodeToPhone(string phoneNumber)
        {
            await _client.SetAuthenticationPhoneNumberAsync(phoneNumber);
        }

        public async Task CheckAuthenticationCodeAsync(string code)
        {
            await _client.CheckAuthenticationCodeAsync(code);
        }

        public async Task LogOut()
        {
            await _client.LogOutAsync();
            CurrentUser = null;
            AuthorizationState = AuthStates.AuthRequired;
        }
        #endregion

        #region Life cycle
        public async Task StartAsync()
        {
            if (ServiceState == ServiceStates.Running)
            {
                _logger?.LogInfo("Attempt to start same instance twice.");
                return;
            }

            try
            {
                await InitParamsFromSettingsRepositoryAsync();
            }
            catch (Exception ex)
            {
                _logger?.LogError("Faield to init client parameters from settings.");
                ServiceState = ServiceStates.BadConfiguration;
                return;
            }

            _client = new TdClient();
            _client.UpdateReceived += HandleUpdate;
            ServiceState = ServiceStates.Running;
        }

        public Task StopAsync()
        {
            if (_client == null)
                return Task.CompletedTask;

            _client.UpdateReceived -= HandleUpdate;
            _client.Dispose();
            _client = null;
            ServiceState = ServiceStates.Idle;
            _isServiceReady = false;
            _pendingMessages.Clear();

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _client?.Dispose();
        }

        private async Task InitParamsFromSettingsRepositoryAsync()
        {
            var settings = await _settingsRepository.GetSettingsAsync();

            if (!settings.TryGetValue("ApiId", out string? strApiId))
                throw new Exception("Error settings up parameters: api_id is required.");

            try
            {
                _apiId = Convert.ToInt32(strApiId);
            }
            catch (Exception ex)
            {
                throw new Exception("Error settings up parameters: api_id is invalid.", ex);
            }

            if (!settings.TryGetValue("ApiHash", out _apiHash))
                throw new Exception("Error settings up parameters: api_hash is required.");

            if (string.IsNullOrEmpty(_apiHash))
                throw new Exception("Error settings up parameters: api_hash is invalid.");

            _apiHash = _apiHash.Trim();
        }

        private async Task EnsureServiceReadyAsync()
        {
            if (_isServiceReady)
                return;

            // This will cache this chat for the client
            await _client.LoadChatsAsync(limit: int.MaxValue);

            _isServiceReady = true;
        }

        private async Task SetTdlibParamsAsync()
        {
            try
            {
                await _client.SetTdlibParametersAsync(
                    databaseDirectory: "tdlib",
                    useMessageDatabase: false,
                    apiId: _apiId,
                    apiHash: _apiHash,
                    systemLanguageCode: "en",
                    deviceModel: "Desktop",
                    applicationVersion: "1.0"
                );
            }
            catch (Exception e)
            {
                _logger?.LogError("Failed to set tdlib params. Error: " + e.Message);
            }
        }
        #endregion

        public enum AuthStates
        {
            AuthRequired,
            AuthCodeValidationPending,
            AuthComplete
        }

        public enum ServiceStates
        {
            Idle,
            Running,
            BadConfiguration   
        }

    }
}