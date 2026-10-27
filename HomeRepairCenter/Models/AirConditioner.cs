using System;

namespace HomeRepairCenter.Models
{
    /// <summary>مكيف: التكلفة تتأثر بسعة التبريد (BTU) وبزيارة الموقع.</summary>
    public class AirConditioner : Device
    {
        private const decimal HourlyRate = 25m;
        private const decimal GasRechargePer12kBtu = 30m;
        private const decimal OnSiteVisitFee = 20m;
        private const decimal BtuUnit = 12000m;

        private int _btuCapacity;
        private bool _needsGasRecharge;
        private bool _requiresOnSiteVisit;

        public AirConditioner(string serialNumber, string brand, string customerName,
            string problemDescription, DateTime receivedDate, DateTime expectedDeliveryDate,
            int btuCapacity, bool needsGasRecharge, bool requiresOnSiteVisit)
            : base(serialNumber, brand, customerName, problemDescription, receivedDate, expectedDeliveryDate)
        {
            BtuCapacity = btuCapacity;
            _needsGasRecharge = needsGasRecharge;
            _requiresOnSiteVisit = requiresOnSiteVisit;
        }

        public int BtuCapacity
        {
            get => _btuCapacity;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "BTU capacity must be positive.");
                _btuCapacity = value;
            }
        }

        public bool NeedsGasRecharge
        {
            get => _needsGasRecharge;
            set => _needsGasRecharge = value;
        }

        public bool RequiresOnSiteVisit
        {
            get => _requiresOnSiteVisit;
            set => _requiresOnSiteVisit = value;
        }

        public override string DeviceTypeName => "Air Conditioner";

        public override decimal CalculateRepairCost()
        {
            decimal total = PartsCost + CalculateLaborCost(HourlyRate);
            if (_needsGasRecharge) total += (_btuCapacity / BtuUnit) * GasRechargePer12kBtu;
            if (_requiresOnSiteVisit) total += OnSiteVisitFee;
            return total;
        }

        public override string GetSummary()
        {
            return base.GetSummary() + $" | {_btuCapacity} BTU" + (_requiresOnSiteVisit ? ", on-site" : string.Empty);
        }
    }
}
