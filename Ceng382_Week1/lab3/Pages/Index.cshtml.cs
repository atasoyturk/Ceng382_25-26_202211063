using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using lab3.Data;
using lab3.Models.Media;

namespace lab3.Pages;

public class UserViewModel
{
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? PhotoBase64 { get; set; }
    public string? PhotoContentType { get; set; }
}

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public IndexModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public List<Student> Students { get; set; } = new();
    public List<UserViewModel> AppUsers { get; set; } = new();
    public List<ImageModel> GalleryImages { get; set; } = new();

    [BindProperty]
    public IFormFile? Upload { get; set; }

    public void OnGet()
    {
        LoadDummyData();
        LoadUsers();
        LoadGallery();
    }

    public void OnPostCalculate()
    {
        LoadDummyData();
        foreach (var student in Students)
        {
            double avg = (student.Midterm * 0.4) + (student.Final * 0.6);
            student.Grade = CalculateGrade(avg);
        }
        LoadUsers();
        LoadGallery();
    }

    public async Task<IActionResult> OnPostUploadAsync()
    {
        if (Upload != null && Upload.Length > 0)
        {
            using var ms = new MemoryStream();
            await Upload.CopyToAsync(ms);

            var image = new ImageModel
            {
                FileName = Upload.FileName,
                ContentType = Upload.ContentType,
                Size = Upload.Length,
                Data = ms.ToArray()
            };

            _db.Images.Add(image);
            await _db.SaveChangesAsync();
        }

        LoadDummyData();
        LoadUsers();
        LoadGallery();
        return Page();
    }

    private void LoadDummyData()
    {
        Students = new List<Student>
        {
            new Student { Id = 1, Name = "Ata",  Midterm = 70, Final = 20 },
            new Student { Id = 2, Name = "Efe",  Midterm = 55, Final = 60 },
            new Student { Id = 3, Name = "Mert", Midterm = 90, Final = 75 }
        };
    }

    private void LoadUsers()
    {
        AppUsers = _db.Users
            .Select(u => new UserViewModel
            {
                UserName = u.UserName,
                Email = u.Email,
                PhotoBase64 = u.ProfilePhoto != null ? Convert.ToBase64String(u.ProfilePhoto) : null,
                PhotoContentType = u.ProfilePhotoContentType
            })
            .ToList();
    }

    private void LoadGallery()
    {
        GalleryImages = _db.Images.ToList();
    }

    private string CalculateGrade(double avg)
    {
        if (avg >= 85) return "AA";
        if (avg >= 70) return "BB";
        if (avg >= 50) return "CC";
        return "FF";
    }
}
