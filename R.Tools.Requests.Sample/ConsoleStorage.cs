using Newtonsoft.Json;
using R.Tools.Requests.Contracts;
using R.Tools.Requests.Entities;

namespace R.Tools.Requests.Sample
{
    public class ConsoleStorage(ILogger<ConsoleStorage> log) : IRequestStorage
    {

        public void Send(RequestData data)
        {
            log.LogCritical(new EventId(1, "DATA_LOG"), JsonConvert.SerializeObject(data));
        }
    }
}
