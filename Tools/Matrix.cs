using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerualNetFrame.Tools
{
    [Serializable]
    public struct Matrix
    {
        public static Matrix Instance { get; set; }
        public static Matrix Indentity(int m)
        {
            Matrix _matrix = new Matrix(m);
            for (int i = 0; i < m; i++)
            {
                _matrix[i, i] = 1;
            }
            return _matrix;
        }
        public double[,] _base = new double[0, 0];
        public int Rows
        {
            get => _base.GetLength(0);
        }
        public int Columns
        {
            get => _base.GetLength(1);
        }
        public double this[int row, int column]
        {
            get => _base[row, column];
            set => _base[row, column] = value;
        }
        public void Scaling(int m, int k)
        {
            _base = (double[,])_base.ResizeArray(new int[] { m, k });
        }
        public Matrix()
        {

        }
        public Matrix(int m, int k)
        {
            Scaling(m, k);
        }
        public Matrix(int m)
        {
            Scaling(m, m);
        }
        //Multiply By Columns
        public static Matrix operator &(Matrix A, Matrix B)
        {
            if (A.Columns != B.Columns || A.Rows != B.Rows)
                return Matrix.Instance;

            int _rows = A.Rows;
            int _columns = B.Columns;
            Matrix extract = new Matrix(_rows, _columns);
            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _columns; j++)
                {
                    extract[i, j] = A[i, j] * B[i, j];
                }
            }
            return extract;
        }
        //Multiply 
        public static Matrix operator *(Matrix A, Matrix B)
        {
            if (A.Columns != B.Rows)
                return Matrix.Instance;

            int _rows = A.Rows;
            int _columns = B.Columns;
            Matrix extract = new Matrix(_rows, _columns);
            Parallel.For(0, _rows ,i =>
            {

                for (int j = 0; j < _columns; j++)
                {
                    for (int k = 0; k < A.Columns; k++)
                        extract[i, j] += A[i, k] * B[k, j];
                }

            });
            return extract;
        }
        //Add
        public static Matrix operator +(Matrix A, Matrix B)
        {
            if (A.Rows != B.Rows && A.Columns != B.Columns)
                return Matrix.Instance;

            int _rows = A.Rows;
            int _columns = A.Columns;
            Matrix extract = new Matrix(_rows, _columns);
            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _columns; j++)
                {
                    extract[i, j] = A[i, j] + B[i, j];
                }
            }
            return extract;
        }
        //Substrac
        public static Matrix operator -(Matrix A, Matrix B)
        {
            if (A.Rows != B.Rows && A.Columns != B.Columns)
                return Matrix.Instance;

            int _rows = A.Rows;
            int _columns = A.Columns;
            Matrix extract = new Matrix(_rows, _columns);
            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _columns; j++)
                {
                    extract[i, j] = A[i, j] - B[i, j];
                }
            }
            return extract;
        }
        //Coefficient
        public static Matrix operator *(double r, Matrix B)
        {
            int _rows = B.Rows;
            int _columns = B.Columns;
            Matrix extract = new Matrix(_rows, _columns);
            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _columns; j++)
                {
                    extract[i, j] = r * B[i, j];
                }
            }
            return extract;
        }
        public Matrix Transpose()
        {
            Matrix result = new Matrix(this.Columns, this.Rows);
            for (int i = 0; i < this.Rows; i++)
            {
                for (int j = 0; j < this.Columns; j++)
                {
                    result[j, i] = this[i, j];
                }
            }
            return result;
        }
       
        /*
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i< this.Rows; i++)
            {
                for(int j =0; j< this.Columns; j++)
                {
                    sb.Append(this[i,j].ToString()+" ");
                }
                sb.Append('\n');
            }

            return sb.ToString();
        }*/
    }
}
