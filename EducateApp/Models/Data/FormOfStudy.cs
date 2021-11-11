using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EducateApp.Models.Data
{
    public class FormOfStudy
    {
        
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ИД")]
        public short Id { get; set; }

        [Required(ErrorMessage = "Введите название формы обучения")]
        [Display(Name = "Форма обучения")]
        public string FormOfEdu { get; set; }
        
        [Required]
        public string IdUser { get; set; }

        [ForeignKey("IdUser")]
        public User User { get; set; }
    }
}
