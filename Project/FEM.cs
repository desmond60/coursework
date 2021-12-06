//TODO: ██████████████████████████████████████████████████████
//TODO: █░░░░░░░░░░░░░░█░░░░░░░░░░░░░░█░░░░░░██████████░░░░░░█
//TODO: █░░▄▀▄▀▄▀▄▀▄▀░░█░░▄▀▄▀▄▀▄▀▄▀░░█░░▄▀░░░░░░░░░░░░░░▄▀░░█
//TODO: █░░▄▀░░░░░░░░░░█░░▄▀░░░░░░░░░░█░░▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀░░█
//TODO: █░░▄▀░░█████████░░▄▀░░█████████░░▄▀░░░░░░▄▀░░░░░░▄▀░░█
//TODO: █░░▄▀░░░░░░░░░░█░░▄▀░░░░░░░░░░█░░▄▀░░██░░▄▀░░██░░▄▀░░█
//TODO: █░░▄▀▄▀▄▀▄▀▄▀░░█░░▄▀▄▀▄▀▄▀▄▀░░█░░▄▀░░██░░▄▀░░██░░▄▀░░█
//TODO: █░░▄▀░░░░░░░░░░█░░▄▀░░░░░░░░░░█░░▄▀░░██░░░░░░██░░▄▀░░█
//TODO: █░░▄▀░░█████████░░▄▀░░█████████░░▄▀░░██████████░░▄▀░░█
//TODO: █░░▄▀░░█████████░░▄▀░░░░░░░░░░█░░▄▀░░██████████░░▄▀░░█
//TODO: █░░▄▀░░█████████░░▄▀▄▀▄▀▄▀▄▀░░█░░▄▀░░██████████░░▄▀░░█
//TODO: █░░░░░░█████████░░░░░░░░░░░░░░█░░░░░░██████████░░░░░░█
//TODO: ██████████████████████████████████████████████████████

using static System.Console;
using static System.Math;
using static Project.Function;
using static Project.Helper;
using System.Linq;
using System.IO;
using System;
using System.Collections.Generic;


namespace Project
{
    public class FEM : Data
    {
        protected string Path; // Путь к тесту (к папке с файлами)
        Matrix matrix;
        

    // ************ Коструктор ************ //
        public FEM(string Path, int Num) : base(Path) { this.Path = Path; Function.NumberFunc = Num; } 

        public void solve() {
            portrait(); //? Составление списка связанностей и массивов ig[] и jg[]
            global();   //? Составление глобальный матрицы
        }

    // ************ Составление портрета ************ //
        private void portrait()
        {
            // Составление списка связанностей
            var list = new int[countNode][];

            var listI = new HashSet<int>();
            for (int i = 0; i < numNodes.Length; i++)
            {
                int value = numNodes[i];
                for (int k = 0; k < countFinitEl; k++)
                {
                    if (finitElements[k].Contains(value))
                        for (int p = 0; p < 3; p++)
                            if (finitElements[k][p] < value)
                                listI.Add(finitElements[k][p]);
                }
                list[i] = listI.OrderBy(n => ((uint)n)).ToArray();
                listI.Clear();
            }

            // Заполнение ig[]
            matrix.ig = new int[countNode + 1];
            matrix.ig[0] = matrix.ig[1] = 0;
            for (int i = 1; i < countNode; i++)
                matrix.ig[i + 1] = (matrix.ig[i] + list[i].Length);

            // Заполнение jg
            matrix.jg = new int[matrix.ig[countNode]];
            int jj = 0;
            for (int i = 0; i < countNode; i++)
                for (int j = 0; j < list[i].Length; j++, jj++)
                    matrix.jg[jj] = list[i][j];

            // Размерность глобальной матрицы
            matrix.N = matrix.ig.Length - 1;
            resize();
        }

        private void global() 
        {
            // Для каждого конечного элемента
            for (int index_fin_el = 0; index_fin_el < countFinitEl; index_fin_el++)
            {
                // Составим локальную матрицу и локальный вектор
                (double[][] local_matrix, double[] local_f) = local(index_fin_el);

                // Занесение в глобальную
                EntryMatInGlobalMatrix(local_matrix, finitElements[index_fin_el]);  
                EntryVecInGlobalMatrix(local_f,      finitElements[index_fin_el]);
            }

            // Для каждого условия на границе
            for (int index_kraev_cond = 0; index_kraev_cond < countKrayCond; index_kraev_cond++)
            {
                int[] curr_kraev = boards[index_kraev_cond]; // Номер краевого условия
                int[] Node = {curr_kraev[1], curr_kraev[2]};
                if (curr_kraev[3] == 1)
                    First_Kraev(curr_kraev); 
                else if (curr_kraev[3] == 2) {
                    double[] corr_vec = Second_Kraev(curr_kraev);
                    EntryVecInGlobalMatrix(corr_vec, Node);
                }
                else {
                    (double[][] corr_mat, double[] corr_vec) = Third_Kraev(curr_kraev);
                    EntryMatInGlobalMatrix(corr_mat, Node);
                    EntryVecInGlobalMatrix(corr_vec, Node);

                }
            }

            // Запись матрицы
            WriteMatrix(); 
        }

        //* Построение локальной матрицы и вектора
        private (double[][], double[]) local(int index_fin_el)
        {
                double[]   local_f      = build_F(index_fin_el);  // Построение локального вектора
                double[][] M            = build_M(index_fin_el);  // Построение матрица массы
                double[][] G            = build_G(index_fin_el);  // Построение матрица жесткости
                double[][] local_matrix = SummMatrix(G, M)     ;  // Локальная матрицы (G + M)

                return (local_matrix, local_f);
        }

        //* Занесение матрицы в глоабальную матрицу
        private void EntryMatInGlobalMatrix(double[][] local_matrix, int[] index)
        {         
            for (int i = 0, h = 0; i < local_matrix.GetUpperBound(0) + 1; i++)
            {
                int ibeg = index[i];
                for (int j = i + 1; j < local_matrix.GetUpperBound(0) + 1; j++)
                {
                    int iend = index[j];
                    int temp = ibeg;
                    if (temp < iend) 
                        (iend, temp) = (temp, iend);      
                            
                    h = matrix.ig[temp];
                    while(matrix.jg[h++] - iend < 0); h--;
                    matrix.ggl[h] += local_matrix[i][j];
                    matrix.ggu[h] += local_matrix[j][i];
                }
                matrix.di[ibeg] += local_matrix[i][i];
            }                
        }

        //* Занесение вектора в глолбальный вектор
        private void EntryVecInGlobalMatrix(double[] f, int[] index)
        {
            for (int i = 0; i < f.Length; i++)
                matrix.pr[index[i]] += f[i];
        }

        //* Построение вектора правой части
        private double[] build_F(int index_fin_el)
        {
            int area_fin_el = areaFinitEl[index_fin_el];        // Область в которой расположек к.э.
            double detD = Abs(ComputeDet(index_fin_el)) / 24.0; // Вычисление detD

            double[] f = new double[3];                         // Вычисление f - на узлах
            for (int i = 0; i < f.Length; i++)
                f[i] = detD * Func(
                     nodes[finitElements[index_fin_el][i]][0],
                     nodes[finitElements[index_fin_el][i]][1],
                     area_fin_el
                );

            double[] local_f = new double[3];                   // Вычисление локального вектора
            local_f[0] = 2*f[0] + f[1] + f[2];
            local_f[1] = 2*f[1] + f[0] + f[2];
            local_f[2] = 2*f[2] + f[1] + f[0]; 
            
            return local_f;
        }

        //* Построение матрицы массы
        private double[][] build_M(int index_fin_el)
        {
            double[][] M_matrix = new double[3][];             // Матрица масс
            for (int i = 0; i < 3; i++) M_matrix[i] = new double[3];

            int area_fin_el = areaFinitEl[index_fin_el];        // Область в которой расположек к.э.
            double gamma = materials[area_fin_el].gamma;        // Значение гаммы в этой области

            double value = gamma * Abs(ComputeDet(index_fin_el)) / 24.0;    // Значение матрицы массы 

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                if (i == j) 
                    M_matrix[i][j] = 2 * value;
                else M_matrix[i][j] = value;

            return M_matrix;
        }

        //* Построение матрицы жесткости
        private double[][] build_G(int index_fin_el)
        {
            double [][] G_matrix = new double[3][];                 // Матрица жесткости
            for (int i = 0; i < 3; i++) G_matrix[i] = new double[3];

            double[] Node1 = nodes[finitElements[index_fin_el][0]]; // Координаты 1 узла i - конечного элемента
            double[] Node2 = nodes[finitElements[index_fin_el][1]]; // Координаты 2 узла i - конечного элемента 
            double[] Node3 = nodes[finitElements[index_fin_el][2]]; // Координаты 3 узла i - конечного элемента
            double[] Mid12 = MidPoints(Node1, Node2);
            double[] Mid13 = MidPoints(Node1, Node3);
            double[] Mid23 = MidPoints(Node2, Node3);

            int area_fin_el = areaFinitEl[index_fin_el];            // Область в которой расположек к.э.
            double lambda = Lambda(Mid12[0], Mid12[1], area_fin_el) + 
                            Lambda(Mid13[0], Mid13[1], area_fin_el) + 
                            Lambda(Mid23[0], Mid23[1], area_fin_el); // Подсчет ламбда расложения
            
            double multip = lambda / (6 * Abs(ComputeDet(index_fin_el)));
            double[,] a = ComputeA(index_fin_el);

            G_matrix[0][0] = multip * (a[0,0]*a[0,0] + a[0,1]*a[0,1]);  
            G_matrix[0][1] = multip * (a[0,0]*a[1,0] + a[0,1]*a[1,1]);
            G_matrix[0][2] = multip * (a[0,0]*a[2,0] + a[0,1]*a[2,1]);
            G_matrix[1][0] = multip * (a[1,0]*a[0,0] + a[1,1]*a[0,1]);
            G_matrix[1][1] = multip * (a[1,0]*a[1,0] + a[1,1]*a[1,1]);
            G_matrix[1][2] = multip * (a[1,0]*a[2,0] + a[1,1]*a[2,1]);
            G_matrix[2][0] = multip * (a[2,0]*a[0,0] + a[2,1]*a[0,1]);
            G_matrix[2][1] = multip * (a[2,0]*a[1,0] + a[2,1]*a[1,1]);
            G_matrix[2][2] = multip * (a[2,0]*a[2,0] + a[2,1]*a[2,1]);      

            return G_matrix;
        }

        //* Первое краевое условие
        private void First_Kraev(int[] kraev)
        {
            // Ставим вместо диагонального эл. единицу
            matrix.di[kraev[1]] = 1;
            matrix.di[kraev[2]] = 1;

            // В вектор правой части ставим значение краевого условия
            matrix.pr[kraev[1]] = Func_First_Kraev(nodes[kraev[1]][0], nodes[kraev[1]][1], kraev[4]);
            matrix.pr[kraev[2]] = Func_First_Kraev(nodes[kraev[2]][0], nodes[kraev[2]][1], kraev[4]);

            // Зануляем в строке все стоящие элементы кроме диагонального
            for (int i = 0; i < matrix.ig[kraev[1] + 1] - matrix.ig[kraev[1]]; i++)
                matrix.ggl[matrix.ig[kraev[1]] + i] = 0;
            for (int i = 0; i < matrix.ig[kraev[2] + 1] - matrix.ig[kraev[2]]; i++)
                matrix.ggl[matrix.ig[kraev[2]] + i] = 0;

            for (int i = kraev[1] + 1; i < countNode; i++)
            {
                int ibeg = matrix.ig[i];
                int iend = matrix.ig[i + 1];
                for (int p = ibeg; p < iend; p++)
                    if (matrix.jg[p] == kraev[1]) {
                        matrix.ggu[p] = 0;
                        continue;
                    }
            }

            for (int i = kraev[2] + 1; i < countNode; i++)
            {
                int ibeg = matrix.ig[i];
                int iend = matrix.ig[i + 1];
                for (int p = ibeg; p < iend; p++)
                    if (matrix.jg[p] == kraev[2]) {
                        matrix.ggu[p] = 0;
                        continue;
                    }
            }
        }

        //* Второе краевое условие
        private double[] Second_Kraev(int[] kraev)
        {
            double[] corr_vec = new double[2];

            int[] Node = {kraev[1], kraev[2]};
            double multip = ComputeMesG(nodes[Node[0]], nodes[Node[1]]) / 6.0;
            for (int i = 0, j = 1; i < 2; i++, j--)
                corr_vec[i] = multip * (2 * Func_Second_Kraev(nodes[Node[i]][0], nodes[Node[i]][1], kraev[4]) +
                                            Func_Second_Kraev(nodes[Node[j]][0], nodes[Node[j]][1], kraev[4]));
            return corr_vec;
        }

        //* Третье краевое условие
        private (double[][], double[]) Third_Kraev(int[] kraev)
        {
            double[]   corr_vec = new double[2];
            double[][] corr_mat = new double[2][];
            for (int i = 0; i < 2; i++) corr_mat[i] = new double[2];

            int[] Node = {kraev[1], kraev[2]};
            double betta = materials[kraev[0]].betta;
            double multip = (betta * ComputeMesG(nodes[Node[0]], nodes[Node[1]])) / 6.0;
            
            for (int i = 0, k = 1; i < 2; i++, k--) {
                corr_vec[i] = multip * (2 * Func_Third_Kraev(nodes[Node[i]][0], nodes[Node[i]][1], kraev[4]) +
                                            Func_Third_Kraev(nodes[Node[k]][0], nodes[Node[k]][1], kraev[4]));
                for (int j = 0; j < 2; j++)
                    corr_mat[i][j] = i == j ? 2 * multip : multip;
            }

            return (corr_mat, corr_vec);
        }

        //* Подсчет компонента detD
        private double ComputeDet(int index_fin_el)
        {
            double[] Node1 = nodes[finitElements[index_fin_el][0]]; // Координаты 1 узла i - конечного элемента
            double[] Node2 = nodes[finitElements[index_fin_el][1]]; // Координаты 2 узла i - конечного элемента 
            double[] Node3 = nodes[finitElements[index_fin_el][2]]; // Координаты 3 узла i - конечного элемента
            
            double detD = (Node2[0] - Node1[0])*(Node3[1] - Node1[1]) - 
                          (Node3[0] - Node1[0])*(Node2[1] - Node1[1]);
            return detD;
        }

        //* Подсчет компонента mes по ребру G
        private double ComputeMesG(double[] Node1, double[] Node2)
        {
            return Sqrt( Pow((Node2[0] - Node1[0]), 2) +  
                         Pow((Node2[1] - Node1[1]), 2) );
        }
        //* Подсчет компонентов a
        private double[,] ComputeA(int index_fin_el)
        {
            double[,] a = new double[3,2];
            double[] Node1 = nodes[finitElements[index_fin_el][0]]; // Координаты 1 узла i - конечного элемента
            double[] Node2 = nodes[finitElements[index_fin_el][1]]; // Координаты 2 узла i - конечного элемента 
            double[] Node3 = nodes[finitElements[index_fin_el][2]]; // Координаты 3 узла i - конечного элемента

            a[0,0] = Node2[1] - Node3[1];
            a[1,0] = Node3[1] - Node1[1];
            a[2,0] = Node1[1] - Node2[1];
            a[0,1] = Node3[0] - Node2[0];
            a[1,1] = Node1[0] - Node3[0];
            a[2,1] = Node2[0] - Node1[0];
            return a;
        }

        //* resize массивов глобальной матрицы
        private void resize()
        {
            matrix.di  = new double[matrix.N];
            matrix.pr  = new double[matrix.N];
            matrix.ggl = new double[matrix.jg.Length];
            matrix.ggu = new double[matrix.jg.Length];
        }

        //* Записб глобальной матрицы
        private void WriteMatrix()
        {   
            Directory.CreateDirectory(Path + "matrix");
            File.WriteAllText(Path + "matrix/kuslau.txt", $"{matrix.N}");
            File.WriteAllText(Path + "matrix/ig.txt",  String.Join(" ", matrix.ig) );
            File.WriteAllText(Path + "matrix/jg.txt",  String.Join(" ", matrix.jg) );
            File.WriteAllText(Path + "matrix/di.txt",  String.Join(" ", matrix.di) );
            File.WriteAllText(Path + "matrix/ggl.txt", String.Join(" ", matrix.ggl));
            File.WriteAllText(Path + "matrix/ggu.txt", String.Join(" ", matrix.ggu));
            File.WriteAllText(Path + "matrix/pr.txt",  String.Join(" ", matrix.pr) );   
        }




    }
}