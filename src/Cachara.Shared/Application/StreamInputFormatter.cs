using Microsoft.AspNetCore.Mvc.Formatters;

namespace Cachara.Shared.Application;

public class StreamInputFormatter : IInputFormatter
{
    public bool CanRead(InputFormatterContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var contentType = context.HttpContext.Request.ContentType;
        if (contentType == "application/octet-stream")
        {
            return true;
        }

        return false;
    }

    public async Task<InputFormatterResult> ReadAsync(InputFormatterContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var memoryStream = new MemoryStream();
        await context.HttpContext.Request.Body.CopyToAsync(memoryStream);

        return await InputFormatterResult.SuccessAsync(memoryStream);
    }
}
