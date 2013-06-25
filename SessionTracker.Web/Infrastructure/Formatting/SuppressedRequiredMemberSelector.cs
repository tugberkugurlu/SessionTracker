using System.Net.Http.Formatting;
using System.Reflection;

namespace SessionTracker.Web.Infrastructure.Formatting
{
    public class SuppressedRequiredMemberSelector : IRequiredMemberSelector
    {
        public bool IsRequiredMember(MemberInfo member)
        {
            return false;
        }
    }
}