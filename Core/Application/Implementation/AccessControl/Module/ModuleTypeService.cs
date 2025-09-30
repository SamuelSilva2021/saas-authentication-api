using Authenticator.API.Core.Application.Interfaces.AccessControl.Module;
using Authenticator.API.Core.Domain.AccessControl.Modules;
using Authenticator.API.Core.Domain.AccessControl.Modules.DTOs;
using Authenticator.API.Core.Domain.Api;
using AutoMapper;

namespace Authenticator.API.Core.Application.Implementation.AccessControl.Module
{
    public class ModuleTypeService(
        IModuleTypeRepository repository,
        IMapper mapper
        ) : IModuleTypeService
    {
        private readonly IModuleTypeRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<ResponseDTO<ModuleTypeDTO>> AddModuleTypeAsync(ModuleTypeCreateDTO moduleType)
        {
            try
            {
                var moduleTypeEntity = _mapper.Map<ModuleTypeEntity>(moduleType);

                var createdModuleType = await _repository.AddAsync(moduleTypeEntity);

                var moduleTypeDTO = _mapper.Map<ModuleTypeDTO>(createdModuleType);

                return ResponseBuilder<ModuleTypeDTO>.Ok(moduleTypeDTO).WithCode(201).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<ModuleTypeDTO>
                    .Fail(new ErrorDTO { Message = ex.Message }).WithException(ex).WithCode(500).Build();
            }
        }

        public Task<ResponseDTO<bool>> DeleteModuleTypeAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseDTO<IEnumerable<ModuleTypeDTO>>> GetAllModuleTypesAsync()
        {
            try
            {
                var moduleTypes = await _repository.GetAllAsync();

                if(!moduleTypes.Any())
                    return ResponseBuilder<IEnumerable<ModuleTypeDTO>>.Ok([]).Build();

                var moduleTypesDTO = _mapper.Map<IEnumerable<ModuleTypeDTO>>(moduleTypes);

                return ResponseBuilder<IEnumerable<ModuleTypeDTO>>.Ok(moduleTypesDTO).Build();

            }
            catch (Exception ex)
            {
                return ResponseBuilder<IEnumerable<ModuleTypeDTO>>
                    .Fail(new ErrorDTO{Message = ex.Message})
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        public async Task<ResponseDTO<ModuleTypeDTO>> GetModuleTypeByIdAsync(Guid id)
        {
            try
            {
                var moduleType = await _repository.GetByIdAsync(id);
                if (moduleType == null)
                    return ResponseBuilder<ModuleTypeDTO>
                        .Fail(new ErrorDTO { Message = "Module type not found." })
                        .WithCode(404)
                        .Build();

                var moduleTypeDTO = _mapper.Map<ModuleTypeDTO>(moduleType);
                return ResponseBuilder<ModuleTypeDTO>.Ok(moduleTypeDTO).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<ModuleTypeDTO>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        public Task<ResponseDTO<ModuleTypeDTO>> UpdateModuleTypeAsync(Guid id, ModuleTypeUpdateDTO moduleType)
        {
            throw new NotImplementedException();
        }
    }
}
