namespace R.Tools.Requests.Contracts
{
    public class StorageConfigurationMissingException: ApplicationException
    {
        public StorageConfigurationMissingException() 
            : base("Missing implementation registration of the IRequestStorage. Please register one in a builder. Services or use an extention method that fits you the most") { }
    }
    public class StorageOptionsMisconfigurationException: ApplicationException
    {
        public StorageOptionsMisconfigurationException() 
            : base("Storage options can't be defined from configuration. Please check the configuration. Section: StorageOptions -> ConnectionString") { }
    }
}
