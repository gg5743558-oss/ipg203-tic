using System;

namespace HomeRepairCenter.Models
{
    /// <summary>غسالة: التكلفة تتأثر باستبدال المحرك وبإصلاح تسرب المياه.</summary>
    public class WashingMachine : Device
    {
        private const decimal HourlyRate = 18m;
        private const decimal MotorReplacementFee = 40m;
        private const decimal LeakRepairFee = 12m;

        private double _loadCapacityKg;
        private bool _motorReplaced;
        private bool _hasWaterLeak;

        public WashingMachine(string serialNumber, string brand, string customerName,
            string problemDescription, DateTime receivedDate, DateTime expectedDeliveryDate,
            double loadCapacityKg, bool motorReplaced, bool hasWaterLeak)
            : base(serialNumber, brand, customerName, problemDescription, receivedDate, expectedDeliveryDate)
        {
            LoadCapacityKg = loadCapacityKg;
            _motorReplaced = motorReplaced;
            _hasWaterLeak = hasWaterLeak;
        }

        public double LoadCapacityKg
        {
            get => _loadCapacityKg;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Load capacity must be positive.");
                _loadCapacityKg = value;
            }
        }

        public bool MotorReplaced
        {
            get => _motorReplaced;
            set => _motorReplaced = value;
        }

        public bool HasWaterLeak
        {
            get => _hasWaterLeak;
            set => _hasWaterLeak = value;
        }

        public override string DeviceTypeName => "Washing Machine";

        public override decimal CalculateRepairCost()
        {
            decimal total = PartsCost + CalculateLaborCost(HourlyRate);
            if (_motorReplaced) total += MotorReplacementFee;
            if (_hasWaterLeak) total += LeakRepairFee;
            return total;
        }

        public override string GetSummary()
        {
            return base.GetSummary() + $" | {_loadCapacityKg}kg" + (_motorReplaced ? ", new motor" : string.Empty);
        }
    }
}
