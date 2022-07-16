using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerualNetFrame.Tools
{
    public static class Extensions 
    {
        public static Matrix ToMatrix(this double[,] base_)
        {
            Matrix A = new Matrix();
            A._base = base_;
            return A;
        }
        public static void Resize<T>(this List<T> list, int sz, T c)
        {
            int cur = list.Count;
            if (sz < cur)
                list.RemoveRange(sz, cur - sz);
            else if (sz > cur)
            {
                //this bit is purely an optimisation, to avoid multiple automatic capacity changes.
                if (sz > list.Capacity)
                    list.Capacity = sz;
                list.AddRange(Enumerable.Repeat(c, sz - cur));
            }
        }
        public static void Resize<T>(this List<T> list, int size) where T : new()
        {
            Resize(list, size, new T());
        }

        public static Array ResizeArray(this Array arr, int[] newSizes)
        {
            if (newSizes.Length != arr.Rank)
                throw new ArgumentException("arr must have the same number of dimensions " +
                                            "as there are elements in newSizes", "newSizes");

            var temp = Array.CreateInstance(arr.GetType().GetElementType(), newSizes);
            int length = arr.Length <= temp.Length ? arr.Length : temp.Length;
            Array.ConstrainedCopy(arr, 0, temp, 0, length);
            return temp;
        }
    }
}
