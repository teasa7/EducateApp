using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EducateApp.Models.Data
{
    public class TypeOfTotal
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ИД")]
        public short Id { get; set; }

        [Required(ErrorMessage = "Введите название аттестации")]
        [Display(Name = "Название аттестации")]
        public string CertificateName { get; set; }

        [Display(Name = "Преподаватель")]
        [Required]
        public string IdUser { get; set; }


        [ForeignKey("IdUser")]
        public User User { get; set; }
    }
}
