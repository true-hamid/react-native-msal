using System.Collections.Generic;

namespace ReactNativeMsal.Data
{
  class MSALResult
  {
    public string accessToken { get; set; }
    public MSALAccount account { get; set; }
    public string expiresOn { get; set; }
    public string idToken { get; set; }
    public IEnumerable<string> scopes { get; set; }
    public string tenantId { get; set; }
    public bool error { get; set; }
    public string errorMessage { get; set; }
  }
}
