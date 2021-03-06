﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Fabric.Identity.API.Persistence.SqlServer.EntityModels;
using Fabric.Identity.API.Persistence.SqlServer.Services;
using Microsoft.EntityFrameworkCore;
using Fabric.Identity.API.Persistence.SqlServer.Mappers;
using ApiResource = IdentityServer4.Models.ApiResource;

namespace Fabric.Identity.API.Persistence.SqlServer.Stores
{
    public class SqlServerApiResourceStore : SqlServerResourceStore, IApiResourceStore
    {
        public SqlServerApiResourceStore(IIdentityDbContext identityDbContext)
            : base(identityDbContext)
        {        
        }

        public void AddResource(ApiResource resource)
        {
            AddResourceAsync(resource).Wait();
        }

        public void UpdateResource(string id, ApiResource resource)
        {
            UpdateResourceAsync(id, resource).Wait();
        }

        public ApiResource GetResource(string id)
        {
            return GetResourceAsync(id).Result;
        }

        public void DeleteResource(string id)
        {
            DeleteResourceAsync(id).Wait();
        }

        public async Task AddResourceAsync(ApiResource resource)
        {
            var resourceEntity = resource.ToEntity();
            
            IdentityDbContext.ApiResources.Add(resourceEntity);
            await IdentityDbContext.SaveChangesAsync();
        }

        public async Task UpdateResourceAsync(string id, ApiResource resource)
        {
            var savedResource = await IdentityDbContext.ApiResources
                .Where(r => r.Name.Equals(id, StringComparison.OrdinalIgnoreCase)
                            && !r.IsDeleted)
                           .SingleOrDefaultAsync();

           resource.ToEntity(savedResource);

            IdentityDbContext.ApiResources.Update(savedResource);
            await IdentityDbContext.SaveChangesAsync();
        }

        public async Task<ApiResource> GetResourceAsync(string id)
        {
            var apiResource = await IdentityDbContext.ApiResources
                .Where(r => r.Name.Equals(id, StringComparison.OrdinalIgnoreCase)
                            && !r.IsDeleted)
                .Include(x => x.ApiSecrets)
                .Include(x => x.ApiScopes)
                .ThenInclude(s => s.ApiScopeClaims)
                .Include(x => x.ApiClaims)
                .FirstOrDefaultAsync();

            return apiResource?.ToModel();
        }

        public async Task DeleteResourceAsync(string id)
        {
            var apiResourceToDelete =
                await IdentityDbContext.ApiResources.FirstOrDefaultAsync(a =>
                    a.Name.Equals(id, StringComparison.OrdinalIgnoreCase));

            apiResourceToDelete.IsDeleted = true;

            await IdentityDbContext.SaveChangesAsync();
        }
    }
}
