using CodeKing.SqlHarvester.Core;
using CodeKing.SqlHarvester.Core.Data;
using CodeKing.SqlHarvester.Data;

using NUnit.Framework;

using Rhino.Mocks;

namespace CodeKing.SqlHarvester.Tests.Core
{
    [TestFixture]
    public class HarvestTestFixture
    {
        #region Constants and Fields

        private IDataCommand database;

        private SqlScripterFactory factory;

        private MockRepository mocker;

        private ISqlScripter scripter;

        #endregion

        #region Public Methods

        [Test]
        public void GetFileTest()
        {
        }

        [Test]
        public void GetInvalidFileTest()
        {
        }

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
        public void WriteContentTest()
        {
        }

        [Test]
        public void WriteFooterTest()
        {
        }

        [Test]
        public void WriteHeaderTest()
        {
        }

        #endregion
    }
}
