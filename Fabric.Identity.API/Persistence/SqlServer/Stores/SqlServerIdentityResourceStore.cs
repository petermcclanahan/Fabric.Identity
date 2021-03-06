﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Fabric.Identity.API.Persistence.SqlServer.Services;
using Microsoft.EntityFrameworkCore;
using Fabric.Identity.API.Persistence.SqlServer.Mappers;
using IdentityResource = IdentityServer4.Models.IdentityResource;

namespace Fabric.Identity.API.Persistence.SqlServer.Stores
{
    public class SqlServerIdentityResourceStore : SqlServerResourceStore, IIdentityResourceStore
    {
        public SqlServerIdentityResourceStore(IIdentityDbContext identityDbContext) :
            base(identityDbContext)
        {
            
        }

        public void AddResource(IdentityResource resource)
        {
            AddResourceAsync(resource).Wait();
        }

        public void UpdateResource(string id, IdentityResource resource)
        {
            UpdateResourceAsync(id, resource).Wait();
        }

        public IdentityResource GetResource(string id)
        {
            return GetResourceAsync(id).Result;
        }

        public void DeleteResource(string id)
        {
            DeleteResourceAsync(id).Wait();
        }

        public async Task AddResourceAsync(IdentityResource resource)
        {
            var resourceEntity = resource.ToEntity();

            IdentityDbContext.IdentityResources.Add(resourceEntity);
            await IdentityDbContext.SaveChangesAsync();
        }

        public async Task UpdateResourceAsync(string id, IdentityResource resource)
        {
            var existingResource = await IdentityDbContext.IdentityResources
                .Where(r => r.Name.Equals(id, StringComparison.OrdinalIgnoreCase)
                            && !r.IsDeleted)
                .SingleOrDefaultAsync();

           resource.ToEntity(existingResource);
            
            IdentityDbContext.IdentityResources.Update(existingResource);
            await IdentityDbContext.SaveChangesAsync();
        }

        public async Task<IdentityResource> GetResourceAsync(string id)
        {
            var identityResourceEntity = await IdentityDbContext.IdentityResources
                .Where(i => !i.IsDeleted)
                .FirstOrDefaultAsync(i => i.Name.Equals(id, StringComparison.CurrentCultureIgnoreCase));

            return identityResourceEntity?.ToModel();
        }

        public async Task DeleteResourceAsync(string id)
        {
            var identityResourceToDelete =
                await IdentityDbContext.IdentityResources.FirstOrDefaultAsync(a =>
                    a.Name.Equals(id, StringComparison.OrdinalIgnoreCase));

            identityResourceToDelete.IsDeleted = true;

            await IdentityDbContext.SaveChangesAsync();
        }
    }
}
