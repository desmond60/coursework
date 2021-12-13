namespace Project
{
    public struct Matrix
    {
        public int N;
        public int maxIter;
        public double epsilon;
        public uint[] ig, jg;  
        public double[] di, gg, pr, x, absolut_x;

        //* Умножение матрицы на вектор
        public double[] mult(double[] x) {
            var y = new double[x.Length];

            for (uint i = 0, jj = 0; i < x.Length; i++) {
                y[i] = di[i] * x[i];

                for (uint j = ig[i]; j < ig[i + 1]; j++, jj++) {
                   y[i]      += gg[jj] * x[jg[jj]];
                   y[jg[jj]] += gg[jj] * x[i];
                }
            }
            return y;
        }
    }

    public static class Helper
    {
        //* Сумма матриц
        public static double[][] SummMatrix(double[][] Mat1, double[][] Mat2)
        {   
            var Mat3 = new double[3][];
            for (uint i = 0; i < 3; i++) Mat3[i] = new double[3];

            for (uint i = 0; i < Mat1.Length; i++)    
                for (uint j = 0; j < Mat1.Length; j++)
                    Mat3[i][j] = Mat1[i][j] + Mat2[i][j];
            return Mat3;
        }

        //* Середина ребра (между двумя узлами)
        public static double[] MidPoints(double[] point1, double[] point2)
        {
            var midpoint = new double[2];
            midpoint[0] = (point1[0] + point2[0]) / 2.0;
            midpoint[1] = (point1[1] + point2[1]) / 2.0;
            return midpoint;
        }

        //* Вычисление скалярного произведение 
        public static double scalar(double[] array1, double[] array2) {
            double res = 0;
            for (uint i = 0; i < array1.Length; i++)
                res += array1[i] * array2[i];
            return res;
        }
    }
}