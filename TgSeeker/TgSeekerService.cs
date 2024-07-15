using TdLib;
using TdLib.Bindings;
using TgSeeker.EventHandlers;
using TgSeeker.EventHandlers.Util;
using TgSeeker.Persistent.Entities;
using TgSeeker.Persistent.Repositiories;
using static TdLib.TdApi;
using static TdLib.TdApi.AuthorizationState;

namespace TgSeeker
{
    public class TgSeekerService : IDisposable
    {
        private TdClient? _client;
        private readonly IMessagesRepository _messagesRepository;
        private readonly ISettingsRepository _settingsRepository;
        private int _apiId;
        private string _apiHash;

        public ServiceStates ServiceState { get; protected set; }
        public AuthStates AuthorizationState { get; protected set; }
        public User? CurrentUser { get; protected set; }

        #region Events
        public event EventHandler<AuthStates> AuthStateChange;
        public event EventHandler<Exception> ErrorOccur;
        #endregion

        public TgSeekerService(IMessagesRepository messagesRepository, ISettingsRepository settingsRepository)
        {
            _messagesRepository = messagesRepository;
            _settingsRepository = settingsRepository;
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

            return Task.CompletedTask;
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
        }

        private async void HandleUpdate(object? sender, TdApi.Update e)
        {
            // Important: do not await any calls to client here, doing so will result in deadlock

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
                    var handler = new NewMessageEventHandler(new TgsEventHandlerOptions { CurrentUser = CurrentUser }, _client, _messagesRepository);
                    await handler.HandleAsync(updateNewMessage);
                }
                else if (e is TdApi.Update.UpdateDeleteMessages updateDeleteMessages)
                {
                    var handler = new MessagesDeletedEventHandler(new TgsEventHandlerOptions { CurrentUser = CurrentUser }, _client, _messagesRepository);
                    await handler.HandleAsync(updateDeleteMessages);
                }
            }
            catch (Exception ex)
            {
                ErrorOccur?.Invoke(this, ex);
            }
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

            }
        }

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