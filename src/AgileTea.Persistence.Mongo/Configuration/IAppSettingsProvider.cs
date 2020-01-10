namespace AgileTea.Persistence.Mongo.Configuration
{
    public interface IAppSettingsProvider
    {
        string DbConnection { get; }
        string DbName { get; }
    }
}
