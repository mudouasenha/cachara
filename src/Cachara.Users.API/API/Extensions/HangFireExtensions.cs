using Cachara.Users.API.Options;
using Hangfire;
using Hangfire.Console;
using Hangfire.SqlServer;

namespace Cachara.API.Extensions
{
    public static class HangFireExtensions
    {
        public static CacharaOptions Options { get; set; }

        
    }
}
