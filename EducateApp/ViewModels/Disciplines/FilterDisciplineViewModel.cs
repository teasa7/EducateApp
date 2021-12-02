namespace EducateApp.ViewModels.Disciplines
{
    public class FilterDisciplineViewModel
    {
        public string SelectedIndexProfModule { get; private set; }   
        public string SelectedProfModule { get; private set; }    
        public string SelectedShortName { get; private set; }


        public FilterDisciplineViewModel(string index, string module, string name)
        {
            SelectedIndexProfModule = index;
            SelectedProfModule = module;
            SelectedShortName = name;   
        }
    }
}
