using static System.Diagnostics.Debug;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Project
{
    public class Data
    {

    // ************ Данные задачи ************ //
        protected (double gamma, double betta)[] materials ; /// Значения гамма и бетта

        protected uint       countNode     ; /// Количество узлов
        protected uint       countFinitEl  ; /// Количество конечных элементов
        protected uint       countAreas    ; /// Количество областей
        protected uint       countKrayCond ; /// Количество краевых условий

        protected double[][]  nodes         ; /// Координаты узлов
        protected uint[][]    finitElements ; /// Координаты конечных элементов
        protected uint[]      areaFinitEl   ; /// номера областей в которых расположен кон. эл. по порядку
        protected uint[][]    boards        ; /// Условия на границах
        protected uint[]      nodesArea     ; /// Область для каждого узла

        //? Для составления списка связанностей
        public uint[]         numNodes      ; /// Номера узлов
        
    // ************ Конструктор DATA (чтение данных) ************ //
         public Data(string Path) { Assert(Read(Path), "Несуществующий файл или неккоректные данные!"); }

        //* Чтение данных
        private bool Read(string path)
        {
            bool isCorr = true;

            isCorr &= readParams(path + "param.txt");

            resize(); // Выделение памяти

            isCorr &= readfiles    (path + "xy.txt",      ref nodes                       );
            isCorr &= readMaterial (path + "area.txt",    ref materials, ref areaFinitEl  );
            isCorr &= readfiles    (path + "board.txt",   ref boards                      );
            isCorr &= readNumNodes (path + "finitel.txt", ref numNodes, ref finitElements );
            isCorr &= readNodesArea(path + "XY_area.txt", ref nodesArea                   );

            // Отсортировать массив board по номеру краевого условия
            boards = boards.OrderByDescending(l => l[3]).ToArray();
    
            return isCorr;
        }

        private bool readfiles(string path, ref double[][] array)
        {
            if (!File.Exists(path)) return false;    
            string[] lines = File.ReadAllLines(path);

            for (uint i = 0; i < lines.Length; i++) {
                var values = lines[i].Split(" ").Select(n => double.Parse(n)).ToArray();
                for (uint j = 0; j < values.Length; j++)
                    array[i][j] = values[j];
            }
            return true;
        }

        private bool readfiles(string path, ref uint[][] array)
        {
            if (!File.Exists(path)) return false;    
            string[] lines = File.ReadAllLines(path);

            for (uint i = 0; i < lines.Length; i++) {
                var values = lines[i].Split(" ").Select(n => uint.Parse(n)).ToArray();
                for (uint j = 0; j < values.Length; j++)
                    array[i][j] = values[j];
            }
            return true;
        }

        private bool readNodesArea(string path, ref uint[] array)
        {
            if (!File.Exists(path)) return false;  
            array = File.ReadAllText(path).Split(" ").Select(n => uint.Parse(n)).ToArray();  
            return true;
        }

        private bool readNumNodes(string path, ref uint[] numNodes, ref uint[][] finEl)
        {
            if (!File.Exists(path)) return false;
            string[] lines = File.ReadAllLines(path);

            List<uint> listTemp = new List<uint>();

            for (int i = 0; i < lines.Length; i++)
            {
                var values = lines[i].Split(" ").Select(n => uint.Parse(n)).ToArray();
                for (int j = 0; j < values.Length; j++) {
                    if (!listTemp.Contains(values[j]))
                        listTemp.Add(values[j]);
                    finEl[i][j] = values[j];
                }
            }
            // Заполнение массива для списка связанностей
            numNodes = listTemp.OrderBy(n => ((uint)n)).ToArray();
            return true;
        }

        private bool readMaterial(string path, ref (double gamma, double betta)[] material, ref uint[] area)
        {
            if (!File.Exists(path)) return false;
            StreamReader file = new StreamReader(path);
            for (int i = 0; i < countAreas; i++) {
                double[] line = file.ReadLine().Split(" ").Select(n => double.Parse(n)).ToArray();
                material[i] = (line[0], line[1]);
            } 
            areaFinitEl = file.ReadLine().Split(" ").Select(n => uint.Parse(n)).ToArray();
            file.Close();
            return true;
        }

        private bool readParams(string path)
        {
            if (!File.Exists(path)) return false;
            bool isCorr = true;
            StreamReader file = new StreamReader(path);
            string[] param = file.ReadLine().Split(" ");

            isCorr &= uint.TryParse(param[0], out countNode);
            isCorr &= uint.TryParse(param[1], out countFinitEl);
            isCorr &= uint.TryParse(param[2], out countAreas);
            isCorr &= uint.TryParse(param[3], out countKrayCond);

            file.Close();
            return isCorr;
        }

        private void resize()
        {
            nodes         = new double           [countNode]    [];
            finitElements = new uint             [countFinitEl] [];
            materials     = new (double, double) [countAreas]     ;
            areaFinitEl   = new uint             [countFinitEl]   ;
            boards        = new uint             [countKrayCond][];
            numNodes      = new uint             [countNode]      ;
            for (uint i = 0; i < countNode    ; i++) nodes[i]         = new double[2];
            for (uint i = 0; i < countFinitEl ; i++) finitElements[i] = new uint[3];
            for (uint i = 0; i < countKrayCond; i++) boards[i]        = new uint[5];
        }
    
        //* Данные задачи
        public string dataAll()
        { 
            System.Console.OutputEncoding = Encoding.GetEncoding(65001);
            string margin = String.Join("", Enumerable.Repeat("-", 50));
            StringBuilder output = new StringBuilder();

            output.Append($"{margin}\n");
            output.Append($"(Количество узлов) countNode                 = {countNode}    \n" + 
                          $"(Количество конечных элементов) countFinitEl = {countFinitEl} \n" + 
                          $"(Количество областей) countAreas             = {countAreas}   \n" + 
                          $"(Количество граничных условий)               = {countKrayCond}\n"); 

            output.Append($"{margin}\n");
            output.Append("(Координаты узлов) nodes: \n");
            for (int i = 0; i < countNode; i++)
                output.Append($"({nodes[i][0]}, " + 
                              $"{nodes[i][1]}) -> область {nodesArea[i]}\n");

            output.Append($"{margin}\n");
            output.Append("(Координаты конечных элементов) finitElements: \n");
            for (int i = 0; i < countFinitEl; i++)
                output.Append($"({finitElements[i][0]}, " + 
                              $"{finitElements[i][1]}, "  + 
                              $"{finitElements[i][2]}) -> область {areaFinitEl[i]}\n");

            output.Append($"{margin}\n");
            output.Append("(гамма и бетта для каждой области) materials: \n");
            for (int i = 0; i < countAreas; i++)
                output.Append($"{i}: \u03B3 = {materials[i].gamma}, \u03B2 = {materials[i].betta})\n");

            output.Append($"{margin}\n");
            output.Append("Данные граничных условий: \n");
            for (int i = 0; i < countKrayCond; i++)
                output.Append($"({boards[i][0]}, " + 
                               $"{boards[i][1]}, " + 
                               $"{boards[i][2]}, " + 
                               $"{boards[i][3]}, " + 
                               $"{boards[i][4]})\n");
            
            return output.ToString();
        }

    }
}