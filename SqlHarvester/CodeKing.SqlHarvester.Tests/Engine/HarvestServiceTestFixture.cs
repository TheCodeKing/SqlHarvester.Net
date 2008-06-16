using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using CodeKing.SqlHarvester;
using Rhino.Mocks;
using CodeKing.SqlHarvester.Data;
using System.IO;

namespace CodeKing.SqlHarvester.Tests.Engine
{
    [TestFixture]
    public class HarvestServiceTestFixture
    {
        private MockRepository mocker;
        private HarvestService service;
        private IHarvester harvestor;
        private ISqlScripter scripter;

        [SetUp]
        public void SetUp()
        {
            mocker = new MockRepository();
            service = mocker.PartialMock<HarvestService>();
            harvestor = mocker.CreateMock<IHarvester>();
            scripter = mocker.CreateMock<ISqlScripter>();
        }

        [TearDown]
        public void TearDown()
        {
            //
            // TODO: Add code to be performed after each test method is run
            //
        }


        [Test]
        public void WithContentScript()
        {
     
        }

        [Test]
        public void WithoutContentScript()
        {
     
        }

    }
}
