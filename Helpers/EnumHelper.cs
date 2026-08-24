using TourManagement.Web.Models.Enums;

namespace TourManagement.Web.Helpers;

public static class EnumHelper
{
    public static string GetStatusName(int status, Type type)
    {
        var name = Enum.GetName(type, status);
        return name ?? status.ToString();
    }

    public static string GetTourStatusName(int s) => GetStatusName(s, typeof(TourStatus));
    public static string GetScheduleStatusName(int s) => GetStatusName(s, typeof(ScheduleStatus));
    public static string GetBookingStatusName(int s) => GetStatusName(s, typeof(BookingStatus));
    public static string GetPaymentStatusName(int s) => GetStatusName(s, typeof(PaymentStatus));
    public static string GetPaymentMethodName(int s) => GetStatusName(s, typeof(PaymentMethod));
    public static string GetReviewStatusName(int s) => GetStatusName(s, typeof(ReviewStatus));
    public static string GetPromotionStatusName(int s) => GetStatusName(s, typeof(PromotionStatus));
    public static string GetGuideStatusName(int s) => GetStatusName(s, typeof(GuideStatus));
    public static string GetDiscountTypeName(int s) => GetStatusName(s, typeof(DiscountType));
    public static string GetGenderName(int? s) => s == null ? "" : GetStatusName(s.Value, typeof(Gender));

    public static string GetBadgeClass(int status, Type type)
    {
        if (type == typeof(BookingStatus))
        {
            return status switch
            {
                (int)BookingStatus.PENDING => "bg-warning text-dark",
                (int)BookingStatus.CONFIRMED => "bg-info text-dark",
                (int)BookingStatus.PAID => "bg-success",
                (int)BookingStatus.CANCELLED => "bg-danger",
                (int)BookingStatus.COMPLETED => "bg-primary",
                _ => "bg-secondary"
            };
        }
        if (type == typeof(ScheduleStatus))
        {
            return status switch
            {
                (int)ScheduleStatus.OPEN => "bg-success",
                (int)ScheduleStatus.FULL => "bg-warning text-dark",
                (int)ScheduleStatus.CLOSED => "bg-secondary",
                (int)ScheduleStatus.CANCELLED => "bg-danger",
                _ => "bg-secondary"
            };
        }
        if (type == typeof(PaymentStatus))
        {
            return status switch
            {
                (int)PaymentStatus.UNPAID => "bg-danger",
                (int)PaymentStatus.PARTIAL_PAID => "bg-warning text-dark",
                (int)PaymentStatus.PAID => "bg-success",
                (int)PaymentStatus.REFUNDED => "bg-secondary",
                _ => "bg-secondary"
            };
        }
        if (type == typeof(UserStatus) || type == typeof(GuideStatus) ||
            type == typeof(PromotionStatus) || type == typeof(DestinationStatus))
        {
            return status == 1 ? "bg-success" : "bg-danger";
        }
        if (type == typeof(ReviewStatus))
        {
            return status == 1 ? "bg-success" : "bg-secondary";
        }
        return "bg-secondary";
    }
}