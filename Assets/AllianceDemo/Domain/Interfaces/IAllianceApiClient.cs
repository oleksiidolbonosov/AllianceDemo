using AllianceDemo.Application.Dtos;

namespace AllianceDemo.Domain.Interfaces
{
    /// <summary>
    /// Contract for backend communication.
    /// Handle HTTP/WebSocket/S2S — infrastructure is replaceable.
    /// </summary>
    public interface IAllianceApiClient
    {
        /// <summary>
        /// Sends a battle result payload to remote server.
        /// </summary>
        void SendBattleReport(BattleReportDto report);
    }
}