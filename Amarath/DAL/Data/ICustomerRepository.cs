/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amarath.Models;

*//*
 * This is the interface for the Customer Repository. These must be created for every repository and should have basic CRUD operations.
 *//*
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
}
*/