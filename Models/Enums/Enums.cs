namespace TourManagement.Web.Models.Enums;

public enum TourStatus
{
    ACTIVE = 1,
    INACTIVE = 2
}

public enum TourType
{
    DOMESTIC = 1,
    INTERNATIONAL = 2,
    GROUP = 3,
    PRIVATE = 4
}

public enum ScheduleStatus
{
    OPEN = 1,
    FULL = 2,
    CLOSED = 3,
    CANCELLED = 4
}

public enum BookingStatus
{
    PENDING = 1,
    CONFIRMED = 2,
    PAID = 3,
    CANCELLED = 4,
    COMPLETED = 5
}

public enum PaymentStatus
{
    UNPAID = 1,
    PARTIAL_PAID = 2,
    PAID = 3,
    REFUNDED = 4
}

public enum PaymentMethod
{
    CASH = 1,
    BANK_TRANSFER = 2,
    ONLINE = 3
}

public enum UserStatus
{
    ACTIVE = 1,
    LOCKED = 2
}

public enum Gender
{
    MALE = 1,
    FEMALE = 2,
    OTHER = 3
}

public enum DestinationStatus
{
    ACTIVE = 1,
    INACTIVE = 2
}

public enum GuideStatus
{
    ACTIVE = 1,
    INACTIVE = 2
}

public enum PromotionStatus
{
    ACTIVE = 1,
    INACTIVE = 2
}

public enum DiscountType
{
    PERCENT = 1,
    FIXED = 2
}

public enum ReviewStatus
{
    VISIBLE = 1,
    HIDDEN = 2
}

public enum NotificationStatus
{
    UNREAD = 1,
    READ = 2
}