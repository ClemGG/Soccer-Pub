using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Clement.Utilities.Maths
{

    public static class Maths
    {

        /// <summary>
        /// Pareil que le Mathf.Approximately, mais celui-là permet d'utiliser un intervalle de sensibilité. Utile quand on ne sait pas si a sera plus grand ou plus petit que b.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public static bool FastApproximate(float a, float b, float threshold)
        {
            return ((a < b) ? (b - a) : (a - b)) <= threshold;
        }


        /// <summary>
        /// Similaire à FastApproximately, mais ne s'utilise que si a est strictement plus grand que b.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public static bool FastApproximatelyWithFirstArgumentAsSuperiorStrict(float a, float b, float threshold)
        {
            return (a - b) <= threshold;
        }


        /// <summary>
        /// Similaire à FastApproximately, mais ne s'utilise que si a est strictement plus petit que b.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public static bool FastApproximatelyWithFirstArgumentAsInferiorStrict(float a, float b, float threshold)
        {
            return (b - a) <= threshold;
        }
    }
}