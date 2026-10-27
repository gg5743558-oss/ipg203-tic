using System;
using System.Text;
using HomeRepairCenter.Enums;
using HomeRepairCenter.Models;
using HomeRepairCenter.Services;
using HomeRepairCenter.Utilities;

namespace HomeRepairCenter
{
    internal static class Program
    {
        private static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            var center = new RepairCenter("Home Appliance Repair Center");
            DateTime today = DateTime.Today;

            Console.WriteLine("=== 1. Registering devices ===");
            center.AddDevice(new Refrigerator("RF-100234", "Samsung", "Ahmad", "Not cooling",
                today, today.AddDays(3), 450, true) { PartsCost = 60m, LaborHours = 2m });
            center.AddDevice(new WashingMachine("WM-200551", "LG", "Lina", "Drum does not spin",
                today, today.AddDays(2), 8.5, true, false) { PartsCost = 25m, LaborHours = 3m });
            center.AddDevice(new AirConditioner("AC-300871", "Gree", "Omar", "Warm air only",
                today, today.AddDays(5), 24000, true, true) { PartsCost = 10m, LaborHours = 1.5m });
            center.AddDevice(new Refrigerator("RF-100777", "Bosch", "Sara", "Noisy compressor",
                today, today.AddDays(1), 600, false) { PartsCost = 80m, LaborHours = 4m });
            Console.WriteLine($"Registered devices: {Device.TotalDevices}");

            Console.WriteLine("\n=== 2. Repair workflow (events fire on each change) ===");
            center.FindBySerial("RF-100234").StartRepair();
            center.FindBySerial("RF-100234").CompleteRepair();
            center.FindBySerial("WM-200551").StartRepair();
            center.FindBySerial("AC-300871").StartRepair();
            Console.WriteLine($"Currently under repair: {Device.UnderRepairCount}");

            Console.WriteLine("\n=== 3. Delivering a repaired device ===");
            center.FindBySerial("RF-100234").DeliverToCustomer();
            Console.WriteLine($"Revenue so far: {Device.TotalRevenue:F2}");

            Console.WriteLine("\n=== 4. Delay check (simulating 10 days later) ===");
            int late = center.CheckDelays(today.AddDays(10));
            Console.WriteLine($"Newly delayed devices: {late}");

            Console.WriteLine("\n=== 5. Polymorphic report ===");
            center.PrintAllDevices();
            Console.WriteLine($"Total cost (all devices): {center.GetTotalCost():F2}");
            Console.WriteLine($"Total cost (Delivered):   {center.GetTotalCost(DeviceStatus.Delivered):F2}");

            Console.WriteLine("\n=== 6. Pending vs repaired ===");
            foreach (Device d in center.GetPendingDevices())
                Console.WriteLine($"Pending : {d.SerialNumber} ({d.DeviceTypeName})");
            foreach (Device d in center.GetRepairedDevices())
                Console.WriteLine($"Repaired: {d.SerialNumber} ({d.DeviceTypeName})");

            Console.WriteLine("\n=== 7. Validation & encapsulation ===");
            Console.WriteLine($"IsValidSerialNumber(\"RF-100234\") = {DeviceValidator.IsValidSerialNumber("RF-100234")}");
            Console.WriteLine($"IsValidSerialNumber(\"123\")       = {DeviceValidator.IsValidSerialNumber("123")}");
            try
            {
                center.AddDevice(new AirConditioner("BAD", "X", "Y", "Broken", today, today, 12000, false, false));
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Rejected: " + ex.Message);
            }
            try
            {
                center.FindBySerial("RF-100234").PartsCost = 1m; // مقفلة بعد الإصلاح
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine("Rejected: " + ex.Message);
            }
            // SerialNumber للقراءة فقط: السطر التالي لو فُعّل لن يُترجم
            // center.FindBySerial("RF-100234").SerialNumber = "XX-000000";

            Console.WriteLine($"\nNotifications logged: {center.Notifications.Count}");
        }
    }
}
