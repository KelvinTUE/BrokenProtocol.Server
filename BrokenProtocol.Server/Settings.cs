using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace BrokenProtocol.Server
{
    public class Settings
    {
        private const string SETTINGS_FILE = "Settings";
        
        public int Port { get; set; } = 8080;


        #region BoilerPlate

        private static Settings _instance = null;
        public static Settings Instance => _instance ??= Load();

        public void Save()
        {
            File.WriteAllText(SETTINGS_FILE, JsonSerializer.Serialize(this));
        }

        public static Settings Load()
        {
            if(File.Exists(SETTINGS_FILE))
                return JsonSerializer.Deserialize<Settings>(File.ReadAllText(SETTINGS_FILE));
            return new Settings();
        }

        #endregion
    }
}
