namespace HomeRepairCenter.Enums
{
    /// <summary>
    /// حالات الجهاز خلال دورة الصيانة (بالترتيب).
    /// </summary>
    public enum DeviceStatus
    {
        Received = 0,     // تم الاستلام من الزبون
        UnderRepair = 1,  // قيد الصيانة
        Repaired = 2,     // تم الإصلاح وجاهز للتسليم
        Delivered = 3     // سُلّم للزبون
    }
}
