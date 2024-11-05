using System.Collections.Generic;
using System.Linq;
using System;

using NUnit.Framework;
using MMABooksEFClasses.MarisModels;
using Microsoft.EntityFrameworkCore;

namespace MMABooksTests
{
    [TestFixture]
    public class CustomerTests
    {      
        MMABooksContext dbContext;
        Customer? c;
        List<Customer>? customers;

        [SetUp]
        public void Setup()
        {
            dbContext = new MMABooksContext();
            dbContext.Database.ExecuteSqlRaw("call usp_testingResetData()");
        }

        [Test]
        public void GetAllTest()
        {
            customers = dbContext.Customers.OrderBy(c => c.Name).ToList();
            Assert.AreEqual(696, customers.Count);
            Assert.AreEqual("Abeyatunge, Derek", customers[0].Name);
            PrintAll(customers);
        }

        [Test]
        public void GetByPrimaryKeyTest()
        {
            c = dbContext.Customers.FirstOrDefault(c => c.Name == "Abeyatunge, Derek");
            Assert.IsNotNull(c);
            Assert.AreEqual(157, c.CustomerId); 
            Console.WriteLine(c.CustomerId);

        }

        [Test]
        public void GetUsingWhere()
        {
            // get a list of all of the customers who live in OR
            customers = dbContext.Customers.Where(c => c.StateCode == "OR").OrderBy(c => c.Name).ToList();
            Assert.AreEqual(5, customers.Count);
            Assert.AreEqual("OR", customers[0].StateCode);
            PrintAll(customers);

        }

        [Test]
        public void GetWithInvoicesTest()
        {
            // get the customer whose id is 20 and all of the invoices for that customer
            c = dbContext.Customers.Include(customer => customer.Invoices).Where(c => c.CustomerId == 20).SingleOrDefault();
            Assert.IsNotNull(c);
            Assert.AreEqual("Chamberland, Sarah", c.Name);
            Assert.AreEqual("1942 S. Gaydon Avenue", c.Address);
            Console.WriteLine(c);
        }

        [Test]
        public void GetWithJoinTest()
        {
            // get a list of objects that include the customer id, name, statecode and statename
            var customers = dbContext.Customers.Join(
               dbContext.States,
               c => c.StateCode,
               s => s.StateCode,
               (c, s) => new { c.CustomerId, c.Name, c.StateCode, s.StateName }).OrderBy(r => r.StateName).ToList();
            Assert.AreEqual(696, customers.Count);
            // I wouldn't normally print here but this lets you see what each object looks like
            foreach (var c in customers)
            {
                Console.WriteLine(c);
            }
        }

        [Test]
        public void DeleteTest()
        {
            c = dbContext.Customers.Find(1);
            dbContext.Customers.Remove(c);
            dbContext.SaveChanges();
            Assert.IsNull(dbContext.Customers.Find(1));
        }

        [Test]
        public void CreateTest()
        {

        }

        [Test]
        public void UpdateTest()
        {

        }

        public void PrintAll(List<Customer> customers)
        {
            foreach (Customer c in customers)
            {
                Console.WriteLine(c);
            }
        }
        
    }
}