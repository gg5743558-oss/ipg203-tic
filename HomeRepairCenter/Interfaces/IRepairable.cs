using HomeRepairCenter.Enums;

namespace HomeRepairCenter.Interfaces
{
    /// <summary>
    /// العمليات الأساسية المشتركة بين جميع الأجهزة المنزلية في مركز الصيانة.
    /// </summary>
    public interface IRepairable
    {
        string SerialNumber { get; }
        DeviceStatus Status { get; }

        void StartRepair();
        void CompleteRepair();
        void DeliverToCustomer();

        decimal CalculateRepairCost();
        string GetSummary();
    }
}
