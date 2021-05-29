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
    // The MSAL Public client app
    private static IPublicClientApplication mPublicClientApp;
    private static AuthenticationResult mAuthResult;


    public AquireToken(MSALConfiguration config, IPublicClientApplication PublicClientApp, AuthenticationResult AuthResult)
    {
      mConfig = config;
      mPublicClientApp = PublicClientApp;
      mAuthResult = AuthResult;
    }

    bool mSilent;
    public async Task<MSALResult> SignIn(JSValue parameters, bool silent)
    {
      try
      {
        string authority = parameters["authority"].AsString();
        List<string> scopes = Helpers.JSValueToListofStrings(parameters, "scopes");
        return await SignInUserAndGetTokenUsingMSAL(authority, scopes, silent);
      }
      catch (MsalException msalEx)
      {
        string title = "Error Acquiring Token: ";
        MSALResult error = new MSALResult();
        error.Error = true;
        error.ErrorMessage = title + msalEx;
        return error;
      }
      catch (Exception ex)
      {
        string title = "Unknown Error: ";
        MSALResult error = new MSALResult();
        error.Error = true;
        error.ErrorMessage = title + ex;
        return error;
      }
    }

    private async Task<MSALResult> SignInUserAndGetTokenUsingMSAL(string authority, List<string> scopes, bool silent)
    {
      // Initialize the MSAL library by building a public client application
      mPublicClientApp = PublicClientApplicationBuilder.Create(mConfig.ClientId)
          .WithAuthority(authority)
          .WithUseCorporateNetwork(false)
          .WithRedirectUri(mConfig.RedirectUri)
           .WithLogging((level, message, containsPii) =>
           {
             Debug.WriteLine($"MSAL: {level} {message} ");
           }, LogLevel.Warning, enablePiiLogging: false, enableDefaultPlatformLogging: true)
          .Build();

      IEnumerable<IAccount> accounts = await mPublicClientApp.GetAccountsAsync().ConfigureAwait(false);
      IAccount firstAccount = accounts.FirstOrDefault();

      if (silent)
      {
        try
        {
          mAuthResult = await mPublicClientApp.AcquireTokenSilent(scopes, firstAccount)
                                            .ExecuteAsync();
        }
        catch (MsalUiRequiredException ex)
        {
          // A MsalUiRequiredException happened on AcquireTokenSilentAsync. This indicates you need to call AcquireTokenAsync to acquire a token
          Debug.WriteLine($"MsalUiRequiredException: {ex.Message}");

          mAuthResult = await mPublicClientApp.AcquireTokenInteractive(scopes)
                                            .ExecuteAsync()
                                            .ConfigureAwait(false);
        }
      } else
      {
        mAuthResult = await mPublicClientApp.AcquireTokenInteractive(scopes)
                                           .ExecuteAsync()
                                           .ConfigureAwait(false);
      }


      // Setup account object
      MSALAccount account = new MSALAccount();
      account.Environment = mAuthResult.Account.Environment;
      account.Identifier = mAuthResult.Account.HomeAccountId.Identifier;
      account.TenantId = mAuthResult.Account.HomeAccountId.TenantId;
      account.Username = mAuthResult.Account.Username;

      // Setup Response object
      MSALResult result = new MSALResult();
      result.AccessToken = mAuthResult.AccessToken;
      result.Account = account;
      result.ExpiresOn = mAuthResult.ExpiresOn;
      result.IdToken = mAuthResult.IdToken;
      result.Scopes = mAuthResult.Scopes;
      result.TenantId = mAuthResult.TenantId;
      result.ErrorMessage = "";

      return result;
    }

  }
}
