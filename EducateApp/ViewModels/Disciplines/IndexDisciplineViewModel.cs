﻿using EducateApp.Models.Data;
using System.Collections.Generic;

namespace EducateApp.ViewModels.Disciplines
{
    public class IndexDisciplineViewModel
    {
        public IEnumerable<Models.Data.Disciplines> Disciplines { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public FilterDisciplineViewModel FilterDisciplineViewModel { get; set; }
        public SortDisciplineViewModel SortDisciplineViewModel { get; set; }
    }
}
