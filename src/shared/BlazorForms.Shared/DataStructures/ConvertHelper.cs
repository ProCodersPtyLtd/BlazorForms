using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Shared
{
    public static class ConvertHelper
    {
        public static string[][] ConvertToJaggedArray(string[,] arr)
        {
            int y = arr.GetLength(0);
            int x = arr.GetLength(1);

            string[][] r = new string[y][];

            for (int j = 0; j < y; j++)
            {
                r[j] = new string[x];

                for (int i = 0; i < x; i++)
                {
                    r[j][i] = arr[j, i];
                }
            }

            return r;
        }

        public static string[][] ConvertToJaggedArray(List<string> list, int width)
        {
            if (width == 0 || list.Count == 0)
            {
                return new string[0][];
            }

            var y = (int)(list.Count / width);
            int x = width;
            int index = 0;

            string[][] r = new string[y][];

            for (int j = 0; j < y; j++)
            {
                r[j] = new string[x];

                for (int i = 0; i < x; i++)
                {
                    r[j][i] = list[index];
                    index++;
                }
            }

            return r;
        }
    }
}
