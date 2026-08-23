using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TourManagement.Web.Models.Entities;
using TourManagement.Web.Models.Enums;

namespace TourManagement.Web.Data.Seed;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<int>> roleManager)
    {
        // Tạo roles
        string[] roles = { "ADMIN", "STAFF", "CUSTOMER" };
        foreach (var r in roles)
        {
            if (!await roleManager.RoleExistsAsync(r))
                await roleManager.CreateAsync(new IdentityRole<int>(r));
        }

        // Tạo tài khoản admin mặc định
        var adminEmail = "admin@tour.com";
        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin == null)
        {
            admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "Quản trị viên",
                PhoneNumber = "0900000000",
                Status = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(admin, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "ADMIN");
            }
        }

        var staffEmail = "staff@tour.com";
        var staff = await userManager.FindByEmailAsync(staffEmail);
        if (staff == null)
        {
            staff = new ApplicationUser
            {
                UserName = staffEmail,
                Email = staffEmail,
                FullName = "Nguyễn Văn Nhân Viên",
                PhoneNumber = "0900000001",
                Status = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(staff, "Staff@123");
            if (result.Succeeded) await userManager.AddToRoleAsync(staff, "STAFF");
        }

        var customerEmail = "customer@tour.com";
        var customer = await userManager.FindByEmailAsync(customerEmail);
        if (customer == null)
        {
            customer = new ApplicationUser
            {
                UserName = customerEmail,
                Email = customerEmail,
                FullName = "Trần Thị Khách Hàng",
                PhoneNumber = "0900000002",
                Status = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(customer, "Customer@123");
            if (result.Succeeded) await userManager.AddToRoleAsync(customer, "CUSTOMER");
        }

        // Seed destinations
        if (!await context.Destinations.AnyAsync())
        {
            var destinations = new List<Destination>
            {
                new() { Name = "Hạ Long", City = "Quảng Ninh", Country = "Việt Nam",
                    Description = "Vịnh Hạ Long - kỳ quan thiên nhiên thế giới với hàng nghìn đảo đá vôi",
                    Status = 1 },
                new() { Name = "Đà Lạt", City = "Lâm Đồng", Country = "Việt Nam",
                    Description = "Thành phố ngàn hoa với khí hậu mát mẻ quanh năm",
                    Status = 1 },
                new() { Name = "Phú Quốc", City = "Kiên Giang", Country = "Việt Nam",
                    Description = "Đảo ngọc với những bãi biển tuyệt đẹp",
                    Status = 1 },
                new() { Name = "Sapa", City = "Lào Cai", Country = "Việt Nam",
                    Description = "Thị trấn vùng cao với ruộng bậc thang và văn hóa dân tộc",
                    Status = 1 },
                new() { Name = "Nha Trang", City = "Khánh Hòa", Country = "Việt Nam",
                    Description = "Thành phố biển nổi tiếng với các resort cao cấp",
                    Status = 1 },
                new() { Name = "Hội An", City = "Quảng Nam", Country = "Việt Nam",
                    Description = "Phố cổ đèn lồng - di sản văn hóa thế giới",
                    Status = 1 },
                new() { Name = "Đà Nẵng", City = "Đà Nẵng", Country = "Việt Nam",
                    Description = "Thành phố đáng sống với biển Mỹ Khê tuyệt đẹp",
                    Status = 1 },
                new() { Name = "Bangkok", City = "Bangkok", Country = "Thái Lan",
                    Description = "Thủ đô sôi động của Thái Lan",
                    Status = 1 },
            };
            context.Destinations.AddRange(destinations);
            await context.SaveChangesAsync();
        }

        // Seed tours
        if (!await context.Tours.AnyAsync())
        {
            var destinations = await context.Destinations.OrderBy(d => d.Id).ToListAsync();
            var tours = new List<Tour>
            {
                new() {
                    DestinationId = destinations[0].Id,
                    Code = "T-01-001",
                    Name = "Tour Hạ Long 2N1Đ - Du thuyền cao cấp",
                    Description = "Khám phá vịnh Hạ Long với du thuyền 5 sao, thăm hang Sửng Sốt, đảo Ti Tốp",
                    DurationDays = 2, DurationNights = 1, BasePrice = 2500000,
                    IncludedServices = "Xe đưa đón, du thuyền, ăn 3 bữa, vé tham quan, hướng dẫn viên",
                    ExcludedServices = "Đồ uống, chi phí cá nhân, tip",
                    Policy = "Hủy trước 15 ngày hoàn 100%, trước 7 ngày hoàn 70%",
                    TourType = 1, Status = 1
                },
                new() {
                    DestinationId = destinations[1].Id,
                    Code = "T-02-001",
                    Name = "Tour Đà Lạt 3N2Đ - Thành phố mộng mơ",
                    Description = "Tham quan các điểm nổi tiếng: Hồ Xuân Hương, Thung lũng Tình Yêu, Langbiang",
                    DurationDays = 3, DurationNights = 2, BasePrice = 3200000,
                    IncludedServices = "Khách sạn 3*, xe đưa đón, ăn sáng, vé tham quan",
                    ExcludedServices = "Vé máy bay, ăn trưa/tối",
                    Policy = "Hủy trước 15 ngày hoàn 100%, trước 7 ngày hoàn 70%",
                    TourType = 1, Status = 1
                },
                new() {
                    DestinationId = destinations[2].Id,
                    Code = "T-03-001",
                    Name = "Tour Phú Quốc 4N3Đ - Thiên đường biển đảo",
                    Description = "Khám phá đảo ngọc với Bãi Sao, Hòn Thơm, VinWonders",
                    DurationDays = 4, DurationNights = 3, BasePrice = 5800000,
                    IncludedServices = "Resort 4*, xe đưa đón, ăn sáng, vé VinWonders",
                    ExcludedServices = "Vé máy bay, ăn trưa/tối",
                    Policy = "Hủy trước 15 ngày hoàn 100%, trước 7 ngày hoàn 70%",
                    TourType = 2, Status = 1
                },
                new() {
                    DestinationId = destinations[3].Id,
                    Code = "T-04-001",
                    Name = "Tour Sapa 3N2Đ - Ruộng bậc thang",
                    Description = "Trekking bản Cát Cát, Fansipan, chợ tình Sapa",
                    DurationDays = 3, DurationNights = 2, BasePrice = 3500000,
                    IncludedServices = "Khách sạn, xe đưa đón, ăn sáng, HDV địa phương",
                    ExcludedServices = "Cáp treo Fansipan, ăn trưa/tối",
                    Policy = "Hủy trước 15 ngày hoàn 100%, trước 7 ngày hoàn 70%",
                    TourType = 1, Status = 1
                },
                new() {
                    DestinationId = destinations[5].Id,
                    Code = "T-06-001",
                    Name = "Tour Hội An - Đà Nẵng 4N3Đ",
                    Description = "Phố cổ Hội An, Bà Nà Hills, Ngũ Hành Sơn",
                    DurationDays = 4, DurationNights = 3, BasePrice = 4500000,
                    IncludedServices = "Khách sạn 4*, xe, ăn sáng, vé Bà Nà",
                    ExcludedServices = "Vé máy bay, ăn trưa/tối",
                    Policy = "Hủy trước 15 ngày hoàn 100%, trước 7 ngày hoàn 70%",
                    TourType = 1, Status = 1
                },
                new() {
                    DestinationId = destinations[7].Id,
                    Code = "T-08-001",
                    Name = "Tour Bangkok - Pattaya 5N4Đ",
                    Description = "Khám phá Thái Lan với Bangkok sôi động và Pattaya biển xanh",
                    DurationDays = 5, DurationNights = 4, BasePrice = 8500000,
                    IncludedServices = "Khách sạn 4*, vé máy bay, HDV, ăn sáng",
                    ExcludedServices = "Hộ chiếu, ăn trưa/tối",
                    Policy = "Hủy trước 15 ngày hoàn 100%, trước 7 ngày hoàn 70%",
                    TourType = 2, Status = 1
                }
            };
            context.Tours.AddRange(tours);
            await context.SaveChangesAsync();

            // Itineraries
            foreach (var t in tours)
            {
                for (int d = 1; d <= t.DurationDays; d++)
                {
                    context.TourItineraries.Add(new TourItinerary
                    {
                        TourId = t.Id,
                        DayNumber = d,
                        Title = $"Ngày {d}",
                        Description = $"Hoạt động trong ngày {d} của tour",
                        Location = t.Name,
                        Meals = d == 1 ? "" : "Sáng, Trưa, Tối",
                        Hotel = d == t.DurationDays ? "" : "Khách sạn 3-4*",
                        TimeInfo = "07:00 - 21:00"
                    });
                }
            }
            await context.SaveChangesAsync();

            // Schedules
            var rng = new Random();
            foreach (var t in tours)
            {
                for (int i = 0; i < 4; i++)
                {
                    var start = DateTime.UtcNow.Date.AddDays(7 + i * 14);
                    var end = start.AddDays(t.DurationDays - 1);
                    context.TourSchedules.Add(new TourSchedule
                    {
                        TourId = t.Id,
                        Code = $"SCH-{t.Id}-{i + 1:000}",
                        StartDate = start,
                        EndDate = end,
                        MeetingTime = new TimeSpan(7, 30, 0),
                        MeetingPoint = "Sân bay Tân Sơn Nhất - Cổng D1",
                        MaxGuests = rng.Next(15, 30),
                        BookedGuests = rng.Next(0, 5),
                        Price = t.BasePrice + rng.Next(-200000, 500000),
                        Status = 1
                    });
                }
            }
            await context.SaveChangesAsync();
        }

        // Seed Guides
        if (!await context.Guides.AnyAsync())
        {
            var guides = new List<Guide>
            {
                new() { FullName = "Lê Văn Hùng", Phone = "0912345678", Email = "hung@tour.com",
                    Address = "Hà Nội", ExperienceYears = 8, Languages = "Tiếng Việt, Tiếng Anh", Status = 1 },
                new() { FullName = "Nguyễn Thị Mai", Phone = "0912345679", Email = "mai@tour.com",
                    Address = "TP.HCM", ExperienceYears = 5, Languages = "Tiếng Việt, Tiếng Anh, Tiếng Pháp", Status = 1 },
                new() { FullName = "Trần Văn Nam", Phone = "0912345680", Email = "nam@tour.com",
                    Address = "Đà Nẵng", ExperienceYears = 10, Languages = "Tiếng Việt, Tiếng Anh, Tiếng Trung", Status = 1 },
                new() { FullName = "Phạm Thị Hoa", Phone = "0912345681", Email = "hoa@tour.com",
                    Address = "Nha Trang", ExperienceYears = 6, Languages = "Tiếng Việt, Tiếng Anh", Status = 1 },
            };
            context.Guides.AddRange(guides);
            await context.SaveChangesAsync();
        }

        // Seed Promotions
        if (!await context.Promotions.AnyAsync())
        {
            context.Promotions.AddRange(
                new Promotion
                {
                    Code = "SUMMER2026",
                    Name = "Khuyến mãi hè 2026",
                    Description = "Giảm 15% cho tất cả tour mùa hè",
                    DiscountType = (int)DiscountType.PERCENT,
                    DiscountValue = 15,
                    MaxDiscount = 1000000,
                    MinOrderValue = 2000000,
                    StartAt = DateTime.UtcNow.AddDays(-10),
                    EndAt = DateTime.UtcNow.AddMonths(3),
                    UsageLimit = 100,
                    UsageCount = 0,
                    Status = 1
                },
                new Promotion
                {
                    Code = "NEW500K",
                    Name = "Giảm 500K cho khách mới",
                    Description = "Giảm cố định 500.000 VNĐ cho đơn từ 3 triệu",
                    DiscountType = (int)DiscountType.FIXED,
                    DiscountValue = 500000,
                    MaxDiscount = null,
                    MinOrderValue = 3000000,
                    StartAt = DateTime.UtcNow.AddDays(-5),
                    EndAt = DateTime.UtcNow.AddMonths(6),
                    UsageLimit = 200,
                    UsageCount = 0,
                    Status = 1
                },
                new Promotion
                {
                    Code = "VIP30",
                    Name = "VIP giảm 30%",
                    Description = "Giảm 30% cho đơn từ 5 triệu, tối đa 2 triệu",
                    DiscountType = (int)DiscountType.PERCENT,
                    DiscountValue = 30,
                    MaxDiscount = 2000000,
                    MinOrderValue = 5000000,
                    StartAt = DateTime.UtcNow.AddDays(-30),
                    EndAt = DateTime.UtcNow.AddMonths(1),
                    UsageLimit = 50,
                    UsageCount = 5,
                    Status = 1
                }
            );
            await context.SaveChangesAsync();
        }
    }
}