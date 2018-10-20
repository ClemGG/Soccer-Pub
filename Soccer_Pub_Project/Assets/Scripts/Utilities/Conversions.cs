using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Clement.Utilities.Conversions
{
    public static class Conversions {



        /// <summary>
        /// Si BoolTo1AndMinus1 est à true, la fonction renverra -1 au lieu de 0 si la string passée en paramètre est une booléenne
        /// </summary>
        /// <param name="str"></param>
        /// <param name="BoolTo1AndMinus1"></param>
        /// <returns></returns>
        public static int ToInt(this string str, bool BoolTo1AndMinus1)
        {
            if (BoolTo1AndMinus1)
            {
                if (str == "true" || str == "True")
                {
                    return 1;
                }
                if (str == "false" || str == "False")
                {
                    return -1;
                }
            }
            else
            {
                if (str == "true" || str == "True")
                {
                    return 1;
                }
                if (str == "false" || str == "False")
                {
                    return 0;
                }
            }

            return int.Parse(str);
        }
        
        
        /// <summary>
        /// Si BoolTo1AndMinus1 est à true, la fonction renverra -1 au lieu de 0 si la string passée en paramètre est une booléenne
        /// </summary>
        /// <param name="b"></param>
        /// <param name="BoolTo1AndMinus1"></param>
        /// <returns></returns>
        public static int ToInt(this bool b, bool BoolTo1AndMinus1)
        {
            if (BoolTo1AndMinus1)
            {
                if (b)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                if (b)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}

