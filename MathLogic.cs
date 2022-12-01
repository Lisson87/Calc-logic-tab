using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WpfApp1
{
    public class MathLogic
    {
        private const int COLUMNADD = 2;    //число добавления столбцов для таблицы истинности (1 для f) (2-й для неопределенных коэффициентов)

        private string sourceString;

        private string stable;          //таблица истинности
        private string ssdnf;           //СДНФ
        private string ssknf;           //СКНФ
        private string simportance;     //Существенность переменных
        private string sjegalkin;       //Полином Жегалкина
        private string silinear;        //проверка на линейность. Класс L
        private string siselfdual;      //проверка на самодвойственность. Класс S
        private string simonotony;      //Класс M
        private string sisavesZero;     //Класс T0
        private string sisavesUnit;     //Класс T1

        public MathLogic(string text)
        {
            sourceString = text;
            if (checking_input(sourceString) == 0)
            {
                solve(sourceString);
            }
        }

        public string getTable()
        {
            return stable;
        }

        public string getResult()
        {
            string result = ssdnf + ssknf + simportance + sjegalkin + silinear + siselfdual + simonotony + sisavesZero + sisavesUnit;
            return result;
        }

        public static int checking_input(string sourceString)
        {
            int vectorcorrect = 0;
            //проверка не пуста ли строка 
            if (String.IsNullOrEmpty(sourceString))
            {
                ////string s = "Ошибка! Cтрока пуста";
                ////lbl1.Content = s;
                ////lbl1.ToolTip = s;
                return 1;
            }
            else
            {
                for (int i = 0; i < sourceString.Length; i++)
                {
                    if (sourceString[i] == '0' | sourceString[i] == '1')
                    {
                        vectorcorrect++;
                    }
                    else
                    {
                        ////string s = "Ошибка! Строка должна содержать только цифры 0 и 1";
                        ////lbl1.Content = s;
                        ////lbl1.ToolTip = s;
                        return 2;
                    }
                }
            }
            if (vectorcorrect == sourceString.Length)
            {
                return 0;
            }
            else
            {
                return 3;
            }
            
        }

        /// <summary>
        /// решение функции заданной вектором (01100110)
        /// </summary>
        /// <param name="sourceString"></param>
        private int solve(string sourceString)
        {
            //при условии что функции задана вектором 0111 а не аргументами и операторами (X v Y^Z)
            //добавить проверку вектора на соответствующую длину кратную количеству переменных.

            int arg = CountArguments(sourceString);                                 //считаем количество аргументов
            //строки и столбцы для создания двумерного массива, (матрицы)
            int row = sourceString.Length;                              //строк в таблице
            int column = arg + COLUMNADD;                               //столбцов в таблице аргументы + f + неопределенные коэффициенты
            int[,] matrix = create_matrix(row, column, sourceString);   //создание таблицы истинности по вектору

            stable = print_matrix(matrix);                    //таблица истинности
            ssdnf = print_sdnf(matrix, row, column - COLUMNADD);          //СДНФ
            ssknf = print_sknf(matrix, row, column - COLUMNADD);          //СКНФ
            simportance = print_importance(matrix);           //Существенность переменных
            sjegalkin = Jegalkin(matrix, sourceString);       //Полином Жегалкина
            silinear = ilinear(matrix);                       //проверка на линейность. Класс L
            siselfdual = iselfdual(matrix);                   //проверка на самодвойственность. Класс S
            simonotony = imonotony(matrix);                   //функция монотонна. Класс M
            sisavesZero = isavesZero(matrix);                 //сохраняет ноль. Класс T0
            sisavesUnit = isavesUnit(matrix);                 //сохраняет единицу. Класс T1


            //Функция Самодвойственна?      +
            //функция Линейна?              +
            //функция Монотонна?
            //функция Сохраняет 0 ноль      +
            //функция Сохраняет 1 единицу   +
            //таблица по теореме Поста (о функциональной полноте)

            //Сокращенная ДНФ
            //Минимальная ДНФ
            //Тупиковая ДНФ
            return 0;
        }

        private string imonotony(int[,] matrix)
        {
            string sresult = "";
            int predok;
            ArrayList mpotomok;
            string text = "";
            bool bmonotony = true;
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                if(matrix[i,matrix.GetLength(1) - COLUMNADD] == 1)
                {
                    predok = i;
                    mpotomok = getpotomok(matrix, predok);

                    foreach(int numb in mpotomok)
                    {
                        if (matrix[predok, matrix.GetLength(1) - COLUMNADD] != matrix[numb, matrix.GetLength(1) - COLUMNADD])
                        {
                            bmonotony = false;
                            text += "  ";
                            for (int j = 0; j < matrix.GetLength(1) - COLUMNADD; j++)
                            {
                                text += matrix[predok, j];
                            }
                            text += " ~ ";
                            text += matrix[predok, matrix.GetLength(1) - COLUMNADD];

                            text += " ≠ ";
                            for (int j = 0; j < matrix.GetLength(1) - COLUMNADD; j++)
                            {
                                text += matrix[numb, j];
                            }
                            text += " ~ ";
                            text += matrix[numb, matrix.GetLength(1) - COLUMNADD];
                        }
                    }
                }
            }
            sresult += bmonotony == true ? "Функция монотонна" : "Функция Не монотонна";
            sresult += text;
            sresult += "\n";
            return sresult;
        }

        private ArrayList getpotomok(int[,] matrix, int predok)
        {
            int next = predok + 1;
            ArrayList mpotomok = new ArrayList();
            for (int i = next; i < matrix.GetLength(0); i++)
            {
                int correctPotomok = 0;
                for (int j = 0; j < matrix.GetLength(1) - COLUMNADD; j++)
                {
                    if(matrix[predok, j] < matrix[i, j])
                    {
                        correctPotomok++;
                    }
                    else if(matrix[predok, j] == matrix[i, j])
                    {
                        correctPotomok++;
                    }
                    else
                    {
                        break;
                    }
                }
                if(correctPotomok == (matrix.GetLength(1) - COLUMNADD))
                {
                    mpotomok.Add(i);
                }
            }
            return mpotomok;
        }

        private string isavesZero(int[,] matrix) 
        {
            string sresult = "";
            bool bsavesZero;
            if (matrix[0, matrix.GetLength(1) - COLUMNADD] == 0)
            {
                bsavesZero = true;
            }
            else
            {
                bsavesZero = false;
            }
            sresult += bsavesZero == true ? "Функция сохраняет ноль" : "Функция Не сохраняет ноль";
            sresult += "\n";
            return sresult;
        }

        private string isavesUnit(int[,] matrix)
        {
            string sresult = "";
            bool bsavesZero;
            if (matrix[matrix.GetUpperBound(0), matrix.GetLength(1) - COLUMNADD] == 1)
            {
                bsavesZero = true;
            }
            else
            {
                bsavesZero = false;
            }
            sresult += bsavesZero == true ? "Функция сохраняет единицу" : "Функция Не сохраняет единицу";
            sresult += "\n";
            return sresult;
        }

        private string iselfdual(int[,] matrix)
        {
            string sresult = "";
            int self_dual = 0;
            int size = matrix.GetLength(0) / 2; //делим таблицу пополам
            for (int i = 0; i < size; i++)
            {
                if (matrix[i, matrix.GetLength(1) - COLUMNADD] != matrix[matrix.GetUpperBound(0) - i, matrix.GetLength(1) - COLUMNADD])
                {
                    self_dual++;
                }
            }
            sresult += self_dual == size ? "Функция самодвойственна" : "Функция Не самодвойственна";
            sresult += "\n";
            return sresult;
        }

        private static string ilinear(int[,] matrix)
        {
            string sresult = "";
            int ilinear = 0;
            for (int i = 1; i < matrix.GetLength(0); i++)
            {
                int count = 0;
                //проходимся по каждому элементу в строке (кроме добавленных столбцов)
                for (int j = 0; j < matrix.GetLength(1) - COLUMNADD; j++)
                {
                    if (matrix[i, j] == 1)
                    {
                        count++;
                    }
                }
                if (count > 1 & matrix[i, matrix.GetLength(1) - 1] == 1)
                {
                    ilinear++;
                }
            }
            sresult += ilinear == 0 ? "Функция Линейна" : "Функция Нелинейна";
            sresult += "\n";
            return sresult;
        }

        private static string Jegalkin(int[,] matrix, string str_vector)
        {
            string sresult = "";
            //построим полином Жегалкина (метод неопределенных коэффициентов) - 1 строка
            //например f(01110110) = a0 Ꚛ a3·C Ꚛ a2·B Ꚛ a23·B·C Ꚛ a1·A Ꚛ a13·A·C Ꚛ a12·A·B Ꚛ a123·A·B·C)
            sresult += "\n\nПолином Жегалкина (метод неопределенных коэффициентов)\n";
            sresult += "f(" + str_vector + ") = a0 Ꚛ ";
            string str = "";
            //проходимся по каждой строке 
            for (int i = 1; i < matrix.GetLength(0); i++)
            {
                string strnum = "";
                sresult += "a";
                //проходимся по каждому элементу в строке (кроме добавленных для f и после него)
                for (int j = 0; j < matrix.GetLength(1) - COLUMNADD; j++)
                {
                    if (matrix[i, j] == 1)
                    {
                        strnum += j + 1;
                        str += Convert.ToChar('A' + j);
                    }
                }
                sresult += strnum + "·";
                for (int k = 0; k < str.Length; k++)
                {
                    sresult += str[k];
                    if (k < str.Length - 1)
                    {
                        sresult += "·";
                    }
                }
                str = "";
                if (i < matrix.GetLength(0) - 1)
                {
                    sresult += " Ꚛ ";
                }
            }

            //записываем в матрицу неопределенные коэффициенты
            matrix[0, matrix.GetLength(1) - 1] = matrix[0, matrix.GetLength(1) - COLUMNADD];    //первый неизвестный коэффициент а0 равен f

            //решение. нахождение неопределенных коэффициентов
            //проходимся по массиву построчно
            sresult += "\n\n";
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                // 1)заголовок строки
                //f(001) = ...
                sresult += "f(";
                for (int h = 0; h < matrix.GetLength(1) - COLUMNADD; h++)
                {
                    sresult += matrix[i, h];
                }
                sresult += ") = ";

                //генерация строки элементов имеющих значение 1
                //напр. 0111 = 234
                // 2)вывод первой строки (000)
                if (i == 0)
                {
                    sresult += "a0 = " + matrix[i, matrix.GetLength(1) - COLUMNADD] + '\n';    //a0 = 1 или 0
                }
                // 2)вывод строк следующих за первой (за нулевой 000)
                string strnum = "";
                if (i != 0)
                {
                    sresult += "a0";
                    for (int j = 0; j < matrix.GetLength(1) - COLUMNADD; j++)
                    {
                        if (matrix[i, j] == 1)
                        {
                            strnum += j + 1;
                        }
                    }

                    //блок генерации сочетаний из элементов для подробного
                    //описания решения полинома методом неизвестных коэф.
                    //напр. ДЛЯ 0111 = 234
                    //по 1 эл = 2 3 4
                    //по 2 эл = 23 24 34
                    //по 3 эл = 234
                    ArrayList alistResult = new ArrayList();
                    //здесь k это значение по сколько элементов нужно собирать сочетание из строки
                    for (int k = 1; k < (matrix.GetLength(1)); k++)
                    {
                        if (strnum.Length >= k)
                        {
                            //получение всех комбинаций в этой строке (из 001 = 3, 010 = 2, 111 = 123, 110 = 12)
                            alistResult.AddRange(getCombinations(strnum, strnum.Length, k));
                        }
                    }

                    foreach (string s in alistResult)
                    {
                        sresult += " Ꚛ a";
                        sresult += s;
                    }

                    //в каждой строке повторяющееся значение это a0 и значение функции f. XOR(a0;f)
                    int res = matrix[0, matrix.GetLength(1) - COLUMNADD] ^ matrix[i, matrix.GetLength(1) - COLUMNADD];    

                    int a = Fxor(alistResult, res, matrix);
                    string b = alistResult[alistResult.Count - 1].ToString();

                    int cell = Fstrtonum(b, matrix.GetLength(1) - COLUMNADD);   //в какую ячейку матрицы записывать значение
                    matrix[cell, matrix.GetLength(1) - 1] = a;                  //запись значения коэф в матрицу

                    sresult += " = " + matrix[i, matrix.GetLength(1) - COLUMNADD];     // + f от (001)

                    string m = alistResult[alistResult.Count - 1].ToString();
                    sresult += " => a" + m + " = " + matrix[i, matrix.GetLength(1) - 1];

                    sresult += "\n";
                }
            }

            sresult += "\n";
            bool firstsymbol = false;
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                if (i == 0 && matrix[0, matrix.GetLength(1) - 1] == 1)
                {
                    sresult += "1";
                    firstsymbol = true;
                }
                string str2 = "";
                if (matrix[i, matrix.GetLength(1) - 1] == 1)
                {
                    for (int j = 0; j < matrix.GetLength(1) - COLUMNADD; j++)
                    {
                        if (matrix[i, j] == 1)
                        {
                            str2 += Convert.ToChar('A' + j);
                        }
                    }
                }
                if (str2 != "")
                {
                    if (firstsymbol == true)
                    {
                        sresult += " Ꚛ ";
                    }
                    sresult += str2;
                    firstsymbol = true;
                }
            }
            sresult += "\n";
            return sresult;
        }

        private static int Fstrtonum(string str, int column)
        {
            int result;
            string sres = "";
            for (int i = 0, n = 1, path = 0; i + n < column + 1; path = 0)
            {
                if (i < str.Length)
                {
                    if (int.Parse(str[i].ToString()) == (i + n))
                    {
                        sres += "1";
                        i++;
                        path = 1;
                    }
                }
                if ((sres.Length < column) & (path == 0))
                {
                    sres += "0";
                    n++;
                }
            }
            result = Convert.ToInt32(sres, 2);
            return result;
        }

        /// <summary>
        /// рекурсивная функция вычисления неизвестного коэффициента
        /// неизв коэф всегда последний, 
        /// напр(f(101) = a0 Ꚛ a1 Ꚛ a3 Ꚛ a13 = 0 => a13 = 0)
        /// (a0 Ꚛ a1 Ꚛ a2 Ꚛ a3 Ꚛ a12 Ꚛ a13 Ꚛ a23 Ꚛ a123 = 1 => a123 = 1)
        /// все предыдущие коэй к этому шагу уже вычислены!
        /// </summary>
        /// <param name="aresult"></param>
        /// <param name="res"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        private static int Fxor(ArrayList aresult, int res, int[,] matrix)
        {
            int result;
            if (aresult.Count == 1)
            {
                result = res;
            }
            else
            {
                string temp = aresult[0].ToString();
                aresult.RemoveAt(0);
                int a = Fstrtonum(temp, matrix.GetLength(1) - 2);
                result = matrix[a, matrix.GetLength(1) - 1] ^ Fxor(aresult, res, matrix);
            }
            return result;
        }

        /// <summary>
        /// Сочетание из количества элементов без повторений
        /// </summary>
        /// <param name="word"></param>
        /// <param name="size"></param>
        /// <param name="komb"></param>
        /// <returns></returns>
        private static ArrayList getCombinations(string word, int size, int komb)
        {
            ArrayList result = new ArrayList();
            string tempword = word;

            if (komb == 1)
            {
                //перечисление по одному элементу
                for (int i = 0; i < size; i++)
                {
                    result.Add(Convert.ToString(word[i]));
                }
            }
            else if (komb >= 2)
            {
                //сочетания из двух и более элементов
                for (int i = 0; i < size; i++)
                {
                    char c = word[i];
                    if (word.Length >= 2)
                    {
                        tempword = tempword.Remove(0, 1);
                        ArrayList result2 = new ArrayList();
                        result2 = getCombinations(tempword, tempword.Length, komb - 1);
                        for (int j = 0; j < result2.Count; j++)
                        {
                            result.Add(Convert.ToString(c) + result2[j]);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// проверка переменных(аргументов) на существенность
        /// </summary>
        /// <param name="matrix"></param>
        private string print_importance(int[,] matrix)
        {
            string sresult = "";
            bool equal;
            string text = "";
            int arg = matrix.GetLength(1) - COLUMNADD;
            sresult += "\n\n";
            sresult += "Существенность переменных\n";
            for (int i = 1, ar = 0; i <= arg; i++, ar++)
            {
                if (matrix[0, arg] == matrix[Convert.ToInt32(Math.Pow(2.0, Convert.ToDouble((arg - i)))), arg])
                    equal = true;
                else
                    equal = false;

                for (int j = 0; j < arg; j++)
                {
                    text += matrix[0, j];
                }
                text += "=" + matrix[0, arg];

                if (equal)
                    text += " = ";
                else
                    text += " ≠ ";

                int line = Convert.ToInt32(Math.Pow(2.0, Convert.ToDouble((arg - i))));
                for (int j = 0; j < arg; j++)
                {
                    text += matrix[line, j];
                }
                text += "=" + matrix[line, arg];

                if (equal)
                {
                    sresult += text + " переменная " + Convert.ToChar('A' + ar) + " несущественна\n";
                }
                else
                {
                    sresult += text + " переменная " + Convert.ToChar('A' + ar) + " существенна\n";
                }
                text = "";
            }
            return sresult;
        }

        /// <summary>
        /// Вывести таблицу истинности в строку
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        private string print_matrix(int[,] matrix)
        {
            string sresult = "";
            int LASTELEMENTROW = matrix.GetUpperBound(1);
            //выводим заголовок
            for (int i = 0; i < LASTELEMENTROW - 1; i++)
            {
                sresult += Convert.ToChar('A' + i);
            }
            sresult += " f\n";

            for (int i = 0; i < matrix.GetUpperBound(0) + 1; i++)
            {
                int[] arr = num2bin(i, LASTELEMENTROW - 1);
                for (int j = 0; j < LASTELEMENTROW - 1; j++)
                {
                    sresult += Convert.ToString(matrix[i, j]);
                }
                sresult += ' ' + Convert.ToString(matrix[i, LASTELEMENTROW - 1]);
                sresult += "\n";
            }
            return sresult;
        }

        /// <summary>
        /// создание таблицы истинности. думерный массив
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="str_vector"></param>
        /// <returns></returns>
        private int[,] create_matrix(int row, int column, string str_vector)
        {
            //создание матрицы аргументы + f + неопред. коэфициенты
            int[,] matrix = new int[row, column];
            //заполнение матрицы
            for (int i = 0; i < row; i++)
            {
                int[] arr = num2bin(i, column - COLUMNADD);
                for (int j = 0; j < column - COLUMNADD; j++)
                {
                    matrix[i, j] = arr[j];
                }
                matrix[i, column - COLUMNADD] = Convert.ToChar(str_vector[i] - '0');
            }
            return matrix;
        }

        /// <summary>
        /// функция вычисления СДНФ по таблице истинности
        /// </summary>
        /// <param name="matrix">сама таблица истинности + столбец результат функции</param>
        /// <param name="row">кол строк</param>
        /// <param name="column">кол столбцов</param>
        /// <returns></returns>
        private string print_sdnf( int[,] matrix, int row, int column)
        {
            string sresult = "";
            //Совершенная ДНФ и КНФ
            //построение СДНФ по таблице истинности (matrix)
            sresult += "СДНФ для функции\n";
            for (int i = 0, k = 0; i < row; i++)
            {
                if (matrix[i, column] == 1)
                {
                    if (k > 0)
                    {
                        sresult += " v ";
                    }

                    for (int j = 0; j < column; j++)
                    {
                        if (matrix[i, j] < 1)
                        {
                            sresult += '⌐';
                        }
                        sresult += Convert.ToChar('A' + j);
                        k++;
                    }
                }
            }
            sresult += "\n\n";
            return sresult;
        }

        /// <summary>
        /// функция вычисления СКНФ по таблице истинности
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private string print_sknf(int[,] matrix, int row, int column)
        {
            string sresult = "";
            sresult += "СКНФ для функции\n";
            bool issecond = false;
            for (int i = 0, k = 0; i < row; i++, k = 0)
            {
                if (matrix[i, column] == 0)
                {
                    if (issecond)
                        sresult += " ^ ";

                    sresult += "(";
                    for (int j = 0; j < column; j++)
                    {
                        if (k < column && k > 0)
                        {
                            sresult += " v ";
                        }
                        if (matrix[i, j] == 1)
                        {
                            sresult += '⌐';
                        }
                        sresult += Convert.ToChar('A' + j);
                        k++;
                    }
                    sresult += ")";
                    issecond = true;
                }
            }
            return sresult;
        }

        /// <summary>
        /// вычисление количества аргументов функции
        /// </summary>
        /// <returns></returns>
        private int CountArguments(string sourceString)
        {
            int i;
            int length = sourceString.Length;
            for (i = 0; length > 1; i++)
            {
                length /= 2;
            }
            return i;
        }

        /// <summary>
        /// переводит число в двоичную систему и записывает в одномерный массив
        /// </summary>
        /// <param name="num"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private int[] num2bin(int num, int count)
        {
            string s = Convert.ToString(num, 2);    //Преобразует значение заданного 32-битового целого числа со знаком в эквивалентное строковое представление в указанной системе счисления.
            if (s.Length < count)
            {
                string zero = new string('0', count - s.Length);
                string result = string.Concat(zero, s);
                int[] arr = new int[result.Length];
                for (int i = 0; i < result.Length; i++)
                {
                    arr[i] = Convert.ToInt32(result[i] - '0');
                }
                return arr;
            }
            else
            {
                int[] arr = new int[count];
                for (int i = 0; i < count; i++)
                {
                    arr[i] = Convert.ToInt32(s[i] - '0');
                }
                return arr;
            }
        }
    }
}
