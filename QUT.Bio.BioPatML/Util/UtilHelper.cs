using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace QUT.Bio.BioPatML.Util
{
    public static class UtilHelper
    {
        /// <summary>
        /// Converts each element in an enumeration into a string, then uses the
        /// separator string to glue them togehter to form a single string.
        /// </summary>
        /// <param name="objectList">A list of objects to be joined.</param>
        /// <param name="separater">A separator string that will be inserted between the items.</param>
        /// <returns>A single string containing the concatenated set of string representations.</returns>

        public static string Join(IEnumerable objectList, string separater)
        {
            StringBuilder b = new StringBuilder();
            bool dejaVu = false;

            lock (objectList)
            {
                foreach (object x in objectList)
                {
                    b.AppendFormat("{0}{1}", (dejaVu ? separater : ""), x);
                    dejaVu = true;
                }
            }

            return b.ToString();
        }

        internal static object Join(string p)
        {
            throw new NotImplementedException();
        }
    }
}
