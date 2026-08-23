using System.Security.Cryptography;
using System.Text;

namespace TourManagement.Web.Helpers;

public static class CodeGenerator
{
    public static string GenerateBookingCode()
    {
        var prefix = $"BK{DateTime.UtcNow:yyyyMMdd}";
        var random = RandomNumberGenerator.GetInt32(1000, 9999);
        return $"{prefix}{random}";
    }

    public static string GenerateTransactionCode()
    {
        return $"TX{DateTime.UtcNow:yyyyMMddHHmmssfff}";
    }

    public static string GenerateScheduleCode(int tourId)
    {
        return $"SCH-{tourId}-{DateTime.UtcNow:yyyyMMddHHmmss}";
    }

    public static string GenerateTourCode(int destinationId, int seq)
    {
        return $"T-{destinationId:00}-{seq:000}";
    }
}