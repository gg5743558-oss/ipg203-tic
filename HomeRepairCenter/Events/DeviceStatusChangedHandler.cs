namespace HomeRepairCenter.Events
{
    /// <summary>
    /// التفويض (Delegate) المسؤول عن الإشعار بتغيّر حالة الجهاز.
    /// </summary>
    public delegate void DeviceStatusChangedHandler(object sender, DeviceStatusChangedEventArgs e);
}
