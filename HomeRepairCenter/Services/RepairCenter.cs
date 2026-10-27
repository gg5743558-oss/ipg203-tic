using System;
using System.Collections.Generic;
using System.Linq;
using HomeRepairCenter.Enums;
using HomeRepairCenter.Events;
using HomeRepairCenter.Models;

namespace HomeRepairCenter.Services
{
    /// <summary>
    /// الفئة الرئيسية: تحتفظ بقائمة من Device وتعالجها بتعدد الأشكال،
    /// وتشترك في أحداث الأجهزة لتسجيل الإشعارات.
    /// </summary>
    public class RepairCenter
    {
        private readonly List<Device> _devices = new List<Device>();
        private readonly List<string> _notifications = new List<string>();

        public RepairCenter(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public IReadOnlyList<Device> Devices => _devices.AsReadOnly();
        public IReadOnlyList<string> Notifications => _notifications.AsReadOnly();

        public void AddDevice(Device device)
        {
            if (device == null) throw new ArgumentNullException(nameof(device));
            if (FindBySerial(device.SerialNumber) != null)
                throw new InvalidOperationException($"Device {device.SerialNumber} is already registered.");

            device.StatusChanged += OnDeviceStatusChanged;
            device.DeliveryDelayed += OnDeviceDelayed;
            _devices.Add(device);
        }

        public Device FindBySerial(string serialNumber)
        {
            return _devices.FirstOrDefault(d =>
                string.Equals(d.SerialNumber, serialNumber, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<Device> GetDevicesByStatus(DeviceStatus status)
        {
            return _devices.Where(d => d.Status == status);
        }

        /// <summary>الأجهزة التي لم يُنجز إصلاحها بعد (مستلمة أو قيد الصيانة).</summary>
        public IEnumerable<Device> GetPendingDevices()
        {
            return _devices.Where(d => d.Status == DeviceStatus.Received || d.Status == DeviceStatus.UnderRepair);
        }

        /// <summary>الأجهزة التي تم إصلاحها (جاهزة أو مسلّمة).</summary>
        public IEnumerable<Device> GetRepairedDevices()
        {
            return _devices.Where(d => d.Status == DeviceStatus.Repaired || d.Status == DeviceStatus.Delivered);
        }

        /// <summary>
        /// مجموع تكاليف الإصلاح لكل الأجهزة، أو لأجهزة حالة محددة.
        /// يستدعي CalculateRepairCost بتعدد الأشكال دون معرفة نوع الجهاز.
        /// </summary>
        public decimal GetTotalCost(DeviceStatus? status = null)
        {
            return _devices
                .Where(d => status == null || d.Status == status)
                .Sum(d => d.CalculateRepairCost());
        }

        /// <summary>يفحص تأخر جميع الأجهزة المفتوحة ويرجع عدد الأجهزة المتأخرة حديثا.</summary>
        public int CheckDelays(DateTime currentDate)
        {
            int count = 0;
            foreach (Device device in _devices)
            {
                if (device.CheckDelay(currentDate)) count++;
            }
            return count;
        }

        public void PrintAllDevices()
        {
            foreach (Device device in _devices)
            {
                Console.WriteLine(device.GetSummary());
            }
        }

        // ---------- معالجات الأحداث ----------
        private void OnDeviceStatusChanged(object sender, DeviceStatusChangedEventArgs e)
        {
            string line = $"[{e.Timestamp:yyyy-MM-dd HH:mm}] {e.DeviceType} {e.SerialNumber}: " +
                          $"{e.OldStatus} -> {e.NewStatus} | {e.Message}";
            _notifications.Add(line);
            Console.WriteLine("  [NOTIFY] " + line);
        }

        private void OnDeviceDelayed(object sender, DeviceStatusChangedEventArgs e)
        {
            string line = $"[{e.Timestamp:yyyy-MM-dd}] {e.DeviceType} {e.SerialNumber}: {e.Message}";
            _notifications.Add(line);
            Console.WriteLine("  [ALERT]  " + line);
        }
    }
}
