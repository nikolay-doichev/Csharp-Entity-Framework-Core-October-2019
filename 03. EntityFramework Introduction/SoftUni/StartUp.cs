using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Linq;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            SoftUniContext context = new SoftUniContext();

            string result = GetAddressesByTown(context);

            Console.WriteLine(result);
        }

        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                                     .Select(e => new
                                     {
                                         Id = e.EmployeeId,
                                         Name = String.Join(" ", e.FirstName, e.LastName, e.MiddleName),
                                         e.JobTitle,
                                         e.Salary
                                     })
                                     .OrderBy(e => e.Id);

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.Name} {e.JobTitle} {e.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 04
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .Where(e => e.Salary > 50_000)
                .OrderBy(e => e.FirstName)
                .Select(e => new
                {
                    FirstName = e.FirstName,
                    Salary = e.Salary
                });

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} - {e.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 05
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .Where(e => e.Department.Name == "Research and Development")
                .OrderBy(e => e.Salary)
                .ThenByDescending(e => e.FirstName)
                .Select(e => new
                {
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    DepatmentName = e.Department.Name,
                    Salary = e.Salary
                });

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} from {e.DepatmentName} - ${e.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 06
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var adress = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            context.Addresses.Add(adress);

            Employee nakov = context.Employees
                .First(e => e.LastName == "Nakov");

            nakov.Address = adress;

            context.SaveChanges();

            var employees = context.Employees
                .OrderByDescending(e => e.AddressId)
                 .Select(a => a.Address.AddressText)
                 .Take(10);


            foreach (var a in employees)
            {
                sb.AppendLine(a);
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 07
        //public static string GetEmployeesInPeriod(SoftUniContext context)
        //{
        //    StringBuilder sb = new StringBuilder();

        //    var employees = context.Projects
        //        .Where(p => p.StartDate.Year >= 2001 && p.StartDate.Year <= 2003);
        //}       

        //Problem 08
        public static string GetAddressesByTown(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var adresses = context.Addresses
                .Select(a => new
                {
                    AddressText = a.AddressText,
                    TownName = a.Town.Name,
                    EmployeesCount = a.Employees.Count
                })
                .OrderByDescending(e => e.EmployeesCount)
                .ThenBy(e => e.TownName)
                .ThenBy(e => e.AddressText)
                .Take(10)
                .ToList();

            foreach (var a in adresses)
            {
                sb.AppendLine($"{a.AddressText}, {a.TownName} - {a.EmployeesCount} employees");
            }
            return sb.ToString().TrimEnd();
        }

        //Problem 09
        public static string GetEmployee147(SoftUniContext context)
        {
            return "";
        }

        //Problem 12
        public static string IncreaseSalaries(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .Where(e => e.Department.Name == "Engineering" ||
                            e.Department.Name == "Tool Design" ||
                            e.Department.Name == "Marketing" ||
                            e.Department.Name == "Information Services");

            foreach (var employee in employees)
            {
                employee.Salary *= 1.12m;
            }

            context.SaveChanges();

            var employeesToPrint = employees
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.Salary
                })
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName);

            foreach (var e in employeesToPrint)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} (${e.Salary:F2})");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 13
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .Where(e => e.FirstName.StartsWith("Sa"))
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    e.Salary
                })
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToList();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary:F2})");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 15
        public static string RemoveTown(SoftUniContext context)
        {
            var seattle = context
                .Towns
                .Where(t => t.Name == "Seattle")
                .First();

            var adressInTown = context
                .Addresses
                .Where(a => a.Town == seattle);

            var employeesToRemoveAdress = context
                .Employees
                .Where(e => adressInTown.Contains(e.Address));

            foreach (var e in employeesToRemoveAdress)
            {
                e.AddressId = null;
            }

            context.Addresses.RemoveRange(adressInTown);
            //foreach (var a in adressInTown)
            //{
            //    context.Addresses.Remove(a);
            //}

            int addressCount = adressInTown.Count();
            context.Towns.Remove(seattle);

            context.SaveChanges();

            return $"{addressCount} addresses in Seattle were deleted";
        }
    }
}
