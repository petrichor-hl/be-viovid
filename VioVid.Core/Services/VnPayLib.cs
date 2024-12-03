using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace VioVid.Core.Services;

public class VnPayLib
{
    private readonly SortedList<string, string> _requestData = new(new VnPayCompare());

    public void AddRequestData(string key, string value)
    {
        if (!string.IsNullOrEmpty(value)) _requestData.Add(key, value);
    }

    public string CreateRequestUrl(string baseUrl, string vnp_HashSecret)
    {
        var data = new StringBuilder();
        foreach (var kv in _requestData)
            if (!string.IsNullOrEmpty(kv.Value))
                data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");

        var queryString = data.ToString();

        baseUrl += "?" + queryString;
        var signData = queryString;
        if (signData.Length > 0) signData = signData.Remove(data.Length - 1, 1);
        var vnp_SecureHash = HmacSHA512(vnp_HashSecret, signData);
        baseUrl += "vnp_SecureHash=" + vnp_SecureHash;

        return baseUrl;
    }

    public static string HmacSHA512(string key, string inputData)
    {
        Console.WriteLine("Input data: " + inputData);
        var hash = new StringBuilder();
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var inputBytes = Encoding.UTF8.GetBytes(inputData);
        using (var hmac = new HMACSHA512(keyBytes))
        {
            var hashValue = hmac.ComputeHash(inputBytes);
            foreach (var theByte in hashValue) hash.Append(theByte.ToString("x2"));
        }

        return hash.ToString();
    }
}

public class VnPayCompare : IComparer<string>
{
    public int Compare(string x, string y)
    {
        if (x == y) return 0;
        if (x == null) return -1;
        if (y == null) return 1;
        var vnpCompare = CompareInfo.GetCompareInfo("en-US");
        return vnpCompare.Compare(x, y, CompareOptions.Ordinal);
    }
}