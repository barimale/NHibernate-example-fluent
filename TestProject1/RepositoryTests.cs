﻿using Demo.Domain.AggregatesModel.Company2Aggregate;
using Demo.Domain.AggregatesModel.CompanyAggregate;
using Demo.Domain.AggregatesModel.ProductAggregate;
using Demo.Infrastructure;
using Demo.Infrastructure.Database;
using Demo.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;

namespace Demo.UnitTests
{
    [TestClass]
    public sealed class RepositoryTests
    {
        private readonly INHibernateHelper _nHibernateHelper;
        private readonly IAddress2Repository _address2Repository;
        private readonly IAddressRepository _addressRepository;
        private readonly ICompany2Repository _company2Repository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IProductRepository _productRepository;
        public RepositoryTests()
        {
            var builder = new ConfigurationBuilder().AddJsonFile($"appsettings.Test.json", optional: false);
            var _config = builder.Build();
            _nHibernateHelper = new NHibernateHelper(_config);
            _address2Repository = new Address2Repository(_nHibernateHelper);
            _addressRepository = new AddressRepository(_nHibernateHelper);
            _company2Repository = new Company2Repository(_nHibernateHelper);
            _companyRepository = new CompanyRepository(_nHibernateHelper);
            _productRepository = new ProductRepository(_nHibernateHelper);
        }

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            // This method is called once for the test class, before any tests of the class are run.
        }

        [TestMethod]
        public async Task AddProduct()
        {
            // Arrange:
            var product = new Product
            {
                Name = Guid.NewGuid().ToString(),
                Category = "Sample Category",
                Discontinued = false,
                Type = new ProductType { Description = " booo" },
            };

            // Act:

            object result = await _productRepository.Add(product);
            Product? retrievedProduct = await _productRepository.GetByName(product.Name);
              
            // Assert: 
            Assert.IsNotNull(result);
            Assert.IsNotNull(retrievedProduct);
        }

        [TestMethod]
        public async Task AddAddress()
        {
            // Arrange:
            var address = new Address
            {
                City = "Sample City",
                Country = "Sample Country",
                Street = "dsa",
                State = "state",
                ZipCode = "zipcode",
                Phone = "phone",
            };

            // Act:
            var result = await _addressRepository.Add(address);
            Address retrievedAddress = await _addressRepository.GetById((int)result);

            // Assert: 
            Assert.IsNotNull(result);
            Assert.IsNotNull(retrievedAddress);
        }

        [TestMethod]
        public async Task GetAddressByCountry()
        {
            // Arrange:
            var address = new Address
            {
                City = "Sample Product",
                Country = Guid.NewGuid().ToString(),
                Street = "dsa",
                State = "state",
                ZipCode = "zipcode",
                Phone = "phone"
            };

            // Act:
            var result = await _addressRepository.Add(address);
            Address retrievedAddress = await _addressRepository.GetByCountry(address.Country);

            // Assert: 
            Assert.IsNotNull(result);
            Assert.IsNotNull(retrievedAddress);
        }

        [TestMethod]
        public async Task AddCompany()
        {
            // Arrange:
            var company = new Company
            {
                Foo = "Sample Company"
            };

            // Act:
            var result = await _companyRepository.Add(company);
            Company retrievedCompany = await _companyRepository.GetById((int)result);

            // Assert: 
            Assert.IsNotNull(result);
            Assert.IsNotNull(retrievedCompany);

        }

        [TestMethod]
        public async Task AddOneToManyTwice()
        {
            // Arrange:
                var address = new Address
            {
                City = "Sample Product",
                Country = "Sample Category",
                Street = "dsa",
                State = "state",
                ZipCode = "zipcode",
                Phone = "phone"
            };
            var company = new Company { Foo = "city" };

            // Act:
            var resultAddress = await _addressRepository.Add(address);
            var resultCompany = await _companyRepository.Add(company);

            var relationId = _addressRepository.AssingAddressToCompany(resultAddress, resultCompany, "description");
            Company retrievedProduct = await _companyRepository.GetById((int)resultCompany);

            // Assert: 
            Assert.IsNotNull(relationId);
            Assert.IsNotNull(retrievedProduct);
            Assert.IsNotNull(retrievedProduct.Addresses);
        }

        [TestMethod]
        public async Task AddManyToMany()
        {
            // Arrange:
            var address = new Address2
            {
                City = "Sample Product",
                Country = "Sample Category",
                Street = "dsa",
                State = "state",
                ZipCode = "zipcode",
                Phone = "phone"
            };

            var address2 = new Address2
            {
                City = "Sample Product22",
                Country = "Sample Category2",
                Street = "dsa22",
                State = "state2",
                ZipCode = "zipcode2",
                Phone = "phone2"
            };

            var company = new Company2 { Foo = "city2" };

            // Act:
            var resultAddress = await _address2Repository.Add(address);
            var resultAddress2 = await _address2Repository.Add(address2);
            var resultCompany = await _company2Repository.Add(company);

            await _address2Repository.AssignAddressToCompany(resultAddress, resultCompany);
            var addedCompany = await _address2Repository.AssignAddressToCompany(resultAddress2, resultCompany);

            Address2 addressAdded = await _address2Repository.GetById((int)resultAddress);
            Address2 address2Added = await _address2Repository.GetById((int)resultAddress2);

            // Assert:
            Assert.IsNotNull(addressAdded);
            Assert.IsNotNull(addedCompany);
            Assert.IsNotNull(addedCompany.Addresses);
            Assert.IsNotNull(addressAdded.Companies);
            Assert.AreEqual(1, addressAdded.Companies.Count);
            Assert.AreEqual(1, address2Added.Companies.Count);
            Assert.AreEqual(address.City, addedCompany.Addresses.First().City);
            Assert.AreEqual(company.Foo, addressAdded.Companies.First().Foo);
            Assert.AreEqual(2, addedCompany.Addresses.Count);
        }
    }
}
