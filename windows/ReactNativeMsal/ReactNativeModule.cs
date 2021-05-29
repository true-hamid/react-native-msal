using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Microsoft.ReactNative;
using Microsoft.ReactNative.Managed;
using ReactNativeMsal.Data;

namespace ReactNativeMsal
{
  [ReactModule("RNMSAL")]
  internal sealed class ReactNativeModule
  {
    private ReactContext _reactContext;

    [ReactInitializer]
    public void Initialize(ReactContext reactContext)
    {
      _reactContext = reactContext;
    }

    public MSALConfiguration mSALConfiguration = new MSALConfiguration();
    // The MSAL Public client app
    private static IPublicClientApplication PublicClientApp;
    private static AuthenticationResult authResult;

    [ReactMethod("createPublicClientApplication")]
    public void CreatePublicClientApplication(JSValue parameters)
    {
      mSALConfiguration.ClientId = parameters["clientId"].IsNull ? null : parameters["clientId"].AsString();
      mSALConfiguration.Authority = parameters["authority"].IsNull ? null : parameters["authority"].AsString();
      mSALConfiguration.RedirectUri = parameters["redirectUri"].IsNull ? null : parameters["redirectUri"].AsString();
      mSALConfiguration.KnownAuthorities = Helpers.JSValueToListofStrings(parameters, "knownAuthorities");
    }


    [ReactMethod("acquireToken")]
    public async void AcquireToken(JSValue parameters, IReactPromise<MSALResult> promise)
    {
      AquireToken aquireToken = new AquireToken(mSALConfiguration, PublicClientApp, authResult);
      MSALResult result = await aquireToken.SignIn(parameters, false);

      if (result.Error == true)
      {
        ReactError error = new ReactError();
        error.Message = result.ErrorMessage;
        promise.Reject(error);
      }
      else
      {
        promise.Resolve(result);
      }
    }

    [ReactMethod("acquireTokenSilent")]
    public async void AcquireTokenSilent(JSValue parameters, IReactPromise<MSALResult> promise)
    {
      AquireToken aquireToken = new AquireToken(mSALConfiguration, PublicClientApp, authResult);
      MSALResult result = await aquireToken.SignIn(parameters, true);

      if (result.Error == true)
      {
        ReactError error = new ReactError();
        error.Message = result.ErrorMessage;
        promise.Reject(error);
      }
      else
      {
        promise.Resolve(result);
      }
    }

    [ReactMethod("getAccounts")]
    public async void GetAccounts(IReactPromise<List<MSALAccount>> promise)
    {
      IEnumerable<IAccount> accounts = await PublicClientApp.GetAccountsAsync();

      var list = new List<MSALAccount>();
      if (accounts != null)
      {
        foreach (var account in accounts)
        {
          MSALAccount mSALAccount = new MSALAccount();

          mSALAccount.Environment = account.Environment;
          mSALAccount.TenantId = account.HomeAccountId.TenantId;
          mSALAccount.Identifier = account.HomeAccountId.Identifier;
          mSALAccount.Username = account.Username;

          list.Add(mSALAccount);
        }
        promise.Resolve(list);
      }
      ReactError error = new ReactError();
      error.Message = "No Accounts Found";
      promise.Reject(error);
    }

    [ReactMethod("getAccount")]
    public async void GetAccount(string accountIdentifier, IReactPromise<MSALAccount> promise)
    {
      IAccount account = await PublicClientApp.GetAccountAsync(accountIdentifier);

      if (account != null)
      {
        MSALAccount mSALAccount = new MSALAccount();

        mSALAccount.Environment = account.Environment;
        mSALAccount.TenantId = account.HomeAccountId.TenantId;
        mSALAccount.Identifier = account.HomeAccountId.Identifier;
        mSALAccount.Username = account.Username;

        promise.Resolve(mSALAccount);
      }
      else
      {
        ReactError error = new ReactError();
        error.Message = "No Account Found";
        promise.Reject(error);
      }
    }

    [ReactMethod("removeAccount")]
    public async void RemoveAccount(IAccount account, IReactPromise<bool> promise)
    {
      try
      {
        await PublicClientApp.RemoveAsync(account);

        promise.Resolve(true);
      }
      catch (Exception msalEx)
      {
        ReactError error = new ReactError();
        error.Message = "Error Signing out: " + msalEx;
        promise.Reject(error);
      }

    }

    [ReactMethod("signout")]
    public static async void Signout(IReactPromise<bool> promise)
    {
      try
      {
        IEnumerable<IAccount> accounts = await PublicClientApp.GetAccountsAsync().ConfigureAwait(false);
        IAccount firstAccount = accounts.FirstOrDefault();
        await PublicClientApp.RemoveAsync(firstAccount).ConfigureAwait(false);
        promise.Resolve(true);

      }
      catch (Exception msalEx)
      {
        ReactError error = new ReactError();
        error.Message = "Error Signing out: " + msalEx;
        promise.Reject(error);
      }
    }
  }
}
