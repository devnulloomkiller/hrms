using System.ComponentModel.DataAnnotations;

namespace Hrms.Web.ViewModels;

public sealed class CandidateFormViewModel
{
    public int Id { get; set; }

    [Display(Name = "Фамилия")]
    [Required(ErrorMessage = "Укажите фамилию")]
    [MaxLength(100)]
    public string LastName { get; set; } = "";

    [Display(Name = "Имя")]
    [Required(ErrorMessage = "Укажите имя")]
    [MaxLength(100)]
    public string FirstName { get; set; } = "";

    [Display(Name = "Email")]
    [Required(ErrorMessage = "Укажите email")]
    [EmailAddress(ErrorMessage = "Похоже, в email опечатка")]
    [MaxLength(256)]
    public string Email { get; set; } = "";

    [Display(Name = "Телефон")]
    [MaxLength(40)]
    public string? Phone { get; set; }

    [Display(Name = "Желаемая должность")]
    [Required(ErrorMessage = "Укажите желаемую должность")]
    [MaxLength(200)]
    public string Headline { get; set; } = "";

    [Display(Name = "О кандидате")]
    [Required(ErrorMessage = "Добавьте короткое описание")]
    [MaxLength(2000)]
    public string Summary { get; set; } = "";

    [Display(Name = "Опыт, лет")]
    [Range(0, 60, ErrorMessage = "Опыт должен быть от 0 до 60 лет")]
    public int ExperienceYears { get; set; }

    [Display(Name = "Желаемая зарплата")]
    [Range(0, 10_000_000, ErrorMessage = "Укажите сумму в рублях")]
    public decimal? DesiredSalary { get; set; }

    [Display(Name = "Образование")]
    [MaxLength(500)]
    public string? Education { get; set; }

    [Display(Name = "Навыки")]
    [MaxLength(1000)]
    public string? Skills { get; set; }

    [Display(Name = "Ссылка на файл резюме")]
    [MaxLength(1000)]
    public string? FilePath { get; set; }
}
