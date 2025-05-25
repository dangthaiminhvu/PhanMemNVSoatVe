using PhanMemNVSoatVe.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhanMemNVSoatVe.Views
{
    public interface ISuCoView
    {
        event Action LoadData;
        event Action<SuCo> SaveRequested;
        event Action<int> DeleteRequested;

        void DisplaySuCoList(IEnumerable<SuCo> list);
        void ShowMessage(string message, string caption);
        SuCo GetSuCoFromForm();
        void PopulateForm(SuCo sc);
    }

}


