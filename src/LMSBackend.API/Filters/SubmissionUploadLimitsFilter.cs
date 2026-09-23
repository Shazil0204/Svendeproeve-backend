using LMSBackend.Application.Abstractions.Submissions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LMSBackend.API.Filters;

public sealed class SubmissionUploadLimitsFilter(ISubmissionFileStore files) : IResourceFilter
{
    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        long requestLimit = checked(files.MaxFileBytes);
        var features = context.HttpContext.Features;
        var bodyLimit = features.Get<IHttpMaxRequestBodySizeFeature>();
        if (bodyLimit is { IsReadOnly: false })
            bodyLimit.MaxRequestBodySize = requestLimit;

        // Resource filters run before model binding reads the multipart body.
        features.Set<IFormFeature>(new FormFeature(context.HttpContext.Request,
            new FormOptions { MultipartBodyLengthLimit = requestLimit }));
    }

    public void OnResourceExecuted(ResourceExecutedContext context) { }
}
