using Microsoft.ReactNative.Managed;
using System.Collections.Generic;
using System.Linq;

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
  }
}
