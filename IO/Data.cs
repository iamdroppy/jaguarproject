using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JaguarProject.IO
{
    class Data
    {
        public static int decodeB64(string Val)
        {
            char[] val = Val.ToCharArray();
            int intTot = 0;
            int y = 0;
            for (int x = (val.Length - 1); x >= 0; x--)
            {
                int intTmp = (int)(byte)((val[x] - 64));
                if (y > 0)
                {
                    intTmp = intTmp * (int)(Math.Pow(64, y));
                }
                intTot += intTmp;
                y++;
            }
            return intTot;
        }
    }
}
