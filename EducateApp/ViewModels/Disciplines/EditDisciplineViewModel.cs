﻿using System.ComponentModel.DataAnnotations;

namespace EducateApp.ViewModels.Disciplines
{
    public class EditDisciplineViewModel
    {
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

        public string IdUser { get; set; }
    }
}