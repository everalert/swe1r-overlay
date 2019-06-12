using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWE1R_Overlay.Utilities
{
    class Helper
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
    }
}
