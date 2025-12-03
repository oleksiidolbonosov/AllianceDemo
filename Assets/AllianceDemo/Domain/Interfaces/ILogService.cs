namespace AllianceDemo.Domain.Interfaces
{
    /// <summary>
    /// Small abstraction for logging, decoupled from UnityEngine.Debug.
    /// </summary>
    public interface ILogService
    {
        void Info(string message);
        void Error(string message);
    }
}
