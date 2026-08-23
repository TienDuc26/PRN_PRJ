namespace TourManagement.Web.Helpers;

public static class FileUploadHelper
{
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

    public static async Task<string?> SaveImageAsync(IFormFile? file, string folder)
    {
        if (file == null || file.Length == 0) return null;
        if (file.Length > MaxFileSize) throw new InvalidOperationException("File vượt quá 5MB");
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext)) throw new InvalidOperationException("Định dạng ảnh không hợp lệ");

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", folder);
        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(uploadsFolder, fileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }
        return $"/uploads/{folder}/{fileName}";
    }

    public static void DeleteImage(string? path)
    {
        if (string.IsNullOrEmpty(path)) return;
        var physical = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", path.TrimStart('/'));
        if (File.Exists(physical)) File.Delete(physical);
    }
}