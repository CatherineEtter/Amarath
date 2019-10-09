using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amarath.Models;

//This is an example model used to communicate with the SQL Database. All models must be defined in DAL/Data/AmarathContext.cs
namespace Amarath.DAL.Data
{
    public interface ICustomerRepository
    {
        Customer GetCustomerByID(int CustomerId);
        Customer Add(Customer customer);
        Customer Update(Customer customer);
        Customer Delete(int CustomerId);
        IEnumerable<Customer> GetAllCustomer();
        void Save();
    }
    public class CustomerRepository : ICustomerRepository
    {
        private AmarathContext context;
        
        public CustomerRepository(AmarathContext context)
        {
            this.context = context;
        }
        public Customer Add(Customer customer)
        {
            context.Customers.Add(customer);
            context.SaveChanges();
            return customer;
        }

        public Customer Delete(int CustomerId)
        {
            Customer customer = context.Customers.Find(CustomerId);
            if(customer != null)
            {
                context.Customers.Remove(customer);
                context.SaveChanges();
            }
            return customer;
        }

        public IEnumerable<Customer> GetAllCustomer()
        {
            return context.Customers.ToList();
        }

        public Customer GetCustomerByID(int CustomerId)
        {
            return context.Customers.Find(CustomerId);
        }

        public Customer Update(Customer customerChanges)
        {
            var customer = context.Customers.Attach(customerChanges);
            customer.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return customerChanges;
        }
        public void Save()
        {
            throw new NotImplementedException(); //TODO: Implement
        }
    }
}