//: ██████████████████████████████████████████████
//: █░░░░░░█████████░░░░░░░░░░░░░░█░░░░░░░░░░░░░░█
//: █░░▄▀░░█████████░░▄▀▄▀▄▀▄▀▄▀░░█░░▄▀▄▀▄▀▄▀▄▀░░█
//: █░░▄▀░░█████████░░▄▀░░░░░░▄▀░░█░░▄▀░░░░░░░░░░█
//: █░░▄▀░░█████████░░▄▀░░██░░▄▀░░█░░▄▀░░█████████
//: █░░▄▀░░█████████░░▄▀░░██░░▄▀░░█░░▄▀░░░░░░░░░░█
//: █░░▄▀░░█████████░░▄▀░░██░░▄▀░░█░░▄▀▄▀▄▀▄▀▄▀░░█
//: █░░▄▀░░█████████░░▄▀░░██░░▄▀░░█░░░░░░░░░░▄▀░░█
//: █░░▄▀░░█████████░░▄▀░░██░░▄▀░░█████████░░▄▀░░█
//: █░░▄▀░░░░░░░░░░█░░▄▀░░░░░░▄▀░░█░░░░░░░░░░▄▀░░█
//: █░░▄▀▄▀▄▀▄▀▄▀░░█░░▄▀▄▀▄▀▄▀▄▀░░█░░▄▀▄▀▄▀▄▀▄▀░░█
//: █░░░░░░░░░░░░░░█░░░░░░░░░░░░░░█░░░░░░░░░░░░░░█
//: ██████████████████████████████████████████████

using static Project.Helper;
using static System.Math;
using System;
using System.Linq;

namespace Project
{
    public class LOS
    {   
        private Matrix matrix;    /// Структура СЛАУ
        
    // ************ Коструктор LOS ************ //
        public LOS(Matrix matrix, int maxIter, double eps) { 
            this.matrix         = matrix; 
            this.matrix.maxIter = maxIter;
            this.matrix.epsilon = eps;    
        } 

        //* Решение СЛАУ
        public Matrix solve(bool isLog = true) {
            var r      = new double[matrix.N];
            var z      = new double[matrix.N];
            var multLr = new double[matrix.N];
            var Lr     = new double[matrix.N];
            var p      = new double[matrix.N];
            double alpha, betta, Eps;
            int iter = 0;

            double[] L = Enumerable.Range(0, matrix.N).Select(i => 1.0 / matrix.di[i]).ToArray();

            double[] multX = matrix.mult(matrix.x);
            for (int i = 0; i < r.Length; i++) {
                r[i] = L[i] * (matrix.pr[i] - multX[i]);
                z[i] = L[i] * r[i];
            }
            double[] multZ = matrix.mult(z);
            for (int i = 0; i < p.Length; i++)
                p[i] = L[i] * multZ[i];

            do
            {
                betta = scalar(p, p);
                alpha = scalar(p, r) / betta;
                for (int i = 0; i < matrix.x.Length; i++) {
                    matrix.x[i] += alpha * z[i];
                    r[i]        -= alpha * p[i];
                    Lr[i]       = L[i] * r[i];
                }

                multLr = matrix.mult(Lr);
                for (int i = 0; i < Lr.Length; i++)
                    multLr[i] = L[i] * multLr[i];
                betta = -scalar(p, multLr) / betta;
                for (int i = 0; i < z.Length; i++) {
                    z[i] = L[i] * r[i] + betta * z[i];
                    p[i] = multLr[i] + betta * p[i];
                }
                Eps = scalar(r, r);
                
                iter++;
                if (isLog) printLog(iter, Eps);
            } while (iter < matrix.maxIter
                && Eps > matrix.epsilon);

            return matrix;
        }

        //* Вывод невязки и количества итераций
        private void printLog(int Iter, double Eps) {
            Console.WriteLine($"Iteration = {Iter}\t\t" + 
                              $"Discrepancy = {Eps}");
        }
    }
}