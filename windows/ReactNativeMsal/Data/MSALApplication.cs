using Microsoft.Identity.Client;

namespace ReactNativeMsal.Data
{
  class MSALApplication
  {
    public IPublicClientApplication PublicClientApp { get; set; }
    public AuthenticationResult AuthResult { get; set; }
  }
}
