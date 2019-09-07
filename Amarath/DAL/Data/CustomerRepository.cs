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
 