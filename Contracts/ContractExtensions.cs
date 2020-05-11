using Newtonsoft.Json;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Contracts
{
    public static class ContractExtensions
    {
        public static string Base64Decode(this string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        public static string Base64Encode(this string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Encode<T>(this T obj) 
            where T: class
        {
            var json = obj.ToJson();
            var encoded = json.Base64Encode();
            return encoded;
        }
        public static byte[] ToByteArray<T>(this T obj) 
            where T : class
        {
            var formatter = new BinaryFormatter();
            byte[] byteMessage;
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, obj);
                byteMessage = stream.ToArray();
            }
            return byteMessage;
        }
        public static string ToJson<T>(this T obj) where T: class
        {
            var json = JsonConvert.SerializeObject(obj);
            return json;
        }
        public static T FromJson<T>(this string json)
            where T : class
        {
            var obj = JsonConvert.DeserializeObject<T>(json);
            return obj;
        }

        public static T Base64Decode<T>(this string base64EncodedData) 
            where T : class
        {
            var json = base64EncodedData.Base64Decode();
            var obj = json.FromJson<T>();
            return obj;
        }
        public static T ToObj<T>(this byte[] byteArray)
            where T:class
        {
            Stream stream = new MemoryStream(byteArray);
            BinaryFormatter formatter = new BinaryFormatter();

            // Deserialize the hashtable from the file and
            // assign the reference to the local variable.
            var obj = (T)formatter.Deserialize(stream);
            return obj;
        }

    }
}
