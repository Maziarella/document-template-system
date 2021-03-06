﻿using DAL.Data;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class TemplateStateRepository : RepositoryAsync<TemplateState>, ITemplateStateRepository
    {
        public TemplateStateRepository(DTSLocalDBContext DtsContext)
            :base(DtsContext)
        {
        }

        public async Task<IEnumerable<TemplateState>> FindAllTemplateStatesAsync()
        {
            var states = await FindAllAsync();
            return states.DefaultIfEmpty() ?? throw new KeyNotFoundException();
        }

        public async Task<TemplateState> FindTemplateStateByIdAsync(int id)
        {
            var states = await FindByConditionAsync(s => s.Id == id);
            return states.FirstOrDefault() ?? throw new KeyNotFoundException();
        }

        public async Task<TemplateState> FindTemplateStateByName(string name)
        {
            var states = await FindByConditionAsync(s => s.State.Equals(name));
            return states.FirstOrDefault() ?? throw new KeyNotFoundException();
        }
    }
}
