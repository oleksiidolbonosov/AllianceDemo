namespace AllianceDemo.Domain.Interfaces
{
    /// <summary>
    /// Abstract logging service decoupled from UnityEngine.Debug.
    /// Allows console/log file/cloud logging without UI dependency.
    /// </summary>
    public interface ILogService
    {
        void Info(string message);
        void Error(string message);
    }
}