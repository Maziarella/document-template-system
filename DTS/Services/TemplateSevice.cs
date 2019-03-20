﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Repositories;
using DTS.API.Models.DTOs;

namespace DTS.API.Services
{
    public class TemplateService : ITemplateService
    {
        private ICommandHandlerAsync<ActivateTemplateCommand> _activateTemplateCommand;
        private ICommandHandlerAsync<ActivateTemplateVersionCommand> _activateTemplateVersionCommand;
        private ICommandHandlerAsync<AddTemplateCommand> _addTemplateCommand;
        private ICommandHandlerAsync<AddTemplateVersionCommand> _addTemplateVersionCommand;
        private ICommandHandlerAsync<DeactivateTemplateCommand> _deactivateTemplateCommand;
        private ICommandHandlerAsync<DeactivateTemplateVersionCommand> _deactivateTemplateVersionCommand;
        private ICommandHandlerAsync<SetTemplateOwnerCommand> _setTemplateOwnerCommand;
        private ICommandHandlerAsync<UpdateTemplateDataCommand> _updateTemplateDataCommand;
        private IQueryHandlerAsync<GetTemplateByIdQuery, TemplateDTO> _getTemplateByIdQuery;
        private IQueryHandlerAsync<GetTemplatesByUserQuery, List<TemplateDTO>> _getTemplatesByUserQuery;
        private IQueryHandlerAsync<GetTemplatesQuery, List<TemplateDTO>> _getTemplatesQuery;
        private IQueryHandlerAsync<FillInTemplateQuery, TemplateContentDTO> _fillInTemplateQuery;
        private IQueryHandlerAsync<GetTemplateFormQuery, Dictionary<string, string>> _getTemplateFormQuery;
        private IQueryHandlerAsync<GetTemplateStatesQuery, IList<string>> _getTemplateStatesQuery;
        private readonly IRepositoryWrapper repository;


        public TemplateService(IRepositoryWrapper repository)
        {
            this.repository = repository;
        }


        public ICommandHandlerAsync<ActivateTemplateCommand> ActivateTemplateCommand
        {
            get
            {
                _activateTemplateCommand = _activateTemplateCommand ?? new ActivateTemplateCommandHandler(repository);

                return _activateTemplateCommand;
            }
        }


        public ICommandHandlerAsync<ActivateTemplateVersionCommand> ActivateTemplateVersionCommand
        {
            get
            {

                _activateTemplateVersionCommand = _activateTemplateVersionCommand ?? new ActivateTemplateVersionCommandHandler(repository);

                return _activateTemplateVersionCommand;
            }
        }


        public ICommandHandlerAsync<AddTemplateCommand> AddTemplateCommand
        {
            get
            {
                    _addTemplateCommand = _addTemplateCommand ?? new AddTemplateCommandHandler(repository);
   
                return _addTemplateCommand;
            }
        }


        public ICommandHandlerAsync<AddTemplateVersionCommand> AddTemplateVersionCommand
        {
            get
            {
                _addTemplateVersionCommand = _addTemplateVersionCommand ?? new AddTemplateVersionCommandHandler(repository);

                return _addTemplateVersionCommand;
            }
        }


        public ICommandHandlerAsync<DeactivateTemplateCommand> DeactivateTemplateCommand
        {
            get
            {
                _deactivateTemplateCommand = _deactivateTemplateCommand ?? new DeactivateTemplateCommandHandler(repository);

                return _deactivateTemplateCommand;
            }
        }


        public ICommandHandlerAsync<DeactivateTemplateVersionCommand> DeactivateTemplateVersionCommand
        {
            get
            {
               _deactivateTemplateVersionCommand = _deactivateTemplateVersionCommand ?? new DeactivateTemplateVersionCommandHandler(repository);

                return _deactivateTemplateVersionCommand;
            }
        }


        public ICommandHandlerAsync<SetTemplateOwnerCommand> SetTemplateOwnerCommand
        {
            get
            {
                _setTemplateOwnerCommand = _setTemplateOwnerCommand ?? new SetTemplateOwnerCommandHandler(repository);

                return _setTemplateOwnerCommand;
            }
        }


        public ICommandHandlerAsync<UpdateTemplateDataCommand> UpdateTemplateDataCommand
        {
            get
            {
                _updateTemplateDataCommand = _updateTemplateDataCommand ?? new UpdateTemplateDataCommandHandler(repository);

                return _updateTemplateDataCommand;
            }
        }


        public IQueryHandlerAsync<GetTemplateByIdQuery, TemplateDTO> GetTemplateByIdQuery
        {
            get
            {
                _getTemplateByIdQuery = _getTemplateByIdQuery ?? new GetTemplateByIdQueryHandler(repository);

                return _getTemplateByIdQuery;
            }
        }


        public IQueryHandlerAsync<GetTemplatesByUserQuery, List<TemplateDTO>> GetTemplatesByUserQuery
        {
            get
            {
                _getTemplatesByUserQuery = _getTemplatesByUserQuery ?? new GetTemplatesByUserQueryHandler(repository);

                return _getTemplatesByUserQuery;
            }
        }


        public IQueryHandlerAsync<GetTemplatesQuery, List<TemplateDTO>> GetTemplatesQuery
        {
            get
            {
                _getTemplatesQuery = _getTemplatesQuery ?? new GetTemplatesQueryHandler(repository);

                return _getTemplatesQuery;
            }
        }


        public IQueryHandlerAsync<FillInTemplateQuery, TemplateContentDTO> FillInTemplateQuery
        {
            get
            {
                _fillInTemplateQuery = _fillInTemplateQuery ?? new FillInTemplateQueryHandler(repository);

                return _fillInTemplateQuery;
            }
        }


        public IQueryHandlerAsync<GetTemplateFormQuery, Dictionary<string, string>> GetTemplateFormQuery
        {
            get
            {
                _getTemplateFormQuery = _getTemplateFormQuery ?? new GetTemplateFormQueryHandler(repository);

                return _getTemplateFormQuery;
            }
        }

        public IQueryHandlerAsync<GetTemplateStatesQuery, IList<string>> GetTemplateStatesQuery
        {
            get
            {
                if (_getTemplateStatesQuery == null)
                {
                    _getTemplateStatesQuery = new GetTemplateStatesQueryHandler(repository);
                }
                return _getTemplateStatesQuery;
            }
        }
    }
}
