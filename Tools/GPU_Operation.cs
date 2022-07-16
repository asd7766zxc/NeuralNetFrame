using ILGPU;
using ILGPU.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerualNetFrame.Tools
{
    public class GPU_Operation
    {
        public static Matrix MatrixMultiply(Matrix A, Matrix B)
        {
            using (var context = new Context())
            {
                var acceleratorId = Accelerator.Accelerators[1];

                using (var accelerator = Accelerator.Create(context, acceleratorId))
                {
                    var acceleratedResult = MatrixMultiplyAccelerated(accelerator, A._base, B._base);
                   // Console.WriteLine($"- Accelerated implementation on {accelerator}: {sw.ElapsedMilliseconds}ms");
                    return acceleratedResult.ToMatrix();
                }

            }
        }
        static double[,] MatrixMultiplyAccelerated(Accelerator accelerator, double[,] a, double[,] b)
        {
            var m = a.GetLength(0);
            var ka = a.GetLength(1);
            var kb = b.GetLength(0);
            var n = b.GetLength(1);

            if (ka != kb)
                throw new ArgumentException($"Cannot multiply {m}x{ka} matrix by {n}x{kb} matrix", nameof(b));

            var kernel = accelerator.LoadAutoGroupedStreamKernel<Index2, ArrayView2D<double>, ArrayView2D<double>, ArrayView2D<double>>(MatrixMultiplyAcceleratedKernel);

            using (var aBuffer = accelerator.Allocate<double>(m, ka))
            using (var bBuffer = accelerator.Allocate<double>(ka, n))
            using (var cBuffer = accelerator.Allocate<double>(m, n))
            {
                aBuffer.CopyFrom(a, Index2.Zero, Index2.Zero, aBuffer.Extent);
                bBuffer.CopyFrom(b, Index2.Zero, Index2.Zero, bBuffer.Extent);

                kernel(cBuffer.Extent, aBuffer, bBuffer, cBuffer);
                accelerator.Synchronize();

                return cBuffer.GetAs2DArray();
            }
        }
        static void MatrixMultiplyAcceleratedKernel(Index2 index, ArrayView2D<double> aView, ArrayView2D<double> bView, ArrayView2D<double> cView)
        {
            var x = index.X;
            var y = index.Y;
            var sum = 0.0;

            for (var i = 0; i < aView.Height; i++)
                sum += aView[new Index2(x, i)] * bView[new Index2(i, y)];

            cView[index] = sum;
        }
    }
}
