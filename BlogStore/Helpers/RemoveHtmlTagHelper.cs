using System.Net;
using System.Text.RegularExpressions;
namespace BlogStore.Helpers
{
    public static class RemoveHtmlTagHelper
    {
        public static string RemoveHtmlTags(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            var noHtml = Regex.Replace(input, "<.*?>", string.Empty);

            return WebUtility.HtmlDecode(noHtml);
        }
    }
}
