using System.Reflection;
using TdLib;
using TgSeeker.Cli;
using TgSeeker.Cli.Locale;
using TgSeeker.Persistent;
using TgSeeker.Persistent.Contexts;
using TgSeeker.Persistent.Repositiories;

namespace TgSeeker
{
    // https://core.telegram.org/api/obtaining_api_id
    // https://my.telegram.org/apps
    internal class TgSeeker
    {
        private readonly static TgSeekerService _service = new TgSeekerService(new MessagesRepository());

        static async Task Main(string[] args)
        {
            Console.WriteLine($"TgSeeker {Assembly.GetExecutingAssembly().GetName().Version}");
            Console.WriteLine($"Bootstrapping...");
            try
            {
                await DbInitializer.InitializeAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine($"Complete.");

            _service.AuthStateChange += OnAuthStateChange;
            Console.CancelKeyPress += Console_CancelKeyPress;

            await _service.StartAsync();
        }

        private static void OnAuthStateChange(object? sender, TgSeekerService.AuthStates e)
        {
            //https://core.telegram.org/api/auth

            Task.Run(async () =>
            {
            BeginAuthorization:

                switch (e)
                {
                    case TgSeekerService.AuthStates.AuthRequired:

                    SetPhoneNumber:
                        string phoneNumber = GetPhoneNumber();
                    SendCode:
                        try
                        {
                            await _service.SendAuthenticationCodeToPhone(phoneNumber);
                        }
                        catch (TdException ex) when (ex.Message == "API_ID_INVALID")
                        {
                            WriteTextOutput(Messages.API_ID_INVALID, Messages.Auth_State_Required);
                            Exit();
                        }
                        catch (TdException ex) when (ex.Message == "API_ID_PUBLISHED_FLOOD")
                        {
                            WriteTextOutput(Messages.API_ID_PUBLISHED_FLOOD, Messages.Auth_State_Required);
                            Exit();
                        }
                        catch (TdException ex) when (ex.Message == "AUTH_RESTART")
                        {
                            WriteTextOutput(Messages.PHONE_NUMBER_FLOOD, Messages.Auth_State_Required);
                            goto SetPhoneNumber;
                        }
                        catch (TdException ex) when (ex.Message == "PHONE_NUMBER_APP_SIGNUP_FORBIDDEN")
                        {
                            WriteTextOutput(Messages.PHONE_NUMBER_APP_SIGNUP_FORBIDDEN, Messages.Auth_State_Required);
                            Exit();
                        }
                        catch (TdException ex) when (ex.Message == "PHONE_NUMBER_BANNED")
                        {
                            WriteTextOutput(Messages.PHONE_NUMBER_BANNED, Messages.Auth_State_Required);
                            goto SetPhoneNumber;
                        }
                        catch (TdException ex) when (ex.Message == "PHONE_NUMBER_FLOOD")
                        {
                            WriteTextOutput(Messages.PHONE_NUMBER_FLOOD, Messages.Auth_State_Required);
                            goto SetPhoneNumber;
                        }
                        catch (TdException ex) when (ex.Message == "PHONE_NUMBER_INVALID")
                        {
                            WriteTextOutput(Messages.PHONE_NUMBER_INVALID, Messages.Auth_State_Required);
                            goto SetPhoneNumber;
                        }
                        catch (TdException ex) when (ex.Message == "PHONE_PASSWORD_FLOOD")
                        {
                            WriteTextOutput(Messages.FeatureNotSupported);
                            WriteTextOutput(Messages.PHONE_PASSWORD_FLOOD, Messages.Auth_State_Required);
                            Exit();
                        }
                        catch (TdException ex) when (ex.Message == "PHONE_PASSWORD_PROTECTED")
                        {
                            WriteTextOutput(Messages.FeatureNotSupported);
                            WriteTextOutput(Messages.Auth_State_Required, Messages.Auth_State_Required);
                            Exit();
                        }
                        catch (TdException ex) when (ex.Message == "SMS_CODE_CREATE_FAILED")
                        {
                            WriteTextOutput(Messages.FeatureNotSupported);
                            WriteTextOutput(Messages.SMS_CODE_CREATE_FAILED, Messages.Auth_State_Required);
                            Exit();
                        }
                        catch (TdException ex) when (ex.Message.StartsWith("Too Many Requests"))
                        {
                            WriteTextOutput(ex.Message, Messages.Auth_State_Required);
                            goto SetPhoneNumber;
                        }
                        catch (Exception ex)
                        {
                            WriteTextOutput(ex.Message);
                            Exit();
                        }
                    case TgSeekerService.AuthStates.AuthCodeValidationPending:
                    SignIn:
                        try
                        {
                            string code = GetAuthCode();
                            await _service.CheckAuthenticationCodeAsync(code);
                        }
                        catch (TdException ex) when (ex.Message == "AUTH_RESTART")
                        {
                            WriteTextOutput(Messages.AUTH_RESTART);
                            e = TgSeekerService.AuthStates.AuthRequired;
                            goto BeginAuthorization;
                        }
                        catch (TdException ex) when (ex.Message == "PHONE_CODE_EMPTY")
                        {
                            WriteTextOutput(Messages.PHONE_CODE_EMPTY);
                            goto SignIn;
                        }
                        catch (TdException ex) when (ex.Message == "PHONE_CODE_EXPIRED")
                        {
                            WriteTextOutput(Messages.PHONE_CODE_EXPIRED);
                            goto SendCode;
                        }
                        catch (TdException ex) when (ex.Message == "PHONE_CODE_INVALID")
                        {
                            WriteTextOutput(Messages.PHONE_CODE_INVALID);
                            goto SendCode;
                        }
                        catch (TdException ex) when (ex.Message == "PHONE_NUMBER_INVALID")
                        {
                            WriteTextOutput(Messages.PHONE_NUMBER_INVALID);
                            e = TgSeekerService.AuthStates.AuthRequired;
                            goto BeginAuthorization;
                        }
                        catch (TdException ex) when (ex.Message == "PHONE_NUMBER_UNOCCUPIED")
                        {
                            WriteTextOutput(Messages.PHONE_NUMBER_UNOCCUPIED);
                            e = TgSeekerService.AuthStates.AuthRequired;
                            goto BeginAuthorization;
                        }
                        catch (TdException ex) when (ex.Message == "SIGN_IN_FAILED")
                        {
                            WriteTextOutput(Messages.SIGN_IN_FAILED);
                            e = TgSeekerService.AuthStates.AuthRequired;
                            goto BeginAuthorization;
                        }

                        WriteTextOutput(Messages.Auth_State_Success, Messages.Auth_State_Required);

                        break;
                    case TgSeekerService.AuthStates.AuthReady:
                        WriteTextOutput(Messages.App_State_Running);
                        break;
                }
            }).GetAwaiter().GetResult();
        }

        private static void Console_CancelKeyPress(object? sender, ConsoleCancelEventArgs args)
        {
            args.Cancel = true;
            _service?.Dispose();
        }

        public static string GetTextInput(string label, string? context = null)
        {
            string? text;
            do
            {
                Console.Write(context == null ? $"{label}: " : $"[{context}] {label}: ");
                text = Console.ReadLine();
            }
            while (string.IsNullOrWhiteSpace(text));
            return text;
        }

        public static void WriteTextOutput(string text, string? context = null)
            => Console.WriteLine(FormatContextMessage(text, context));

        public static string FormatContextMessage(string message, string? context = null)
            => string.IsNullOrWhiteSpace(context) ? message : $"[{context}] {message}";

        public static string GetPhoneNumber()
            => GetTextInput(Messages.Auth_State_EnterPhoneNumber, Messages.Auth_State_Required);

        public static string GetAuthCode()
            => GetTextInput(Messages.Auth_State_WaitForCode, Messages.Auth_State_Required);

        static void Exit()
        {
            Environment.Exit(0);
        }
    }
}
