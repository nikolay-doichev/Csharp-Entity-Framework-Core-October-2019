using System;
using System.IO;
using System.Xml.Serialization;

using AutoMapper;

using ProductShop.Data;
using ProductShop.Models;
using ProductShop.Dtos.Import;
using System.Linq;
using AutoMapper.QueryableExtensions;
using ProductShop.Dtos.Export;
using System.Xml;
using System.Text;

namespace ProductShop
{
    public class StartUp
    {
        
        public static void Main(string[] args)
        {
            Mapper.Initialize(cfg =>
                                cfg.AddProfile<ProductShopProfile>());

            using (var db = new ProductShopContext())
            {                
                //string inputJson = File.ReadAllText("./../../../Datasets/categories-products.xml");

                string result = GetUsersWithProducts(db);

                Console.WriteLine(result);
            }
        }

        //01. Import Users
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportUserDto[]),
                new XmlRootAttribute("Users"));

            ImportUserDto[] userDtos;

            using (var reader = new StringReader(inputXml))
            {
                userDtos = (ImportUserDto[])xmlSerializer.Deserialize(reader);
            }

            var users = Mapper.Map<User[]>(userDtos);

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Length}";
        }

        //02. Import Prodicts
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportProductDto[]),
               new XmlRootAttribute("Products"));

            ImportProductDto[] productDtos;

            using (var reader = new StringReader(inputXml))
            {
                productDtos = (ImportProductDto[])xmlSerializer.Deserialize(reader);
            }

            var products = Mapper.Map<Product[]>(productDtos);

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Length}";
        }

        //03. Import Categories
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportCategoriesDto[]),
               new XmlRootAttribute("Categories"));

            ImportCategoriesDto[] categoriesDtos;

            using (var reader = new StringReader(inputXml))
            {
                categoriesDtos = (ImportCategoriesDto[])xmlSerializer.Deserialize(reader);
            }

            var categories = Mapper.Map<Category[]>(categoriesDtos);

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Length}";
        }

        //04. Import Categories and Products
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportCategoryProductsDto[]),
               new XmlRootAttribute("CategoryProducts"));

            ImportCategoryProductsDto[] categoryProductsDtos;

            using (var reader = new StringReader(inputXml))
            {
                categoryProductsDtos = ((ImportCategoryProductsDto[])xmlSerializer.Deserialize(reader))
                    .Where(c => context.Categories.Any(ct => ct.Id == c.CategoryId))
                    .Where(c => context.Products.Any(p => p.Id == c.ProductId))
                    .ToArray();
            }

            var categoryProducts = Mapper.Map<CategoryProduct[]>(categoryProductsDtos);

            context.CategoryProducts.AddRange(categoryProducts);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Length}";
        }

        //05. Export Products In Range
        public static string GetProductsInRange(ProductShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var products = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .Select(p => new ExportProductInRange
                {
                    Name = p.Name,
                    Price = p.Price,
                    BuyerName = $"{p.Buyer.FirstName} {p.Buyer.LastName}"
                })
                .OrderBy(p => p.Price)
                .Take(10)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportProductInRange[]),
                new XmlRootAttribute("Products"));

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            xmlSerializer.Serialize(new StringWriter(sb), products, namespaces);

            return sb.ToString().TrimEnd();
        }
        //06. Export Sold Products
        public static string GetSoldProducts(ProductShopContext context)
        {
            StringBuilder sb = new StringBuilder();
            var users = context.Users
                .Where(u => u.ProductsSold.Any(up => up.Buyer != null))                
                .Select(u => new ExportSoldProductsDto
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Product = u.ProductsSold.Select(ps => new ExportProductPraiceAndName
                    {
                        Name = ps.Name,
                        Price = ps.Price
                    }).ToArray()
                })
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Take(5)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportSoldProductsDto[]),
                new XmlRootAttribute("Users"));

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            xmlSerializer.Serialize(new StringWriter(sb), users, namespaces);

            return sb.ToString().TrimEnd();

        }

        //07. Export Categories By Products Count
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            StringBuilder sb = new StringBuilder();
            var categories = context.Categories
                .Select(c => new GetAllCategoriesAndTotalAvenue
                {
                    Name = c.Name,
                    Count = c.CategoryProducts.Count(),
                    AveragePrice = c.CategoryProducts.Average(cp => cp.Product.Price),
                    TotalRevenue = c.CategoryProducts.Sum(cp => cp.Product.Price)
                })
                .OrderByDescending(c => c.Count)
                .ThenBy(c => c.TotalRevenue)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(GetAllCategoriesAndTotalAvenue[]),
                new XmlRootAttribute("Categories"));

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            xmlSerializer.Serialize(new StringWriter(sb), categories, namespaces);

            return sb.ToString().TrimEnd();
        }

        //08. Users and Products
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var users = context.Users
                .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                .Select(u => new UsersAndProducts
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age.Value,
                    Product = u.ProductsSold.Select(ps => new ExportProductPraiceAndName
                    {
                        Name = ps.Name,
                        Price = ps.Price
                    }).ToArray()
                })
                .OrderBy(u => u.Product.Count())
                .Take(10)
                .ToArray();

            var usersOutput = new
            {
                usersCount = users.Count(),
                users = users
            };


            var xmlSerializer = new XmlSerializer(typeof(UsersAndProducts[]),
                new XmlRootAttribute("users"));

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            xmlSerializer.Serialize(new StringWriter(sb), users, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}