using CodeKing.SqlHarvester.Core;

using NUnit.Framework;

using Rhino.Mocks;

namespace CodeKing.SqlHarvester.Tests.Engine
{
    [TestFixture]
    public class HarvestServiceTestFixture
    {
        #region Constants and Fields

        private IHarvester harvestor;

        private MockRepository mocker;

        private ISqlScripter scripter;

        private HarvestService service;

        #endregion

        #region Public Methods

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

        #endregion
    }
}
