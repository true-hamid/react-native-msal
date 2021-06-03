using Microsoft.Identity.Client;
using Microsoft.ReactNative.Managed;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReactNativeMsal
{
  class Helpers
  {
    public static List<string> JSValueToListofStrings(JSValue jsValue, string key)
    {
      var list = new List<string>();
      if (!jsValue.IsNull && key != null)
      {
        if (!jsValue[key].IsNull)
        {
          var array = jsValue.AsObject()[key].AsArray();
          foreach (var value in array)
          {
            list.Append(value.AsString());
          }
        }
      }
      return list;
    }

    public static async Task<IAccount> GetMSALAccount(string indentifier, IPublicClientApplication PublicClientApp)
    {
      return await PublicClientApp.GetAccountAsync(indentifier);
    }
  }
}
