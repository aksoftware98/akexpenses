using AkExpenses.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkExpenses.WPF.Services
{
    public class WPFNavigationService : INavigationService
    {
        public void NavigateTo(INavigationView view, params object[] parameters)
        {
            
        }

        public void ShowDialog(INavigationView view, params object[] parameters)
        {
            
        }

        public void ShowMessage(string title, string content)
        {
            throw new NotImplementedException();
        }

        public QuestionResponse ShowQuestion(string title, string content, QuestionType questionType)
        {
            throw new NotImplementedException();
        }
    }
}
