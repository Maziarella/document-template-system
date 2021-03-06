﻿using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DTS.API.Services
{
    public sealed class GetUserStatusesQuery :IQuery
    {
    }

    public sealed class GetUserStatusesQueryHandler :
    IQueryHandlerAsync<GetUserStatusesQuery, IList<string>>
    {
        private IRepositoryWrapper repository;

        public GetUserStatusesQueryHandler(IRepositoryWrapper repository)
        {
            this.repository = repository;
        }

        public async Task<IList<string>> HandleAsync(GetUserStatusesQuery query)
        {
            var statuses = await repository.UserStatus.FindAllUserStatuses();
            IList<string> statusesNames = new List<string>();
            foreach (var type in statuses)
            {
                statusesNames.Add(type.Name);
            }
            return statusesNames;
        }
    }
}
