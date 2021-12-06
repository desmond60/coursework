using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Text;
using static System.Diagnostics.Debug;

namespace Project
{
    public class Data
    {

    // ************ Данные ************ //
         protected (double gamma, double betta)[] materials ; // Значения лямбда гамма и бетта

         protected int       countNode     ; // Количество узлов
         protected int       countFinitEl  ; // Количество конечных элементов
         protected int       countAreas    ; // Количество областей
         protected int       countKrayCond ; // Количество краевых условий

         protected double[][] nodes         ; // Координаты узлов
         protected int[][]    finitElements ; // Координаты конечных элементов
         protected int[]      areaFinitEl   ; // номера областей в которых расположен кон. эл. по порядку
         protected int[][]    boards        ; // Условия на границах

         public int[]         numNodes      ; // Номера узлов
        
         public Data(string Path) { Assert(Read(Path), "Неккоректные файлы или данные"); }

    // ************ Чтение данных ************ //
        private bool Read(string path)
        {
            bool isCorr = true;

            if (!File.Exists(path + "param.txt")) return false;
            isCorr &= readParams(path + "param.txt");

            resize(); // ВЫделение памяти

            isCorr &= readfiles(path + "xy.txt", ref nodes);
            isCorr &= readfiles(path + "area.txt", ref materials, ref areaFinitEl);
            isCorr &= readfiles(path + "board.txt", ref boards);
            isCorr &= readNumNodes(path + "finitel.txt", ref numNodes, ref finitElements);       

            return isCorr;
        }

        private bool readfiles(string path, ref double[][] array)
        {
            if (!File.Exists(path)) return false;    
            string[] lines = File.ReadAllLines(path);

            for (int i = 0; i < lines.Length; i++) {
                var values = lines[i].Split(" ").Select(n => double.Parse(n)).ToArray();
                for (int j = 0; j < values.Length; j++)
                    array[i][j] = values[j];
            }
            return true;
        }

        private bool readfiles(string path, ref int[][] array)
        {
            if (!File.Exists(path)) return false;    
            string[] lines = File.ReadAllLines(path);

            for (int i = 0; i < lines.Length; i++) {
                var values = lines[i].Split(" ").Select(n => int.Parse(n)).ToArray();
                for (int j = 0; j < values.Length; j++)
                    array[i][j] = values[j];
            }
            return true;
        }

        private bool readNumNodes(string path, ref int[] numNodes, ref int[][] finEl)
        {
            if (!File.Exists(path)) return false;
            string[] lines = File.ReadAllLines(path);

            List<int> listTemp = new List<int>();

            for (int i = 0; i < lines.Length; i++)
            {
                var values = lines[i].Split(" ").Select(n => int.Parse(n)).ToArray();
                for (int j = 0; j < values.Length; j++) {
                    if (!listTemp.Contains(values[j]))
                        listTemp.Add(values[j]);
                    finEl[i][j] = values[j];
                }
            }
            // Массив для списка связанностей
            numNodes = listTemp.OrderBy(n => ((uint)n)).ToArray();
            return true;
        }

        private bool readfiles(string path, ref (double gamma, double betta)[] material, ref int[] area)
        {
            if (!File.Exists(path)) return false;
            StreamReader file = new StreamReader(path);
            for (int i = 0; i < countAreas; i++) {
                double[] line = file.ReadLine().Split(" ").Select(n => double.Parse(n)).ToArray();
                material[i] = (line[0], line[1]);
            } 
            areaFinitEl = file.ReadLine().Split(" ").Select(n => int.Parse(n)).ToArray();
            file.Close();
            return true;
        }

        private bool readParams(string path)
        {
            bool isCorr = true;
            StreamReader file = new StreamReader(path);
            string[] param = file.ReadLine().Split(" ");

            isCorr &= int.TryParse(param[0], out countNode);
            isCorr &= int.TryParse(param[1], out countFinitEl);
            isCorr &= int.TryParse(param[2], out countAreas);
            isCorr &= int.TryParse(param[3], out countKrayCond);

            file.Close();
            return isCorr;
        }

        private void resize()
        {
            nodes         = new double                   [countNode][];
            for (int i = 0; i < countNode; i++)          nodes[i] = new double[2];
            finitElements = new int                      [countFinitEl][];
            for (int i = 0; i < countFinitEl; i++)       finitElements[i] = new int[3];
            materials     = new (double, double)         [countAreas];
            areaFinitEl   = new int                      [countFinitEl];
            boards        = new int                      [countKrayCond][];
            for (int i = 0; i < countKrayCond; i++)      boards[i] = new int[5];
            numNodes      = new int                      [countNode];
        }
    
    // ************ Вывод данных ************ //
        public string dataAll()
        { 
            System.Console.OutputEncoding = Encoding.GetEncoding(65001);
            StringBuilder output = new StringBuilder($"(Количество узлов) countNode                 = {countNode}    \n" + 
                                                     $"(Количество конечных элементов) countFinitEl = {countFinitEl} \n" + 
                                                     $"(Количество областей) countAreas             = {countAreas}   \n" + 
                                                     $"(Количество граничных условий)               = {countKrayCond}\n"); 

            output.Append("---------------------------------------------------\n");
            output.Append("(Координаты узлов) nodes: \n");
            for (int i = 0; i < countNode; i++)
                output.Append($"({nodes[i][0]}, {nodes[i][1]})\n");

            output.Append("---------------------------------------------------\n");
            output.Append("(Координаты конечных элементов) finitElements: \n");
            for (int i = 0; i < countFinitEl; i++)
                output.Append($"({finitElements[i][0]}, {finitElements[i][1]}, {finitElements[i][2]}) -> область {areaFinitEl[i]}\n");

            output.Append("---------------------------------------------------\n");
            output.Append("(лямда, гамма и бетта для каждой области) materials = \n");
            for (int i = 0; i < countAreas; i++)
                output.Append($"{i}: \u03B3 = {materials[i].gamma}, \u03B2 = {materials[i].betta})\n");

            output.Append("---------------------------------------------------\n");
            output.Append("Данные граничных условий = \n");
            for (int i = 0; i < countKrayCond; i++)
                output.Append($"({boards[i][0]}, {boards[i][1]}, {boards[i][2]}, {boards[i][3]}, {boards[i][4]})\n");
            
            return output.ToString();
        }

    }
}