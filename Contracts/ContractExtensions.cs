using Newtonsoft.Json;

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
        public static string Base64Encode<T>(this T obj) where T: class
        {
            var json = JsonConvert.SerializeObject(obj);
            var encoded = json.Base64Encode();
            return encoded;
        }
        public static T Base64Decode<T>(this string base64EncodedData) where T : class
        {
            var json = base64EncodedData.Base64Decode();
            var obj = JsonConvert.DeserializeObject<T>(json);
            return obj;
        }

    }
}
