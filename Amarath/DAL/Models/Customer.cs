using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/*
 * This is an example of how to setup each entity.
 * TODO: Delete when the actual database is initialized
 */
namespace Amarath.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public int NameStyle { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string CompanyName { get; set; }
        public string SalesPerson { get; set; }
        public string EmailAddress { get; set; }
        public string Phone { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public Guid rowguid { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
