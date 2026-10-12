using System.Text.RegularExpressions;

namespace ECommerce.WebUI.Extensions
{
    public static class StringExtensions
    {
        public static string ShortcodeToHtml(this string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            // Köşeli parantez içindeki [ Metin → Link ] formatını yakalayan düzenli ifade (Regex)
            var pattern = @"\[\s*(.*?)\s*→\s*(.*?)\s*\]";

            var result = Regex.Replace(input, pattern, match =>
            {
                var btnText = match.Groups[1].Value;
                var btnUrl = match.Groups[2].Value;

                // Dönüşecek şık ve kurumsal buton tasarımı
                return $"<a href=\"{btnUrl}\" class=\"btn btn-lg px-4 py-2 rounded-pill my-3 shadow-sm\" style=\"background: linear-gradient(135deg,#8b0028,#a70b3b); border:none; color:#fff; font-weight:600; text-decoration:none; display:inline-flex; align-items:center; gap:8px;\">{btnText} <i class=\"bi bi-arrow-right\"></i></a>";
            });

            return result;
        }
    }
}