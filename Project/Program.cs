using static System.Console;
using Project;

//FEM task = new("file/test1/", 1);               //! Обычный тест с одной областью в начале координат
//FEM task = new("file/test2-SPLIT/split1/", 2);  //! Тест с разбиением #1
//FEM task = new("file/test2-SPLIT/split2/", 2);  //! Тест с разбиением #2
//FEM task = new("file/test2-SPLIT/split3/", 2);  //! Тест с разбиением #3
//FEM task = new("file/test2-SPLIT/split4/", 2);  //! Тест с разбиением #4
//FEM task = new("file/test3-STUDY/study1/", 3);  //! Тест с книги МКЭ #1
//FEM task = new("file/test3-STUDY/study2/", 4);  //! Тест с книги МКЭ #2   
FEM task = new("file/test4-Decomposition/", 5); //! Тест с книги МКЭ #2 

task.solve();                                     //! Запуск решения задачи
//WriteLine(task.dataAll());                      //! Вывод всех данных задачи