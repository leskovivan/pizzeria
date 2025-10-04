using System.ComponentModel.DataAnnotations;

namespace pizzeria.ViewModels
{
    public class OrderViewModel : OrderViewModelAuthenticated
    {
        [Required(ErrorMessage = "Введите ФИО")]
        [Display(Name = "Фио")]
        public string? Fio { get; set; }

        [Required(ErrorMessage = "Введите телефон")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Некорректный телефон")]
        [Display(Name = "Телефон")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Введите емейл")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Не корректный емейл")]
        [Display(Name = "Email")]
        public string? Email { get; set; }
    }
}
