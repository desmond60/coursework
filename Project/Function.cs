using static System.Math;

namespace Project
{
    public static class Function
    {
        public static uint NumberFunc;

        //* Функция u(x,y)
        public static double Absolut(double x, double y, uint area)
        { 
            switch(NumberFunc)
            {
                case 1: /// test1
                return area switch
                {
                    0 => x,
                    _ => 0,
                };   

                case 2: /// test2-SPLIT
                return area switch
                {
                    0 => 10*x + 10*y,
                    _ => 0,
                };

                case 3: /// test3-STUDY/test1
                return area switch
                {
                    0 => y*y,
                    1 => 20*y - 19,
                    _ => 0,
                };

                case 4: /// test3-STUDY/test2
                return area switch
                {
                    0 => x + 6*y - 2,
                    1 => x + 6*y - 2,
                    _ => 0,
                };

                case 5: /// test4-Decomposition
                return area switch
                {
                    0 => x,
                    _ => 0,
                };
                
            }
            return 0;
        }

        //* Функция f(x,y)
        public static double Func(double x, double y, uint area)
        { 
            switch(NumberFunc)
            {
                case 1: /// test1
                return area switch
                {
                    0 => 2*x,
                    _ => 0,
                };
                
                case 2: /// test2-SPLIT
                return area switch
                {
                    0 => 20*x + 20*y,
                    _ => 0,
                };

                case 3: /// test3-STUDY/test1
                return area switch
                {
                    0 => -20,
                    1 => 0,
                    _ => 0,
                };

                case 4: /// test3-STUDY/test2
                return area switch
                {
                    0 => 5*x + 30*y - 10,
                    1 => 0,
                    _ => 0,
                };

                case 5: /// test4-Decomposition
                return area switch
                {
                    0 => 2*x,
                    _ => 0,
                };
                
            }
            return 0;
        }   

        //* Функция lambda(x,y)
        public static double Lambda(double x, double y, uint area)
        {
            switch(NumberFunc)
            {
                case 1: /// test1
                return area switch
                {
                    0 => 1,
                    _ => 0,
                };
                
                case 2: /// test2-SPLIT
                return area switch
                {
                    0 => 4,
                    _ => 0,
                };

                case 3: /// test3-STUDY/test1
                return area switch
                {
                    0 => 10,
                    1 => 1,
                    _ => 0,
                };

                case 4: /// test3-STUDY/test2
                return area switch
                {
                    0 => 1,
                    1 => 1,
                    _ => 0,
                };

                case 5: /// test4-Decomposition
                return area switch
                {
                    0 => x,
                    _ => 0,
                };
            }
            return 0;
        }

        //* Функция первого краевого условия 
        public static double Func_First_Kraev(double x, double y, uint area)
        {
            switch(NumberFunc)
            {
                case 1: /// test1
                return area switch
                {
                    0 => x,
                    _ => 0,
                };
                
                case 2: /// test2-SPLIT
                return area switch
                {
                    0 => 50 + 10*y,
                    _ => 0,
                };

                case 3: /// test3-STUDY/test1
                return area switch
                {
                    0 => y*y,
                    _ => 0,
                };

                case 4: /// test3-STUDY/test2
                return area switch
                {
                    0 => 6*y + 2,
                    _ => 0,
                };

                case 5: /// test4-Decomposition
                return area switch
                {
                    0 => x,
                    _ => 0,
                };
            }
            return 0;
        }

        //* Функция второго краевого условия 
        public static double Func_Second_Kraev(double x, double y, uint area, uint lam_area)
        {   
            switch(NumberFunc)
            {
                case 1: /// test1
                return area switch
                {
                    0 => 1,
                    1 => -1,
                    _ => 0,
                };
                
                case 2: /// test2-SPLIT
                return area switch
                {
                    0 => -40,
                    1 => 40,
                    _ => 0,
                };

                case 3: /// test3-STUDY/test1
                return area switch
                {
                    0 => 20,
                    1 => 0,
                    _ => 0,
                };

                case 4: /// test3-STUDY/test2
                return area switch
                {
                    0 => -6,
                    1 => -1,
                    2 => 6,
                    _ => 0,
                };

                case 5: /// test4-Decomposition
                return area switch
                {
                    0 => x, 
                    1 => -x, 
                    _ => 0,
                };
            }
            return 0;
        }

        //* Функция третьего краевого условия 
        public static double Func_Third_Kraev(double x, double y, uint area, uint lam_area)
        {   
            switch(NumberFunc)
            {
                case 1: /// test1
                return area switch
                {
                    0 => x,
                    _ => 0,
                };
                
                case 2: /// test2-SPLIT
                return area switch
                {
                    0 => 10*x + 2,
                    _ => 0,
                };

                case 3: /// test3-STUDY/test1
                return area switch
                {
                    0 => 20*y - 27,
                    _ => 0,
                };

                case 4: /// test3-STUDY/test2
                return area switch
                {
                    0 => 6*y + 2.1,
                    _ => 0,
                };

                case 5: /// test4-Decomposition
                return area switch
                {
                    0 => x, 
                    _ => 0,
                };
            }
            return 0;
        }
        

    }
}