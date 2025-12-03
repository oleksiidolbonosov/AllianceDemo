using AllianceDemo.Domain.Interfaces;
using UnityEngine;

namespace AllianceDemo.Infrastructure.Services
{
    /// <summary>
    /// Unity-based implementation of ILogService.
    /// Can be replaced with any other logging provider.
    /// </summary>
    public class UnityLogService : ILogService
    {
        public void Info(string message)
        {
            Debug.Log($"[INFO] {message}");
        }

        public void Error(string message)
        {
            Debug.LogError($"[ERROR] {message}");
        }
    }
}
