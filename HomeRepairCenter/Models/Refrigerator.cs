using System;

namespace HomeRepairCenter.Models
{
    /// <summary>ثلاجة: التكلفة تتأثر بإعادة تعبئة الغاز وبالسعة الكبيرة.</summary>
    public class Refrigerator : Device
    {
        private const decimal HourlyRate = 20m;
        private const decimal GasRefillFee = 35m;
        private const decimal LargeCapacityFee = 15m;
        private const int LargeCapacityLiters = 500;

        private int _capacityLiters;
        private bool _needsGasRefill;

        public Refrigerator(string serialNumber, string brand, string customerName,
            string problemDescription, DateTime receivedDate, DateTime expectedDeliveryDate,
            int capacityLiters, bool needsGasRefill)
            : base(serialNumber, brand, customerName, problemDescription, receivedDate, expectedDeliveryDate)
        {
            CapacityLiters = capacityLiters;
            _needsGasRefill = needsGasRefill;
        }

        public int CapacityLiters
        {
            get => _capacityLiters;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Capacity must be positive.");
                _capacityLiters = value;
            }
        }

        public bool NeedsGasRefill
        {
            get => _needsGasRefill;
            set => _needsGasRefill = value;
        }

        public override string DeviceTypeName => "Refrigerator";

        public override decimal CalculateRepairCost()
        {
            decimal total = PartsCost + CalculateLaborCost(HourlyRate);
            if (_needsGasRefill) total += GasRefillFee;
            if (_capacityLiters > LargeCapacityLiters) total += LargeCapacityFee;
            return total;
        }

        public override string GetSummary()
        {
            return base.GetSummary() + $" | {_capacityLiters}L" + (_needsGasRefill ? ", gas refill" : string.Empty);
        }
    }
}
