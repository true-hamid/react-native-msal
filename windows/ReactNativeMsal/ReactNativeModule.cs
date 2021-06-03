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

    public MSALConfiguration mSALConfiguration = new MSALConfiguration();
    // The MSAL Public client app
    public MSALApplication MsalApplication;

    [ReactMethod("createPublicClientApplication")]
    public void CreatePublicClientApplication(JSValue parameters)
    {
      mSALConfiguration.ClientId = parameters["clientId"].IsNull ? null : parameters["clientId"].AsString();
      mSALConfiguration.Authority = parameters["authority"].IsNull ? null : parameters["authority"].AsString();
      mSALConfiguration.RedirectUri = parameters["redirectUri"].IsNull ? null : parameters["redirectUri"].AsString();
      mSALConfiguration.KnownAuthorities = Helpers.JSValueToListofStrings(parameters, "knownAuthorities");

      MsalApplication = new MSALApplication();

    }


    [ReactMethod("acquireToken")]
    public async void AcquireToken(JSValue parameters, IReactPromise<MSALResult> promise)
    {
      AquireToken aquireToken = new AquireToken(mSALConfiguration);
      MSALResult result = await aquireToken.SignIn(parameters, false, MsalApplication);

      if (result.error == true)
      {
        ReactError error = new ReactError();
        error.Message = result.errorMessage;
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
      AquireToken aquireToken = new AquireToken(mSALConfiguration);
      MSALResult result = await aquireToken.SignIn(parameters, true, MsalApplication);

      if (result.error == true)
      {
        ReactError error = new ReactError();
        error.Message = result.errorMessage;
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
      IEnumerable<IAccount> accounts = await MsalApplication.PublicClientApp.GetAccountsAsync();

      var list = new List<MSALAccount>();
      if (accounts != null)
      {
        foreach (var account in accounts)
        {
          MSALAccount mSALAccount = new MSALAccount();

          mSALAccount.environment = account.Environment;
          mSALAccount.tenantId = account.HomeAccountId.TenantId;
          mSALAccount.identifier = account.HomeAccountId.Identifier;
          mSALAccount.username = account.Username;

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

      IAccount account = await Helpers.GetMSALAccount(accountIdentifier, MsalApplication.PublicClientApp);

      if (account != null)
      {
        MSALAccount mSALAccount = new MSALAccount();

        mSALAccount.environment = account.Environment;
        mSALAccount.tenantId = account.HomeAccountId.TenantId;
        mSALAccount.identifier = account.HomeAccountId.Identifier;
        mSALAccount.username = account.Username;

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
    public async void RemoveAccount(string identifier, IReactPromise<bool> promise)
    {
      try
      {
        //string identifier = account["account"].IsNull ? null : account["account"].AsString();

        //JSValue maccount = account["account"].IsNull ? "" : account.AsObject()["account"];
        //string identifier = maccount["identifier"].AsString();
        IAccount msalAccount = await Helpers.GetMSALAccount(identifier, MsalApplication.PublicClientApp);

        await MsalApplication.PublicClientApp.RemoveAsync(msalAccount).ConfigureAwait(false);

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
