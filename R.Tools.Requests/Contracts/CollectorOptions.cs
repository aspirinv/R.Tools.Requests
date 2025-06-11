using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace R.Tools.Requests.Contracts
{
    public class CollectorOptions
    {
        public string ServiceName { get; set; } = "Primus";
        public Func<PathString, bool> ShouldCollect { get; set; } = x => true;
        public  Func<ClaimsPrincipal, string?> DefineUserId { get; set; } = p => p.FindFirstValue(ClaimTypes.NameIdentifier);

    }
}
