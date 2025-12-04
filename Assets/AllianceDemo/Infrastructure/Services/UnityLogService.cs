using AllianceDemo.Domain.Interfaces;
using UnityEngine;

namespace AllianceDemo.Infrastructure.Services
{
    /// <summary>
    /// Default log provider — redirectable to backend, file, analytics etc.
    /// </summary>
    public class UnityLogService : ILogService
    {
        public void Info(string message) => Debug.Log($"[INFO] {message}");
        public void Error(string message) => Debug.LogError($"[ERROR] {message}");
    }
}
