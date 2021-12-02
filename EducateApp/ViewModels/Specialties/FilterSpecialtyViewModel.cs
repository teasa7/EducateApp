using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Texnicum.ViewModels.Specialties
{
    public class FilterSpecialtyViewModel
    {
        public string SelectedCode { get; private set; }   
        public string SelectedName { get; private set; }    

        public SelectList FormOfStudies { get; private set; } 
        public short? FormOfEdu { get; private set; }   


        public FilterSpecialtyViewModel(string code, string name,
            List<FormOfStudy> formOfStudies, short? formOfEdu)
        {
            SelectedCode = code;
            SelectedName = name;

 
            formOfStudies.Insert(0, new FormOfStudy { FormOfEdu = "", Id = 0 });

            FormOfStudies = new SelectList(formOfStudies, "Id", "FormOfEdu", formOfEdu);
            FormOfEdu = formOfEdu;
        }
    }
}