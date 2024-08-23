using TdLib;
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

        public ServiceStates ServiceState { get; protected set; }
        public AuthStates AuthorizationState { get; protected set; }
        public User? CurrentUser { get; protected set; }

        #region Events
        public event EventHandler<AuthStates> AuthStateChange;
        public event EventHandler<Exception> ErrorOccur;
        #endregion

        public TgSeekerService(IMessagesRepository messagesRepository, ISettingsRepository settingsRepository, ITgsServiceLogger? logger = null)
        {
            _messagesRepository = messagesRepository;
            _settingsRepository = settingsRepository;
            _logger = logger;
        }

        public async Task StartAsync()
        {
            if (ServiceState == ServiceStates.Running)
            {
                ErrorOccur?.Invoke(this, new Exception("Error: Attempt to start same instance twice."));
                return;
            }

            try
            {
                await InitParamsFromSettingsRepositoryAsync();
            }
            catch (Exception ex)
            {
                ErrorOccur?.Invoke(this, ex);
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

            return Task.CompletedTask;
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
                        AuthStateChange?.Invoke(this, AuthStates.AuthComplete);
                    }
                    else if (updateAuthState.AuthorizationState is AuthorizationState.AuthorizationStateWaitCode)
                    {
                        AuthorizationState = AuthStates.AuthCodeValidationPending;
                        AuthStateChange?.Invoke(this, AuthStates.AuthCodeValidationPending);
                    }
                    else if (updateAuthState.AuthorizationState is AuthorizationStateWaitPhoneNumber)
                    {
                        AuthorizationState = AuthStates.AuthRequired;
                        AuthStateChange?.Invoke(this, AuthStates.AuthRequired);
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
            }
            catch (Exception ex)
            {
                ErrorOccur?.Invoke(this, ex);
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

            if (updateNewMessage.Message.Content is MessageVoiceNote)
            {
                await new VoiceMessageEventHandler(options, _client, _messagesRepository).HandleCreateAsync(updateNewMessage.Message);
            }
            else if (updateNewMessage.Message.Content is MessageText) // text
            {
                await new TextMessageEventHandler(options, _client, _messagesRepository).HandleCreateAsync(updateNewMessage.Message);
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
                    await new VoiceMessageEventHandler(options, _client, _messagesRepository).HandleDeleteAsync(voiceMessage);
                }
                else if (message is TgsTextMessage textMessage)
                {
                    await new TextMessageEventHandler(options, _client, _messagesRepository).HandleDeleteAsync(textMessage);
                }
            }
        }

        #endregion

        #region Initialization
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

        public void Dispose()
        {
            _client?.Dispose();
        }

        public enum AuthStates
        {
            AuthRequired,
            AuthCodeValidationPending,
            AuthComplete
        }

        public enum ServiceStates
        {
            Idle,
            Running
        }
    }
}