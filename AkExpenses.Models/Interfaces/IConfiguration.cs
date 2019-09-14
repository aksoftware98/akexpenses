using System;
using System.Collections.Generic;
using System.Text;

namespace AkExpenses.Models.Interfaces
{
    public interface IConfiguration
    {

        string AccessToken { get; set; }

        Dictionary<string, object> Dictionary { get; }

        void SaveValue(string key, object value);

        void SaveSettings();

        void LoadSettings(); 

    }
}
