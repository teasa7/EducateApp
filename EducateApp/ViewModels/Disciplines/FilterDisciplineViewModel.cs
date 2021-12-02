namespace EducateApp.ViewModels.Disciplines
{
    public class FilterDisciplineViewModel
    {
        public string SelectedIndexProfModule { get; private set; }    
        public string SelectedProfModule { get; private set; }   
        public string SelectedIndex { get; private set; }    
        public string SelectedName { get; private set; }   
        public string SelectedShortName { get; private set; }   

        public FilterDisciplineViewModel(string IndexProfModule, string ProfModule, string Index, string name, string ShortName)
        {
            SelectedIndexProfModule = IndexProfModule;
            SelectedProfModule = ProfModule;
            SelectedIndex = Index;
            SelectedName = name;
            SelectedShortName = ShortName;
        }
    }
}
