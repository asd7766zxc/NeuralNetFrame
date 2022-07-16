using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerualNetFrame.Tools
{
    public static class MNIST
    {
        public static List<Matrix>[] Training_DataSet()
        {
            List<Matrix>[] matrices = new List<Matrix>[2];
            List<Matrix> _train = new List<Matrix>();
            List<Matrix> _test = new List<Matrix>();
            string path = Directory.GetCurrentDirectory();
            var imgs = File.ReadAllBytes(path + @"\DataSet\train\train-images.idx3-ubyte");
            var labels = File.ReadAllBytes(path + @"\DataSet\train\train-labels.idx1-ubyte");
            Console.WriteLine("     -- Images --");
            List<string> info = new List<string>()
            {
                "magic number",
                "number of images",
                "number of rows",
                "number of columns",
            };
            //前面都info
            // img data 在 0016 開始 offset 16 
            List<byte> infa = new List<byte>();
            for (int i = 0; i < 16; i++)
            {
                infa.Add(imgs[i]);
            }
            var imgset = imgs.ToList();
            imgset.RemoveRange(0, 16);
            for (int i = 0; i < 4; i++)
            {
                var a = infa.Chunk(4).ToList()[i];
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(a);
                int u = BitConverter.ToInt32(a, 0);
                Console.WriteLine("    " + info[i] + " " + u);
            }
            Console.WriteLine();
            Console.WriteLine("     -- Label --");
            List<string> info1 = new List<string>()
            {
                "magic number",
                "number of items",
            };
            List<byte> infa1 = new List<byte>();
            // label data 在 0008 開始 offset 8
            for (int i = 0; i < 8; i++)
            {
                infa1.Add(labels[i]);
            }
            var labelset = labels.ToList();
            labelset.RemoveRange(0, 8);
            for (int i = 0; i < 2; i++)
            {
                var a = infa1.Chunk(4).ToList()[i];
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(a);
                int u = BitConverter.ToInt32(a, 0);
                Console.WriteLine("    " + info1[i] + " " + u);
            }

            var temp = imgset.Chunk(784).ToList();
            for(int i = 0; i < temp.Count; i++)
            {
                Matrix tp = new Matrix(784, 1);
                for (int j = 0; j < 784; j++)
                {
                    tp[j,0] = (double)temp[i][j]/255.0;
                }
                _train.Add(tp);
            }

            for (int i = 0; i < labelset.Count; i++)
            {
                Matrix tp = new Matrix(10, 1);
                tp[labelset[i],0] = 1;
                _test.Add(tp);
            }
            matrices[0] = _train;
            matrices[1] = _test;
            return matrices;
        }
    }
}
