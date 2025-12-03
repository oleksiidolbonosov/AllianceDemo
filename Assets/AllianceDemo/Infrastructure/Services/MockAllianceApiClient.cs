using AllianceDemo.Application.Dtos;
using AllianceDemo.Domain.Interfaces;
using UnityEngine;

namespace AllianceDemo.Infrastructure.Services
{
    /// <summary>
    /// Mock implementation of IAllianceApiClient.
    /// In production this would send HTTP/WebSocket requests.
    /// Here we just log serialized data to show intent.
    /// </summary>
    public class MockAllianceApiClient : IAllianceApiClient
    {
        public void SendBattleReport(BattleReportDto report)
        {
            var json = JsonUtility.ToJson(report);
            Debug.Log($"[NETWORK MOCK] POST /battle/report payload={json}");
        }
    }
}
