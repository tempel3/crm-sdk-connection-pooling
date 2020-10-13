namespace Kaziya.CRM.ConnectionPooling
{
    public interface IConnectionPoolLogger
    {
        void Debug(string message);
        void Warn(string message);
    }
}