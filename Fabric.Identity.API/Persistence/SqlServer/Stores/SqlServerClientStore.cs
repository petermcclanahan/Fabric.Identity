﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fabric.Identity.API.Persistence.SqlServer.Services;
using Fabric.Identity.API.Persistence.SqlServer.Mappers;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;

namespace Fabric.Identity.API.Persistence.SqlServer.Stores
{
    public class SqlServerClientStore : IClientManagementStore
    {
        private readonly IIdentityDbContext _identityDbContext;

        public SqlServerClientStore(IIdentityDbContext identityDbContext)
        {
            _identityDbContext = identityDbContext;            
        }

        public Task<Client> FindClientByIdAsync(string clientId)
        {
            var client = _identityDbContext.Clients
                .Include(x => x.ClientGrantTypes)
                .Include(x => x.ClientRedirectUris)
                .Include(x => x.ClientPostLogoutRedirectUris)
                .Include(x => x.ClientScopes)
                .Include(x => x.ClientSecrets)
                .Include(x => x.ClientClaims)
                .Include(x => x.ClientIdpRestrictions)
                .Include(x => x.ClientCorsOrigins)
                .Where(c => !c.IsDeleted)
                .FirstOrDefault(x => x.ClientId == clientId);
            var clientEntity = client?.ToModel();

            return Task.FromResult(clientEntity);
        }

        public IEnumerable<Client> GetAllClients()
        {
            var clients = _identityDbContext.Clients
                .Include(x => x.ClientGrantTypes)
                .Include(x => x.ClientRedirectUris)
                .Include(x => x.ClientPostLogoutRedirectUris)
                .Include(x => x.ClientScopes)
                .Include(x => x.ClientSecrets)
                .Include(x => x.ClientClaims)
                .Include(x => x.ClientIdpRestrictions)
                .Include(x => x.ClientCorsOrigins)
                .Where(c => !c.IsDeleted)
                .Select(c => c.ToModel());

            return clients;
        }

        public int GetClientCount()
        {
            return _identityDbContext.Clients.Count();
        }

        public void AddClient(Client client)
        {
            AddClientAsync(client).Wait();
        }

        public void UpdateClient(string clientId, Client client)
        {
            UpdateClientAsync(clientId, client).Wait();
        }

        public void DeleteClient(string id)
        {
            DeleteClientAsync(id).Wait();
        }

        public async Task AddClientAsync(Client client)
        {
            var domainModelClient = client.ToEntity();

            _identityDbContext.Clients.Add(domainModelClient);
            await _identityDbContext.SaveChangesAsync();
        }

        public async Task UpdateClientAsync(string clientId, Client client)
        {
            var existingClient = await _identityDbContext.Clients.FirstOrDefaultAsync(c =>
                c.ClientId.Equals(client.ClientId, StringComparison.OrdinalIgnoreCase));

            client.ToEntity(existingClient);

            _identityDbContext.Clients.Update(existingClient);
            await _identityDbContext.SaveChangesAsync();
        }

        public async Task DeleteClientAsync(string id)
        {
            var clientToDelete =
               await _identityDbContext.Clients.FirstOrDefaultAsync(c =>
                    c.ClientId.Equals(id, StringComparison.OrdinalIgnoreCase));

            clientToDelete.IsDeleted = true;

            await _identityDbContext.SaveChangesAsync();
        }
    }
}
