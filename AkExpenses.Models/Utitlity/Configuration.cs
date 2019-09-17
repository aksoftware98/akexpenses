using AkExpenses.Models.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AkExpenses.Models.Utitlity
{
    public class Configuration : IConfiguration
    {
        private readonly string _filePath = "";

        public Configuration(string filePath)
        {
            _filePath = filePath;
        }

        public string AccessToken { get; set; }
        public Dictionary<string, object> Dictionary { get; private set; }


        /// <summary>
        /// Load the settings into the file 
        /// </summary>
        public void LoadSettings()
        {
            if (!File.Exists(_filePath))
                File.WriteAllText(_filePath, JsonConvert.SerializeObject(this)); 

            using (var reader = new StreamReader(_filePath))
            {
                var settings = JsonConvert.DeserializeObject<Configuration>(reader.ReadToEnd());
                this.Dictionary = settings.Dictionary;
                this.AccessToken = settings.AccessToken; 
            }
        }

        /// <summary>
        /// Save the current settings into a json file 
        /// </summary>
        public void SaveSettings()
        {
            string jsonSettings = JsonConvert.SerializeObject(this);

            using (var writer = new StreamWriter(_filePath))
            {
                writer.Write(jsonSettings); 
            }
        }

        /// <summary>
        /// Set a new value into the application settigns 
        /// </summary>
        /// <param name="key">Setting key</param>
        /// <param name="value">Setting value</param>
        public void SaveValue(string key, object value)
        {
            if (Dictionary == null)
                Dictionary = new Dictionary<string, object>();

            Dictionary.Add(key, value);

            SaveSettings(); 
        }


    }
}
