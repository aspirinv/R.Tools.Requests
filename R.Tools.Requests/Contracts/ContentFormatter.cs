namespace R.Tools.Requests.Contracts
{
    interface IContentFormatterBase
    {
        object FixObjectBase(object body);
    }

    public abstract class ContentFormatter<T> : Attribute, IContentFormatterBase
    {
        public object FixObjectBase(object body)
        {
            if(body.GetType() == typeof(T))
                return FixObject((T)body);
            throw new NotSupportedException($"Content formatter type doesn't support request body type. Formatter: {typeof(T)}, Body: {body.GetType()}");
        }
        public abstract object FixObject(T body);
    }
}
