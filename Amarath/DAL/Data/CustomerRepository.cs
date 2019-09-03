using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amarath.Models;

namespace Amarath.DAL.Data
{
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
            return customer;
        }

        public Customer Delete(int CustomerId)
        {
            Customer customer = context.Customers.Find(CustomerId);
            context.Customers.Remove(customer);
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

        public Customer Update(Customer customer)
        {
            context.Entry(customer).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            return customer;
        }
        public void Save()
        {
            throw new NotImplementedException(); //TODO: Implement
        }
    }
}
 