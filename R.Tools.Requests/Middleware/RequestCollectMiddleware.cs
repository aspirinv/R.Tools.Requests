using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using R.Tools.Requests.Contracts;

namespace R.Tools.Requests.Middleware
{
    class RequestCollectMiddleware(ILogger<RequestCollectMiddleware> logger, IServiceScopeFactory scopefactory,
        CollectorOptions options) : IRequestCollectMiddleware
    {

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var ep = context.GetEndpoint()?.Metadata;
            logger.LogTrace($"Start processing path {context.Request.Path}");
            if (CanSaveRequest(context.Request.Path, next, ep))
            {
                await SaveRequest(context, next, ep!);
            }
            else
            {
                await next.Invoke(context);
            }
        }

        private bool CanSaveRequest(string path, RequestDelegate next, EndpointMetadataCollection? ep)
        {
            if(ep == null)
            {
                logger.LogTrace("Save cancelled. Request continues without saving");
                return false;
            }
            if (!options.ShouldCollect(path))
            {
                logger.LogTrace("Save skipped based on path validation");
                return false;
            }
            if (ep.GetMetadata<SkipRequestSavingAttribute>() != null)
            {
                logger.LogTrace("Save skipped based on attribute set on the controller action");
                return false;
            }
            return true;
        }

        private async Task SaveRequest(HttpContext context, RequestDelegate next, EndpointMetadataCollection ep)
        {
            var builder = new RequestLogBuilder(context.Request)
                .From(context.Connection)
                .AsUser(options.DefineUserId(context.User))
                .WithBody(await CollectBody(context.Request, ep))
                .SavedBy(options.ServiceName);
            
            try
            {
                await next.Invoke(context);
                logger.LogTrace($"Main method executed");
                builder.Success(context.Response);
                logger.LogTrace($"Success marked");
            }
            catch (Exception ex)
            {
                logger.LogTrace(ex, $"Failure on underlying execution");
                builder.Failure(ex);
                throw;
            }
            finally
            {
                logger.LogTrace($"Collection execution results");
                var item = builder.Finish();
                logger.LogTrace($"Start saving async");

                _ = Task.Run(() =>
                {
                    using var scope = scopefactory.CreateScope();
                    var storage = scope.ServiceProvider.GetRequiredService<IRequestStorage>();
                    storage.Send(item);
                });
            }
        }

        private async Task<object?> CollectBody(HttpRequest request, EndpointMetadataCollection ep)
        {
            if (request.ContentLength == 0 || !request.HasJsonContentType())
            {
                return string.Empty;
            }
            request.EnableBuffering();
            var descriptor = ep.GetMetadata<ActionDescriptor>();
            var bodyParameter = descriptor?.Parameters.FirstOrDefault(p => p.BindingInfo?.BindingSource == BindingSource.Body);
            if (bodyParameter != null)
            {
                var body = await request.ReadFromJsonAsync(bodyParameter.ParameterType);
                request.Body.Position = 0;
                if (body != null)
                {
                    var formatter = ep.GetMetadata<IContentFormatterBase>();
                    if (formatter != null)
                    {
                        body = formatter.FixObjectBase(body);
                    }
                    logger.LogTrace($"Body collected");
                    return body;
                }
            }

            using var memory = new MemoryStream();
            await request.Body.CopyToAsync(memory);
            memory.Position = 0;
            request.Body.Position = 0;

            using var reader = new StreamReader(memory);
            var result = reader.ReadToEnd();
            logger.LogTrace($"Raw body collected");
            return result;
        }

    }
}
