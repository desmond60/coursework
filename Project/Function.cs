namespace Project
{
    public static class Function
    {
        public static int NumberFunc;

        //* Функция f(x)
        public static double Func(double x, double y, int area)
        { 
            switch(NumberFunc)
            {
                case 1: //? test1
                return area switch
                {
                    0 => 2 * x,
                    _ => 0,
                };
                
                case 2: //? test2
                return area switch
                {
                    0 => 20,
                    _ => 0,
                };
            }
            return 0;
        }   

        //* Функция lambda 
        public static double Lambda(double x, double y, int area)
        {
            switch(NumberFunc)
            {
                case 1: //? test1
                return area switch
                {
                    0 => 1,
                    _ => 0,
                };
                
                case 2: //? test2
                return area switch
                {
                    0 => 10,
                    1 => 1,
                    _ => 0,
                };
            }
            return 0;
        }

        //* Функция первого краевого условия 
        public static double Func_First_Kraev(double x, double y, int area)
        {
            switch(NumberFunc)
            {
                case 1: //? test1
                return area switch
                {
                    0 => x,
                    _ => 0,
                };
                
                case 2: //? test2
                return area switch
                {
                    0 => y*y,
                    _ => 0,
                };
            }
            return 0;
        }

        //* Функция второго краевого условия 
        public static double Func_Second_Kraev(double x, double y, int area)
        {   
            switch(NumberFunc)
            {
                case 1: //? test1
                return area switch
                {
                    0 => 1,
                    1 => -1,
                    _ => 0,
                };
                
                case 2: //? test2
                return area switch
                {
                    0 => 20,
                    1 => 0,
                    _ => 0,
                };
            }
            return 0;
        }

        //* Функция третьего краевого условия 
        public static double Func_Third_Kraev(double x, double y, int area)
        {   
            switch(NumberFunc)
            {
                case 1: //? test1
                return area switch
                {
                    0 => x,
                    _ => 0,
                };
                
                case 2: //? test2
                return area switch
                {
                    0 => 20*y - 27,
                    _ => 0,
                };
            }
            return 0;
        }
        

    }
}