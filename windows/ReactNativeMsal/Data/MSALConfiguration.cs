using System.Collections.Generic;

namespace ReactNativeMsal.Data
{
  class MSALConfiguration
  {
    public string ClientId { get; set; }
    public string Authority { get; set; }
    public List<string> KnownAuthorities { get; set; }
    public string RedirectUri { get; set; }
  }
}
