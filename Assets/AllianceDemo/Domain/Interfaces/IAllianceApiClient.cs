using AllianceDemo.Application.Dtos;

namespace AllianceDemo.Domain.Interfaces
{
    /// <summary>
    /// Abstraction for sending data to a backend service.
    /// In production this would be implemented with HTTP / WebSockets.
    /// </summary>
    public interface IAllianceApiClient
    {
        /// <summary>
        /// Sends a battle report to backend. In this demo it's a mock, but
        /// the interface is ready for a real implementation.
        /// </summary>
        void SendBattleReport(BattleReportDto report);
    }
}
