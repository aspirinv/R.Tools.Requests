using Microsoft.AspNetCore.Http;
using R.Tools.Requests.Entities;

namespace R.Tools.Requests
{
    class RequestLogBuilder
    {
        private RequestData _log;

        internal RequestLogBuilder(HttpRequest request)
        {
            _log = new RequestData(request.Method,
                request.QueryString.ToString(),
                request.Path.ToUriComponent(),
                DateTime.UtcNow);
        }

        internal RequestLogBuilder From(ConnectionInfo connection)
        {
            _log.IpAddress = Convert.ToString(connection.RemoteIpAddress);
            return this;
        }

        internal RequestLogBuilder Success(HttpResponse response)
        {
            _log.ResponseStatus = response.StatusCode;
            return this;
        }

        internal RequestLogBuilder Failure(Exception exception)
        {
            _log.FailureText = exception.Message;
            return this;
        }

        internal RequestLogBuilder AsUser(string? uId)
        {
            _log.UserId = uId;
            return this;
        }

        internal RequestLogBuilder WithBody(object? content)
        {
            _log.ContentObj = content;
            return this;
        }

        internal RequestData Finish()
        {
            _log.EndTime = DateTime.UtcNow;
            return _log;
        }

        internal RequestLogBuilder SavedBy(string? serviceName)
        {
            _log.ServiceName = serviceName;
            return this;
        }
    }
}
