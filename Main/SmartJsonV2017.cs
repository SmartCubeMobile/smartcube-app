using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace SmartCubeMobile
{
    public static class SmartJsonV2017
    {
        /// <summary>
        /// JSON Serialization
        /// </summary>
        internal static string JsonSerializer<T>(T t)
        {
            string jsonString = string.Empty;
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream())
            {
                //MemoryStream ms = new MemoryStream();
                ser.WriteObject(ms, t);
                jsonString = Encoding.UTF8.GetString(ms.ToArray());
                //return jsonString;
            }
            //ms.Close();
            return jsonString;
        }
    }
}
