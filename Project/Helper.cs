using System.Text;

namespace Project
{
    struct Matrix
    {
        public int N;
        public int[] ig, jg;  
        public double[] di, gg, pr;  
    }

    public static class Helper
    {
        //public static Encoding code65001 = CodePagesEncodingProvider.Instance.GetEncoding(65001);

        //* Сумма двух матриц
        public static double[][] SummMatrix(double[][] Mat1, double[][] Mat2)
        {   
            double[][] Mat3 = new double[3][];
            for (int i = 0; i < 3; i++) Mat3[i] = new double[3];

            for (int i = 0; i < Mat1.Length; i++)    
                for (int j = 0; j < Mat1.Length; j++)
                    Mat3[i][j] = Mat1[i][j] + Mat2[i][j];
            return Mat3;
        }

        //* Середина ребра (между двумя точками)
        public static double[] MidPoints(double[] point1, double[] point2)
        {
            double[] midpoint = new double[2];
            midpoint[0] = (point1[0] + point2[0]) / 2.0;
            midpoint[1] = (point1[1] + point2[1]) / 2.0;
            return midpoint;
        }

    }
}