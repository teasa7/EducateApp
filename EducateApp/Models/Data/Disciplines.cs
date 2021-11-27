using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EducateApp.Models.Data
{
    public class Disciplines
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ИД")]
        public short Id { get; set; }

        [Required(ErrorMessage = "Введите индекс профессионального модуля")]
        [Display(Name = "Индекс профессионального модуля")]
        public string IndexProfModule { get; set; }

        [Required(ErrorMessage = "Введите название профессионального модуля")]
        [Display(Name = "Название профессионального модуль")]
        public string ProfModule { get; set; }

        [Display(Name = "Индекс")]
        public string Index { get; set; }

        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Краткое название")]
        public string ShortName { get; set; }

        [Display(Name = "Преподаватель")]
        [Required]
        public string IdUser { get; set; }

        [ForeignKey("IdUser")]
        public User User { get; set; }


    }
}
