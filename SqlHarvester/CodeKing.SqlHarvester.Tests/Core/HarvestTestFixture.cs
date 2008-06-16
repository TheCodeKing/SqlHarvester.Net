using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using CodeKing.SqlHarvester;
using Rhino.Mocks;
using System.IO;
using CodeKing.SqlHarvester.Data;

namespace CodeKing.SqlHarvester.Tests.Core
{
    [TestFixture]
    public class HarvestTestFixture
    {
        private MockRepository mocker;
        private ISqlScripter scripter;
        private SqlScripterFactory factory;
        private IDataCommand database;

        [SetUp]
        public void SetUp()
        {
            mocker = new MockRepository();
            scripter = mocker.CreateMock<ISqlScripter>();
            database = mocker.CreateMock<IDataCommand>();
            factory = new SqlScripterFactory(database);
            //harvestor = new Harvestor(factory, );
        }


        [TearDown]
        public void TearDown()
        {
            //
            // TODO: Add code to be performed after each test method is run
            //
        }

        [Test]
        public void WriteHeaderTest()
        {

        }


        [Test]
        public void WriteContentTest()
        {

        }

        [Test]
        public void WriteFooterTest()
        {

        }

        [Test]
        public void GetFileTest()
        {
        }

        [Test]
        public void GetInvalidFileTest()
        {

        }
    }
}
