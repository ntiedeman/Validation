using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Validation
{
    public class Contact
    {
        public string Name { get; set; }

        public string City { get; set; }

        public string PhoneNumber { get; set; }

        public string EmailAddress { get; set; }

        public int ValidationErrors { get; private set; }

        public string ValidationMessage { get; private set; }

        private bool IsPhoneNumberValid()
        {
            Regex regEx = new Regex(@"^[- \d]*\d[- \d]*$");
            return regEx.IsMatch(PhoneNumber);
        }

        private bool IsEmailAddressValid()
        {
            Regex regEx = new Regex(@"[^@]+@[^@]+$");
            return regEx.IsMatch(EmailAddress);
        }

        private void Validate()
        {
            ValidationErrors = 0;
            ValidationMessage = "Valid";

            if (!IsPhoneNumberValid())
            {
                ValidationErrors = 1;
                ValidationMessage = "Phone is invalid.";
            }

            if (!IsEmailAddressValid())
            {
                if (ValidationErrors == 1)
                {
                    ValidationErrors = 2;
                    ValidationMessage = "Email and phone are invalid.";
                }
                else
                {
                    ValidationErrors = 1;
                    ValidationMessage = "Email is invalid.";
                }
            }
        }

        [JsonConstructor]
        public Contact(string fullname, string cityName, string phoneNumber, string emailAddress)
        {
            Name = fullname;
            City = cityName;
            PhoneNumber = phoneNumber;
            EmailAddress = emailAddress;

            Validate();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //step 1 order contracts alphabetically in ascending order
            Console.WriteLine("Contacts");
            var contacts = JsonConvert.DeserializeObject<List<Contact>>(File.ReadAllText(@"Web Developer Test.json")).OrderBy(c => c.Name).ToList();
            foreach (Contact contact in contacts)
            {
                Console.WriteLine(contact.Name);
                Console.WriteLine(contact.ValidationMessage);
            }

            //step 2 order cities by validation errors in descending order
            Console.WriteLine("\nCities");
            var cities = contacts.GroupBy(c => c.City)
                    .Select(g => new { key = g.Key, value = g.Sum(s => s.ValidationErrors) })
                    .OrderByDescending(c => c.value);

            foreach (var city in cities)
            {
                Console.WriteLine(city.key);
                Console.WriteLine(city.value);
            }
            Console.Read();
        }
    }
}
