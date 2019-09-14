using System;
using System.Collections.Generic;
using System.Text;

namespace AkExpenses.Client.Interfaces
{
    public interface INavigationService
    {
        void NavigateTo(INavigationView view, params object[] parameters);
        void ShowDialog(INavigationView view, params object[] parameters);
        void ShowMessage(string title, string content);
        QuestionResponse ShowQuestion(string title, string content, QuestionType questionType);
    }

    public enum QuestionResponse
    {
        Yes, No, Cancel, Ignore
    }

    public enum QuestionType
    {
        YesNo, YesNoCancel, YesIgnore
    }

    public interface INavigationView
    {

    }

}