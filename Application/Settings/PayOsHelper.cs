using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Settings
{
    public class PayOsHelper
    {
        private readonly string _checksumKey;

        public PayOsHelper(string checksumKey)
        {
            _checksumKey = checksumKey;
        }

        public string GenerateSignature(Dictionary<string, object> data)
        {
            var sortedKeys = data.Keys.OrderBy(k => k).ToList();
            var sb = new StringBuilder();

            for (int i = 0; i < sortedKeys.Count; i++)
            {
                var key = sortedKeys[i];
                var value = data[key];

                string valueStr = value switch
                {
                    DateTime dt => dt.ToString("yyyy-MM-dd HH:mm:ss"),
                    _ => value?.ToString() ?? ""
                };

                sb.Append($"{key}={valueStr}");
                if (i < sortedKeys.Count - 1)
                    sb.Append("&");
            }

            var rawData = sb.ToString();
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_checksumKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

    }

}
