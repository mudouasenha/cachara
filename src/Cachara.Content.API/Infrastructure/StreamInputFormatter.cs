using Microsoft.AspNetCore.Mvc.Formatters;

namespace Cachara.Content.API.Infrastructure
{
    public class StreamInputFormatter : IInputFormatter
    {
        public bool CanRead(InputFormatterContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            var contentType = context.HttpContext.Request.ContentType;
            if (contentType == "application/octet-stream")
            {
                return true;
            }
            return false;
        }

        public async Task<InputFormatterResult> ReadAsync(InputFormatterContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var memoryStream = new MemoryStream();
            await context.HttpContext.Request.Body.CopyToAsync(memoryStream);

            return await InputFormatterResult.SuccessAsync(memoryStream);
        }
    }
}
