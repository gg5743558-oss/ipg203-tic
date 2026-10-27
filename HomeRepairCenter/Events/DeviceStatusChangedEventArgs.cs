using System;
using HomeRepairCenter.Enums;

namespace HomeRepairCenter.Events
{
    /// <summary>
    /// بيانات الحدث المرسلة عند تغيّر حالة جهاز أو تأخر تسليمه.
    /// </summary>
    public class DeviceStatusChangedEventArgs : EventArgs
    {
        public string SerialNumber { get; }
        public string DeviceType { get; }
        public DeviceStatus OldStatus { get; }
        public DeviceStatus NewStatus { get; }
        public string Message { get; }
        public DateTime Timestamp { get; }

        public DeviceStatusChangedEventArgs(string serialNumber, string deviceType,
            DeviceStatus oldStatus, DeviceStatus newStatus, string message, DateTime timestamp)
        {
            SerialNumber = serialNumber;
            DeviceType = deviceType;
            OldStatus = oldStatus;
            NewStatus = newStatus;
            Message = message;
            Timestamp = timestamp;
        }
    }
}
