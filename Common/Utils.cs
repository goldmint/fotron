using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net;


namespace Fotron.Common
{
    public static class Utils
    {

        public static async Task<dynamic> DownloadDynamicObj(string url, bool singleObj = true)
        {

            using (var webClient = new CustomWebClient())
            {

                try
                {
                    var json = await webClient.DownloadStringTaskAsync(url);

                    try
                    {
                        return JObject.Parse(json);
                    }
                    catch
                    {
                        var obj = JArray.Parse(json);
                        return singleObj ? obj[0] : obj;
                    }

                }
                catch
                {
                    return null;
                }

            }
        }


        public static double ParceDouble(string str)
        {
            double.TryParse(str?.Trim().Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture,
                out double res);

            return res;
        }

        public static decimal ParceDecimal(string str)
        {
            decimal.TryParse(str?.Trim().Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture,
                out decimal res);

            return res;
        }

        public static string GetDescription<T>(this T e) where T : IConvertible
        {
            string description = null;

            if (e is Enum)
            {
                Type type = e.GetType();
                Array values = System.Enum.GetValues(type);

                foreach (int val in values)
                {
                    if (val == e.ToInt32(CultureInfo.InvariantCulture))
                    {
                        var memInfo = type.GetMember(type.GetEnumName(val));
                        var descriptionAttributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                        if (descriptionAttributes.Length > 0)
                        {
                            // we're only getting the first description we find
                            // others will be ignored
                            description = ((DescriptionAttribute)descriptionAttributes[0]).Description;
                        }

                        break;
                    }
                }
            }

            return description;
        }

        public static DateTime UnixTimestampToDateTime(double unixTime)
        {
            DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            long unixTimeStampInTicks = (long)(unixTime * TimeSpan.TicksPerSecond);
            return new DateTime(unixStart.Ticks + unixTimeStampInTicks, System.DateTimeKind.Utc);
        }

        public static double ToUnixTimestamp(this DateTime dateTime)
        {
            DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            long unixTimeStampInTicks = (dateTime.ToUniversalTime() - unixStart).Ticks;
            return (double)unixTimeStampInTicks / TimeSpan.TicksPerSecond;
        }


        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static bool In<T>(this T val, params T[] values) where T : struct
        {
            return values.Contains(val);
        }

        public static T GetProbabilitySelection<T>(this Dictionary<T, double> dict)
        {
            var diceRoll = new Random().NextDouble();

            var cumulative = 0.0;

            foreach (var item in dict)
            {
                cumulative += item.Value;
                if (diceRoll <= cumulative) return item.Key;
            }

            return default(T);
        }

        public static double RoundUp(this double input, int places)
        {
            var multiplier = Math.Pow(10, Convert.ToDouble(places));
            return Math.Ceiling(input * multiplier) / multiplier;
        }

        public static decimal RoundUp(this decimal input, int places)
        {
            return (decimal)((double)input).RoundUp(places);
        }

        public static string ToInvariantString(this double d)
        {
            var s = d.ToString("F18", CultureInfo.InvariantCulture).TrimEnd('0');
            if (s.EndsWith('.')) s += "0";

            return s;
        }
    }
}