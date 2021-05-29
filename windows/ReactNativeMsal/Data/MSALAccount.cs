using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactNativeMsal.Data
{
  class MSALAccount
  {
    public string Identifier { get; set; }
    public string Environment { get; set; }
    public string TenantId { get; set; }
    public string Username { get; set; }
  }
}
