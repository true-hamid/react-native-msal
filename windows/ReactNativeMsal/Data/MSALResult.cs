using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactNativeMsal.Data
{
  class MSALResult
  {
    public string AccessToken { get; set; }
    public MSALAccount Account { get; set; }
    public DateTimeOffset ExpiresOn { get; set; }
    public string IdToken { get; set; }
    public IEnumerable<string> Scopes { get; set; }
    public string TenantId { get; set; }
    public bool Error { get; set; }
    public string ErrorMessage { get; set; }
  }
}
