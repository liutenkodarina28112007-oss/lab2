using System;
using System.Collections.Generic;

namespace Lab
{
    /// <summary>
    /// Завдання 1. Клас "Товар" (варіант 3).
    /// Поля: код, назва, виробник, вартість, кількість у партії.
    /// Методи: загальна вартість партії, збільшення вартості на відсоток, виведення інформації.
    /// Це клас (reference type), тому його можна зберігати в бінарному дереві з обмеженням where T : class.
    /// </summary>
    public class Product : IComparable<Product>
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public Product(int code, string name, string manufacturer, decimal price, int quantity)
        {
            Code = code;
            Name = name;
            Manufacturer = manufacturer;
            Price = price;
            Quantity = quantity;
        }

        /// <summary>Загальна вартість партії = вартість одиниці * кількість.</summary>
        public decimal GetTotalCost() => Price * Quantity;

        /// <summary>Збільшення вартості товару на певний відсоток.</summary>
        public void IncreasePrice(double percent)
        {
            if (percent < 0)
                throw new ArgumentOutOfRangeException(nameof(percent), "Відсоток не може бути від'ємним.");
            Price = Math.Round(Price * (1 + (decimal)percent / 100m), 2);
        }

        /// <summary>Виведення інформації про товар.</summary>
        public void PrintInfo()
        {
            Console.WriteLine(ToString());
        }

        public override string ToString() =>
            $"[{Code,3}] {Name,-12} | {Manufacturer,-10} | ціна: {Price,9:F2} | к-сть: {Quantity,4} | партія: {GetTotalCost(),10:F2}";

        // Порівняння за замовчуванням (IComparable<T>) - за кодом товару.
        public int CompareTo(Product other)
        {
            if (other is null) return 1;
            return Code.CompareTo(other.Code);
        }
    }

    // ---------- Завдання 4: компаратори (IComparer<T>) ----------

    /// <summary>Порівняння за назвою (за алфавітом).</summary>
    public class ProductNameComparer : IComparer<Product>
    {
        public int Compare(Product x, Product y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x is null) return -1;
            if (y is null) return 1;
            return string.Compare(x.Name, y.Name, StringComparison.CurrentCulture);
        }
    }

    /// <summary>Порівняння за вартістю одиниці товару.</summary>
    public class ProductPriceComparer : IComparer<Product>
    {
        public int Compare(Product x, Product y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x is null) return -1;
            if (y is null) return 1;
            return x.Price.CompareTo(y.Price);
        }
    }

    /// <summary>Порівняння за загальною вартістю партії.</summary>
    public class ProductTotalCostComparer : IComparer<Product>
    {
        public int Compare(Product x, Product y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x is null) return -1;
            if (y is null) return 1;
            return x.GetTotalCost().CompareTo(y.GetTotalCost());
        }
    }

    /// <summary>Необщий (non-generic) компаратор для ArrayList.Sort (IComparer).</summary>
    public class ProductNonGenericComparer : System.Collections.IComparer
    {
        public int Compare(object x, object y) =>
            new ProductPriceComparer().Compare(x as Product, y as Product);
    }
}
