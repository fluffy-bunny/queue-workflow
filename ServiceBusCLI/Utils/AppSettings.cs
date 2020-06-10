using Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ServiceBusCLI.Utils
{
    public class AppSettings<T> where T : class
    {
        public AppSettings(ISerializer serializer)
        {
            _serializer = serializer;
        }
        private const string DEFAULT_FILENAME = "settings.json";
        private ISerializer _serializer;

        public void Save(T pSettings, string fileName = DEFAULT_FILENAME)
        {
            File.WriteAllText(fileName, _serializer.Serialize<T>(pSettings));
        }

        public T Load(string fileName = DEFAULT_FILENAME)
        {
            if (File.Exists(fileName))
                return _serializer.Deserialize<T>(File.ReadAllText(fileName));
            return null;
        }
    }
}
