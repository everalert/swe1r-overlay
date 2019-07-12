using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWE1R.Util
{
    static class Helper
    {
        public static List<String> ArrayToStrList(Array input)
        {
            var output = new List<String>();
            foreach (float item in input)
                output.Add(item.ToString());
            return output;
        }

        public static string[] FormatTimesArray( float[] input, string format )
        {
            var output = new string[input.Length];
            for (var i = 0; i < input.Length; i++)
                output[i] = TimeSpan.FromSeconds(input[i]).ToString(format);
            return output;
        }

        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        public static float ByteToFloat(byte b)
        {
            return b * 255;
        }
    }
}
