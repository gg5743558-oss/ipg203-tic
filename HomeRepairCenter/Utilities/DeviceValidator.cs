using System;
using System.Text.RegularExpressions;

namespace HomeRepairCenter.Utilities
{
    /// <summary>
    /// فئة ثابتة للتحقق من صحة البيانات المدخلة.
    /// </summary>
    public static class DeviceValidator
    {
        // الرقم التسلسلي: حرفان أو ثلاثة كبيرة، شرطة، ثم 4 إلى 8 أرقام (مثال: RF-100234)
        private static readonly Regex SerialPattern =
            new Regex(@"^[A-Z]{2,3}-\d{4,8}$", RegexOptions.Compiled);

        public static bool IsValidSerialNumber(string serial)
        {
            return !string.IsNullOrWhiteSpace(serial) && SerialPattern.IsMatch(serial);
        }

        public static bool IsValidText(string text, int minLength = 2)
        {
            return !string.IsNullOrWhiteSpace(text) && text.Trim().Length >= minLength;
        }

        public static bool IsValidDate(DateTime date)
        {
            return date != default && date.Year >= 2000;
        }

        // تاريخ التسليم المتوقع يجب ألا يسبق تاريخ الاستلام
        public static bool IsValidDeliveryDate(DateTime received, DateTime expected)
        {
            return IsValidDate(received) && IsValidDate(expected) && expected.Date >= received.Date;
        }

        public static bool IsNonNegative(decimal value)
        {
            return value >= 0m;
        }
    }
}
