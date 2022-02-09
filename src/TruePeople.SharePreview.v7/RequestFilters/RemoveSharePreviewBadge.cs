using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace TruePeople.SharePreview.RequestFilters
{
    public class RemoveSharePreviewBadge : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.IsChildAction || !filterContext.HttpContext.Request.Url.AbsolutePath.Contains("umbraco/sharepreview/index/"))
                return;

            var originalFilter = filterContext.HttpContext.Response.Filter;
            filterContext.HttpContext.Response.Filter = new PreviewUrlFilter(originalFilter);
            base.OnActionExecuting(filterContext);
        }
    }

    public class PreviewUrlFilter : MemoryStream
    {
        private readonly StringBuilder _data = new StringBuilder();
        private readonly Stream responseStream;

        public PreviewUrlFilter(Stream stream)
        {
            responseStream = stream;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _data.Append(Encoding.UTF8.GetString(buffer, offset, count));
        }

        public override void Close()
        {
            var html = _data.ToString();
            var regex = @"(?s)<a[^>]*id=""umbracoPreviewBadge"".*<\/a>";
            html = Regex.Replace(html, regex, "");

            var output = Encoding.UTF8.GetBytes(html);
            responseStream.Write(output, 0, output.Length);
            responseStream.Flush();
            _data.Clear();
        }
    }
}
