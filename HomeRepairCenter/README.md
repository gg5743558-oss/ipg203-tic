# Home Appliance Repair Center (IPG203 – OOP in C#)

نظام إدارة مركز صيانة وإصلاح الأجهزة المنزلية (ثلاجة، غسالة، مكيف) مبني بلغة C# ومبادئ البرمجة غرضية التوجه.

## التشغيل
```
افتح HomeRepairCenter.csproj في Visual Studio مع استهداف .NET Framework 4.8، ثم شغّل المشروع (F5).
```
(يتطلب .NET Framework 4.8 Developer Pack على Windows)

## هيكل المشروع
| المجلد | المحتوى |
|---|---|
| `Interfaces/` | `IRepairable` – العمليات المشتركة |
| `Models/` | `Device` (مجردة) + `Refrigerator`, `WashingMachine`, `AirConditioner` |
| `Events/` | `DeviceStatusChangedHandler` (Delegate) + `DeviceStatusChangedEventArgs` |
| `Services/` | `RepairCenter` – قائمة الأجهزة ومعالجتها بتعدد الأشكال |
| `Utilities/` | `DeviceValidator` – فئة ثابتة للتحقق |
| `Enums/` | `DeviceStatus` |

## المبادئ المطبقة
- **التجريد:** `IRepairable` + `Device` (فيها `CalculateRepairCost()` مجردة)
- **الوراثة وتعدد الأشكال:** 3 فئات فرعية، و`List<Device>` داخل `RepairCenter`
- **التغليف:** حقول خاصة، خصائص، `SerialNumber` للقراءة فقط، قفل التكاليف بعد الإصلاح
- **التفويض والأحداث:** `StatusChanged` و`DeliveryDelayed`
- **الثوابت:** `DeviceValidator` + `Device.TotalDevices / UnderRepairCount / TotalRevenue`
