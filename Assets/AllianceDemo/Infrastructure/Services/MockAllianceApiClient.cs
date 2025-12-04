using AllianceDemo.Application.Dtos;
using AllianceDemo.Domain.Interfaces;
using UnityEngine;

namespace AllianceDemo.Infrastructure.Services
{
    /// <summary>
    /// Mock network service — prints JSON payload in console.
    /// Swap with real implementation anytime.
    /// </summary>
    public class MockAllianceApiClient : IAllianceApiClient
    {
        public void SendBattleReport(BattleReportDto report)
        {
            var json = JsonUtility.ToJson(report, prettyPrint: true);
            Debug.Log($"[NETWORK] BattleReport sent:\n{json}");
        }
    }
}
