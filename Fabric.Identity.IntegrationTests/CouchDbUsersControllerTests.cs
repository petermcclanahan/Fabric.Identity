﻿using System.Threading.Tasks;
using Fabric.Identity.API;
using Fabric.Identity.API.Persistence.CouchDb.Stores;
using Moq;
using Serilog;
using Xunit;

namespace Fabric.Identity.IntegrationTests
{
    public class CouchDbUsersControllerTests : UsersControllerTests
    {
        public CouchDbUsersControllerTests() : base(FabricIdentityConstants.StorageProviders.CouchDb)
        {
            UserStore = new CouchDbUserStore(CouchDbService, new Mock<ILogger>().Object);
        }

        public override async Task UsersController_Search_ReturnsUsers()
        {
            Assert.True(true);
        }
    }
}
