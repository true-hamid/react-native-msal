using Microsoft.Identity.Client;
using Microsoft.ReactNative.Managed;
using ReactNativeMsal.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ReactNativeMsal
{
  class AquireToken
  {
    MSALConfiguration mConfig;


    public AquireToken(MSALConfiguration config)
    {
      mConfig = config;
    }

    public async Task<MSALResult> SignIn(JSValue parameters, bool silent, MSALApplication MsalApplication)
    {
      try
      {
        string authority = parameters["authority"].AsString();
        List<string> scopes = Helpers.JSValueToListofStrings(parameters, "scopes");
        return await SignInUserAndGetTokenUsingMSAL(authority, scopes, silent, MsalApplication);
      }
      catch (MsalException msalEx)
      {
        string title = "Error Acquiring Token: ";
        MSALResult error = new MSALResult();
        error.error = true;
        error.errorMessage = title + msalEx;
        return error;
      }
      catch (Exception ex)
      {
        string title = "Unknown Error: ";
        MSALResult error = new MSALResult();
        error.error = true;
        error.errorMessage = title + ex;
        return error;
      }
    }

    private async Task<MSALResult> SignInUserAndGetTokenUsingMSAL(string authority, List<string> scopes, bool silent, MSALApplication MsalApplication)
    {
      // Initialize the MSAL library by building a public client MsalApplication
      MsalApplication.PublicClientApp = PublicClientApplicationBuilder.Create(mConfig.ClientId)
          .WithAuthority(authority)
          .WithUseCorporateNetwork(false)
          .WithRedirectUri(mConfig.RedirectUri)
           .WithLogging((level, message, containsPii) =>
           {
             Debug.WriteLine($"MSAL: {level} {message} ");
           }, LogLevel.Warning, enablePiiLogging: false, enableDefaultPlatformLogging: true)
          .Build();

      IEnumerable<IAccount> accounts = await MsalApplication.PublicClientApp.GetAccountsAsync().ConfigureAwait(false);
      IAccount firstAccount = accounts.FirstOrDefault();

      if (silent)
      {
        try
        {
          MsalApplication.AuthResult = await MsalApplication.PublicClientApp.AcquireTokenSilent(scopes, firstAccount)
                                            .ExecuteAsync();
        }
        catch (MsalUiRequiredException ex)
        {
          // A MsalUiRequiredException happened on AcquireTokenSilentAsync. This indicates you need to call AcquireTokenAsync to acquire a token
          Debug.WriteLine($"MsalUiRequiredException: {ex.Message}");

          MsalApplication.AuthResult = await MsalApplication.PublicClientApp.AcquireTokenInteractive(scopes)
                                            .ExecuteAsync()
                                            .ConfigureAwait(false);
        }
      }
      else
      {
        MsalApplication.AuthResult = await MsalApplication.PublicClientApp.AcquireTokenInteractive(scopes)
                                           .ExecuteAsync()
                                           .ConfigureAwait(false);
      }


      // Setup account object
      MSALAccount account = new MSALAccount();
      account.environment = MsalApplication.AuthResult.Account.Environment;
      account.identifier = MsalApplication.AuthResult.Account.HomeAccountId.Identifier;
      account.tenantId = MsalApplication.AuthResult.Account.HomeAccountId.TenantId;
      account.username = MsalApplication.AuthResult.Account.Username;

      // Setup Response object
      MSALResult result = new MSALResult();
      result.accessToken = MsalApplication.AuthResult.AccessToken;
      result.account = account;
      result.expiresOn = MsalApplication.AuthResult.ExpiresOn.ToString();
      result.idToken = MsalApplication.AuthResult.IdToken;
      result.scopes = MsalApplication.AuthResult.Scopes;
      result.tenantId = MsalApplication.AuthResult.TenantId;
      result.errorMessage = "";

      return result;
    }

  }
}
