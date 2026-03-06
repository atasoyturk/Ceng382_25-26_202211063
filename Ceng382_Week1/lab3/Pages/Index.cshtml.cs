using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace lab3.Pages;

public class IndexModel : PageModel
{
    public List<Student> Students { get; set; } = new List<Student>();
    public void OnGet()
    {
        LoadDummyData();
    }

    public void OnPostCalculate()
    {
        LoadDummyData(); 

        foreach (var student in Students)
        {
            double average = (student.Midterm * 0.4) + (student.Final * 0.6);
            student.Grade = CalculateGrade(average);
        }
    }

    private void LoadDummyData()
    {
        Students = new List<Student>
        {
            new Student { Id = 1, Name = "Ata", Midterm = 70, Final = 20 },
            new Student { Id = 2, Name = "Efe", Midterm = 55, Final = 60 },
            new Student { Id = 3, Name = "Mert", Midterm = 90, Final = 75 }
        };
    }

    private string CalculateGrade(double avg)
    {
        if (avg >= 85) return "AA";
        if (avg >= 70) return "BB";
        if (avg >= 50) return "CC";
        return "FF";
    }
}
