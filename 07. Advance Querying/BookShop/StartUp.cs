namespace BookShop
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    using Data;
    using Initializer;

    public class StartUp
    {
        public static void Main()
        {
            using (var db = new BookShopContext())
            {
                //DbInitializer.ResetDatabase(db);
                //string input = Console.ReadLine();

                string result = GetMostRecentBooks(db);

                Console.WriteLine(result);
            }
        }
        //Problem 01
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            StringBuilder sb = new StringBuilder();

            var books = context.Books
                .Where(b => b.AgeRestriction.ToString().ToLower() == command.ToLower())
                .Select(b => new
                {
                    b.Title
                })
                .OrderBy(b => b.Title)
                .ToList();

            foreach (var b in books)
            {
                sb.AppendLine(b.Title);
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 02
        public static string GetGoldenBooks(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var books = context.Books
                .Where(b => b.Copies < 5000 && b.EditionType.ToString() == "Gold")
                .Select(b => new 
                {
                    Id = b.BookId,
                    Title = b.Title
                })
                .OrderBy(b => b.Id);

            foreach (var b in books)
            {
                sb.AppendLine(b.Title);
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 03
        public static string GetBooksByPrice(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var books = context.Books
                .Where(b => b.Price > 40)
                .Select(b => new
                {
                    Title = b.Title,
                    Price = b.Price
                })
                .OrderByDescending(b => b.Price)
                .ToList();

            foreach (var b in books)
            {
                sb.AppendLine($"{b.Title} - ${b.Price:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 04
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            StringBuilder sb = new StringBuilder();

            var books = context.Books
                .Where(b => b.ReleaseDate.Value.Year != year)
                .Select(b => b.Title)
                .ToList();

            foreach (var b in books)
            {
                sb.AppendLine(b);
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 05
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            string[] categories = input.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.ToLower())
                .ToArray();

            var books = context.Books
                .Where(b => b.BookCategories.Any(bc =>
                categories.Contains(bc.Category.Name.ToLower())))
                .Select(b => b.Title)
                .OrderBy(b => b);

            return String.Join(Environment.NewLine, books);
        }

        //Problem 06
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            StringBuilder sb = new StringBuilder();

            var books = context.Books
                .Where(b => b.ReleaseDate < DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture))
                .Select(b => new
                {
                    Title = b.Title,
                    EditionType = b.EditionType.ToString(),
                    Price = b.Price,
                    ReleseDate = b.ReleaseDate
                })
                .OrderByDescending(b => b.ReleseDate)
                .ToList();

            foreach (var b in books)
            {
                sb.AppendLine($"{b.Title} - {b.EditionType} - ${b.Price:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 09
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            var booksAndAuthors = context.Books
                .Where(a => a.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .OrderBy(b => b.BookId)
                .Select(b => new
                {
                    b.Title,
                    Author = $"{b.Author.FirstName} {b.Author.LastName}"
                })
                .ToList();

            foreach (var b in booksAndAuthors)
            {
                sb.AppendLine($"{b.Title} ({b.Author})");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 11
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var authors = context.Authors
                .Select(a => new
                {
                    AuthorName = $"{a.FirstName} {a.LastName}",
                    Copies = a.Books.Sum(b => b.Copies)
                })
                .OrderByDescending(a => a.Copies)
                .ToList();

            foreach (var a in authors)
            {
                sb.AppendLine($"{a.AuthorName} - {a.Copies}");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 13
        public static string GetMostRecentBooks(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var categoriesAndMostRecentBooks = context.Categories
                .Select(c => new
                {
                    CategoryName = c.Name,
                    CategoryRecentBooks = c.CategoryBooks
                                            .OrderByDescending(cb => cb.Book.ReleaseDate)
                                            .Take(3)
                                            .Select(cb => new 
                                            {
                                                BookTitle = cb.Book.Title,
                                                BookRelese = cb.Book.ReleaseDate.Value.Year
                                            })
                                            .ToList()
                })
                .OrderBy(c => c.CategoryName)
                .ToList();

            foreach (var c in categoriesAndMostRecentBooks)
            {
                sb.AppendLine($"--{c.CategoryName}");
                foreach (var b in c.CategoryRecentBooks)
                {
                    sb.AppendLine($"{b.BookTitle} ({b.BookRelese})");
                }
            }

            return sb.ToString().TrimEnd();
        }
    }
}
