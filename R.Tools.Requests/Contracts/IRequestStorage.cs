using R.Tools.Requests.Entities;

namespace R.Tools.Requests.Contracts
{
    public interface IRequestStorage
    {
        void Send(RequestData data);
    }
}
