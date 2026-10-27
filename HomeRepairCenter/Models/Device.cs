using System;
using HomeRepairCenter.Enums;
using HomeRepairCenter.Events;
using HomeRepairCenter.Interfaces;
using HomeRepairCenter.Utilities;

namespace HomeRepairCenter.Models
{
    /// <summary>
    /// الفئة المجردة لجميع الأجهزة المنزلية. تطبق الواجهة IRepairable
    /// وتترك حساب تكلفة الإصلاح للفئات الفرعية.
    /// </summary>
    public abstract class Device : IRepairable
    {
        // ---------- الأعضاء الثابتة (إحصاءات المركز) ----------
        private static int _totalDevices;
        private static int _underRepairCount;
        private static decimal _totalRevenue;

        /// <summary>عدد الأجهزة المسجلة منذ بداية التشغيل.</summary>
        public static int TotalDevices => _totalDevices;

        /// <summary>عدد الأجهزة الموجودة حاليا قيد الصيانة.</summary>
        public static int UnderRepairCount => _underRepairCount;

        /// <summary>مجموع التكاليف المحصّلة من الأجهزة المسلّمة.</summary>
        public static decimal TotalRevenue => _totalRevenue;

        // ---------- الحقول (جميعها خاصة) ----------
        private readonly string _serialNumber;
        private readonly string _customerName;
        private readonly DateTime _receivedDate;
        private readonly DateTime _expectedDeliveryDate;
        private string _brand = string.Empty;
        private string _problemDescription = string.Empty;
        private DeviceStatus _status;
        private decimal _partsCost;
        private decimal _laborHours;
        private bool _delayNotified;

        // ---------- الأحداث ----------
        /// <summary>يُطلق عند أي تغيير في حالة الجهاز (استلام، إصلاح، تسليم...).</summary>
        public event DeviceStatusChangedHandler StatusChanged;

        /// <summary>يُطلق عند تجاوز موعد التسليم المتوقع دون إنهاء الصيانة.</summary>
        public event DeviceStatusChangedHandler DeliveryDelayed;

        protected Device(string serialNumber, string brand, string customerName,
            string problemDescription, DateTime receivedDate, DateTime expectedDeliveryDate)
        {
            if (!DeviceValidator.IsValidSerialNumber(serialNumber))
                throw new ArgumentException("Invalid serial number. Expected format like RF-100234.", nameof(serialNumber));
            if (!DeviceValidator.IsValidText(customerName))
                throw new ArgumentException("Customer name is required.", nameof(customerName));
            if (!DeviceValidator.IsValidDeliveryDate(receivedDate, expectedDeliveryDate))
                throw new ArgumentException("Expected delivery date must not be before the received date.", nameof(expectedDeliveryDate));

            _serialNumber = serialNumber;
            _customerName = customerName.Trim();
            _receivedDate = receivedDate;
            _expectedDeliveryDate = expectedDeliveryDate;
            _status = DeviceStatus.Received;

            Brand = brand;                           // يمر عبر التحقق في الـ setter
            ProblemDescription = problemDescription;

            _totalDevices++;
        }

        // ---------- خصائص للقراءة فقط بعد الإنشاء ----------
        public string SerialNumber => _serialNumber;
        public string CustomerName => _customerName;
        public DateTime ReceivedDate => _receivedDate;
        public DateTime ExpectedDeliveryDate => _expectedDeliveryDate;
        public DeviceStatus Status => _status;
        public bool IsDelayed => _delayNotified;

        // ---------- خصائص قابلة للتعديل مع تحقق ----------
        public string Brand
        {
            get => _brand;
            set
            {
                if (!DeviceValidator.IsValidText(value))
                    throw new ArgumentException("Brand is required.", nameof(value));
                _brand = value.Trim();
            }
        }

        public string ProblemDescription
        {
            get => _problemDescription;
            set
            {
                if (!DeviceValidator.IsValidText(value, 3))
                    throw new ArgumentException("Problem description is too short.", nameof(value));
                _problemDescription = value.Trim();
            }
        }

        /// <summary>تكلفة قطع الغيار. لا يمكن تعديلها بعد اكتمال الإصلاح.</summary>
        public decimal PartsCost
        {
            get => _partsCost;
            set
            {
                EnsureCostEditable();
                if (!DeviceValidator.IsNonNegative(value))
                    throw new ArgumentOutOfRangeException(nameof(value), "Parts cost cannot be negative.");
                _partsCost = value;
            }
        }

        /// <summary>عدد ساعات العمل. لا يمكن تعديلها بعد اكتمال الإصلاح.</summary>
        public decimal LaborHours
        {
            get => _laborHours;
            set
            {
                EnsureCostEditable();
                if (!DeviceValidator.IsNonNegative(value))
                    throw new ArgumentOutOfRangeException(nameof(value), "Labor hours cannot be negative.");
                _laborHours = value;
            }
        }

        /// <summary>اسم نوع الجهاز كما يظهر في التقارير.</summary>
        public abstract string DeviceTypeName { get; }

        /// <summary>
        /// حساب تكلفة الإصلاح - كل نوع جهاز يحسبها بطريقته الخاصة.
        /// </summary>
        public abstract decimal CalculateRepairCost();

        // ---------- دورة حياة الصيانة ----------
        public void StartRepair()
        {
            if (_status != DeviceStatus.Received)
                throw new InvalidOperationException($"Cannot start repair while status is {_status}.");
            ChangeStatus(DeviceStatus.UnderRepair, "Repair work started.");
        }

        public void CompleteRepair()
        {
            if (_status != DeviceStatus.UnderRepair)
                throw new InvalidOperationException($"Cannot complete repair while status is {_status}.");
            ChangeStatus(DeviceStatus.Repaired, $"Repair completed. Final cost: {CalculateRepairCost():F2}");
        }

        public void DeliverToCustomer()
        {
            if (_status != DeviceStatus.Repaired)
                throw new InvalidOperationException("Only a repaired device can be delivered.");
            _totalRevenue += CalculateRepairCost();
            ChangeStatus(DeviceStatus.Delivered, "Device delivered to the customer.");
        }

        /// <summary>
        /// يفحص تأخر التسليم نسبة إلى التاريخ الحالي ويطلق الحدث مرة واحدة فقط.
        /// </summary>
        public bool CheckDelay(DateTime currentDate)
        {
            bool stillOpen = _status == DeviceStatus.Received || _status == DeviceStatus.UnderRepair;
            if (!stillOpen || _delayNotified || currentDate.Date <= _expectedDeliveryDate.Date)
                return false;

            _delayNotified = true;
            int lateDays = (currentDate.Date - _expectedDeliveryDate.Date).Days;
            var args = new DeviceStatusChangedEventArgs(_serialNumber, DeviceTypeName,
                _status, _status, $"Delivery delayed by {lateDays} day(s).", currentDate);
            OnDeliveryDelayed(args);
            return true;
        }

        public virtual string GetSummary()
        {
            return $"{DeviceTypeName,-15} | {_serialNumber,-10} | {_brand,-10} | {_customerName,-12} | " +
                   $"{_status,-11} | Cost: {CalculateRepairCost(),8:F2}{(IsDelayed ? " | DELAYED" : string.Empty)}";
        }

        // ---------- أدوات مساعدة محمية ----------
        /// <summary>تكلفة اليد العاملة = الساعات × أجر الساعة الخاص بنوع الجهاز.</summary>
        protected decimal CalculateLaborCost(decimal hourlyRate)
        {
            return _laborHours * hourlyRate;
        }

        protected virtual void OnStatusChanged(DeviceStatusChangedEventArgs e)
        {
            StatusChanged?.Invoke(this, e);
        }

        protected virtual void OnDeliveryDelayed(DeviceStatusChangedEventArgs e)
        {
            DeliveryDelayed?.Invoke(this, e);
        }

        private void ChangeStatus(DeviceStatus newStatus, string message)
        {
            DeviceStatus oldStatus = _status;

            if (oldStatus == DeviceStatus.UnderRepair) _underRepairCount--;
            if (newStatus == DeviceStatus.UnderRepair) _underRepairCount++;

            _status = newStatus;
            OnStatusChanged(new DeviceStatusChangedEventArgs(_serialNumber, DeviceTypeName,
                oldStatus, newStatus, message, DateTime.Now));
        }

        private void EnsureCostEditable()
        {
            if (_status == DeviceStatus.Repaired || _status == DeviceStatus.Delivered)
                throw new InvalidOperationException("Costs are locked after the repair is completed.");
        }
    }
}
