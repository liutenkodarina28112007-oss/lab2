using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab
{
    internal static class Program
    {
        private static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            Task1();
            Task2();
            Task3();
            Task4();
        }

        private static void Header(string text)
        {
            Console.WriteLine();
            Console.WriteLine(new string('=', 90));
            Console.WriteLine(text);
            Console.WriteLine(new string('=', 90));
        }

        private static Product[] Sample() => new[]
        {
            new Product(5, "Молоко",   "Галичина", 32.50m, 100),
            new Product(2, "Хліб",     "Київхліб", 24.00m, 200),
            new Product(8, "Сир",      "Яготин",   180.00m, 30),
            new Product(1, "Масло",    "Тульчин",  95.90m, 50),
            new Product(4, "Йогурт",   "Данон",    21.30m, 150),
            new Product(7, "Яблука",   "Агро",     38.00m, 80),
            new Product(3, "Кефір",    "Галичина", 29.90m, 120),
        };

        // ---------------- Завдання 1 ----------------
        private static void Task1()
        {
            Header("ЗАВДАННЯ 1. Клас Product (варіант 3)");
            var p = new Product(10, "Цукор", "Сумицукор", 40.00m, 25);
            p.PrintInfo();
            Console.WriteLine($"Загальна вартість партії: {p.GetTotalCost():F2}");
            p.IncreasePrice(10);
            Console.WriteLine("Після збільшення вартості на 10%:");
            p.PrintInfo();
        }

        // ---------------- Завдання 2 ----------------
        private static void Task2()
        {
            Header("ЗАВДАННЯ 2. Колекції та масиви");

            // --- 2.1 Узагальнена колекція List<Product> ---
            Console.WriteLine("\n--- List<Product> (generic) ---");
            var list = new List<Product>(Sample());
            list.Add(new Product(9, "Чай", "Лісова", 60m, 40));                // додавання
            Console.WriteLine("Після додавання:");
            list.ForEach(Console.WriteLine);                                    // прохід

            list.RemoveAll(x => x.Code == 2);                                   // видалення
            Console.WriteLine("\nПісля видалення товару з кодом 2: " + list.Count + " елементів");

            var found = list.Find(x => x.Name == "Сир");                        // пошук
            Console.WriteLine("Пошук 'Сир': " + (found?.ToString() ?? "не знайдено"));

            int idx = list.FindIndex(x => x.Code == 4);                         // оновлення
            list[idx] = new Product(4, "Йогурт грецький", "Данон", 35m, 90);
            Console.WriteLine("Оновлення елемента [" + idx + "]: " + list[idx]);

            // --- 2.2 Необ'єктна (non-generic) колекція ArrayList ---
            Console.WriteLine("\n--- ArrayList (non-generic) ---");
            var arrList = new ArrayList(Sample());
            arrList.Add(new Product(9, "Чай", "Лісова", 60m, 40));
            arrList.Add("Рядок у колекції товарів!");                           // компілятор дозволяє будь-який тип
            Console.WriteLine("Після додавання (разом із стороннім рядком): " + arrList.Count + " елементів");

            arrList.RemoveAt(1);                                                // видалення за індексом
            int pos = -1;
            for (int i = 0; i < arrList.Count; i++)                             // пошук
                if (arrList[i] is Product pr && pr.Name == "Сир") { pos = i; break; }
            Console.WriteLine("Індекс 'Сир': " + pos);

            arrList[0] = new Product(5, "Молоко 2.5%", "Галичина", 34m, 100);   // оновлення
            Console.WriteLine("Оновлено [0]: " + arrList[0]);

            Console.WriteLine("Прохід через foreach (потрібне приведення типів):");
            foreach (object o in arrList)
            {
                if (o is Product pp) Console.WriteLine("  " + pp);
                else Console.WriteLine("  (не Product) " + o);
            }

            try
            {
                foreach (Product pp in arrList)   // неявне приведення object -> Product
                    _ = pp.Name;
            }
            catch (InvalidCastException ex)
            {
                Console.WriteLine("InvalidCastException у ArrayList під час виконання: " + ex.Message);
            }

            // --- 2.3 Масив Product[] ---
            Console.WriteLine("\n--- Product[] (масив) ---");
            Product[] arr = Sample();
            Console.WriteLine("Довжина масиву: " + arr.Length + " (фіксована)");

            // "додавання" = створення нового масиву більшого розміру
            Array.Resize(ref arr, arr.Length + 1);
            arr[arr.Length - 1] = new Product(9, "Чай", "Лісова", 60m, 40);
            Console.WriteLine("Після Array.Resize та додавання: " + arr.Length);

            // "видалення" = копіювання без елемента
            arr = arr.Where(x => x.Code != 2).ToArray();
            Console.WriteLine("Після видалення (перестворення масиву): " + arr.Length);

            int ai = Array.FindIndex(arr, x => x.Name == "Сир");                // пошук
            Console.WriteLine("Індекс 'Сир': " + ai);
            arr[0] = new Product(5, "Молоко 3.2%", "Галичина", 36m, 100);       // оновлення
            Console.WriteLine("Оновлено [0]: " + arr[0]);
            foreach (var x in arr) Console.WriteLine("  " + x);

            Console.WriteLine(@"
ПОРІВНЯННЯ ТА ВІДМІННОСТІ:
 * List<Product>  - типобезпечна, динамічний розмір, зручні Add/Remove/Find/Sort. Без boxing/приведення типів.
                    Помилка типу виявляється на етапі компіляції.
 * ArrayList      - зберігає object: можна додати елементи будь-якого типу, потрібне приведення при читанні,
                    можливий InvalidCastException під час виконання; для типів-значень додатково boxing/unboxing.
 * Product[]      - фіксований розмір (зміна = створення нового масиву), найшвидший доступ за індексом,
                    немає вбудованих Add/Remove; типобезпечний.");
        }

        // ---------------- Завдання 3 ----------------
        private static void Task3()
        {
            Header("ЗАВДАННЯ 3. Узагальнений бінарний тип BinaryTree<T> where T : class");

            var tree = new BinaryTree<Product>();       // порівняння за замовчуванням - IComparable (за кодом)
            foreach (var p in Sample()) tree.Add(p);
            Console.WriteLine("Елементів у дереві: " + tree.Count);
            Console.WriteLine("Contains(код 8): " + tree.Contains(new Product(8, "", "", 0, 0)));
            Console.WriteLine("Contains(код 99): " + tree.Contains(new Product(99, "", "", 0, 0)));
            Console.WriteLine("Дублікат (код 5) додано? " + tree.Add(new Product(5, "Дубль", "-", 1, 1)));

            Console.WriteLine("Видалення коду 5: " + tree.Remove(new Product(5, "", "", 0, 0)));
            Console.WriteLine("Елементів: " + tree.Count);

            // Дерево працює з будь-яким reference-типом, не лише з Product:
            var strTree = new BinaryTree<string>();
            foreach (var s in new[] { "slon", "babak", "vovk", "akula" }) strTree.Add(s);
            Console.WriteLine("BinaryTree<string>: " + string.Join(", ", strTree));

            // BinaryTree<int> не скомпілюється: int - структура (обмеження where T : class).
            Console.WriteLine("BinaryTree<int> не компілюється (обмеження where T : class).");
        }

        // ---------------- Завдання 4 ----------------
        private static void Task4()
        {
            Header("ЗАВДАННЯ 4. Порівняння (IComparable / IComparer) та власний ітератор");

            // 4.1 Обхід за IComparable<Product> (за кодом) - центрований порядок
            var byCode = new BinaryTree<Product>();
            foreach (var p in Sample()) byCode.Add(p);
            Console.WriteLine("\nЦентрований обхід (inorder), IComparable<Product> - за кодом:");
            foreach (var p in byCode) Console.WriteLine("  " + p);

            // 4.2 Різні IComparer<Product>
            var byName = new BinaryTree<Product>(new ProductNameComparer());
            var byPrice = new BinaryTree<Product>(new ProductPriceComparer());
            var byTotal = new BinaryTree<Product>(new ProductTotalCostComparer());
            foreach (var p in Sample()) { byName.Add(p); byPrice.Add(p); byTotal.Add(p); }

            Console.WriteLine("\nInorder, IComparer - за назвою:");
            foreach (var p in byName) Console.WriteLine("  " + p);
            Console.WriteLine("\nInorder, IComparer - за ціною:");
            foreach (var p in byPrice) Console.WriteLine("  " + p);
            Console.WriteLine("\nInorder, IComparer - за вартістю партії:");
            foreach (var p in byTotal) Console.WriteLine("  " + p);

            // 4.3 Функціонал порівняння через методи сортування
            Console.WriteLine("\n--- Сортування через інтерфейси порівняння ---");

            var arr = Sample();
            Array.Sort(arr);                                    // IComparable<T>
            Console.WriteLine("Array.Sort(arr) - IComparable (код):  " + string.Join(", ", arr.Select(p => p.Code)));

            Array.Sort(arr, new ProductNameComparer());         // IComparer<T>
            Console.WriteLine("Array.Sort(arr, NameComparer):        " + string.Join(", ", arr.Select(p => p.Name)));

            var list = new List<Product>(Sample());
            list.Sort(new ProductPriceComparer());              // IComparer<T>
            Console.WriteLine("List.Sort(PriceComparer) (ціна):      " + string.Join(", ", list.Select(p => p.Price)));

            list.Sort();                                        // IComparable<T>
            Console.WriteLine("List.Sort() - IComparable (код):      " + string.Join(", ", list.Select(p => p.Code)));

            list.Sort((a, b) => b.Quantity.CompareTo(a.Quantity));   // Comparison<T>
            Console.WriteLine("List.Sort(Comparison) (к-сть, спад.): " + string.Join(", ", list.Select(p => p.Quantity)));

            var al = new ArrayList(Sample());
            al.Sort(new ProductNonGenericComparer());           // non-generic IComparer
            Console.WriteLine("ArrayList.Sort(IComparer) (ціна):     " +
                              string.Join(", ", al.Cast<Product>().Select(p => p.Price)));

            // 4.4 Власний ітератор у явному вигляді (IEnumerator)
            Console.WriteLine("\n--- Явне використання IEnumerator<T> ---");
            using (IEnumerator<Product> e = byCode.GetEnumerator())
            {
                while (e.MoveNext()) Console.Write(e.Current.Code + " ");
                Console.WriteLine();
                e.Reset();
                e.MoveNext();
                Console.WriteLine("Після Reset() перший елемент: код " + e.Current.Code);
            }

            // Перевірка, що inorder справді відсортований
            bool sorted = byCode.Zip(byCode.Skip(1), (a, b) => a.Code < b.Code).All(x => x);
            Console.WriteLine("Центрований обхід дає відсортовану послідовність: " + sorted);

            // 4.5 Додаткові дії над товарами: збільшення ціни
            Console.WriteLine("\n--- Збільшення вартості на 10% для всіх товарів у дереві ---");
            foreach (var p in byCode) p.IncreasePrice(10);
            foreach (var p in byCode) Console.WriteLine("  " + p);
            Console.WriteLine("Сумарна вартість усіх партій: " + byCode.Sum(p => p.GetTotalCost()).ToString("F2"));
        }
    }
}
