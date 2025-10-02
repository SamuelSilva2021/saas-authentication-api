using Authenticator.API.Core.Application.Interfaces.AccessControl.Module;
using Authenticator.API.Core.Domain.AccessControl.Modules;
using Authenticator.API.Core.Domain.AccessControl.Modules.DTOs;
using Authenticator.API.Core.Domain.Api;
using AutoMapper;

namespace Authenticator.API.Core.Application.Implementation.AccessControl.Module
{
    public class ModuleTypeService(
        IModuleRepository moduleRepository,
        IMapper mapper
        ) : IModuleService
    {
        private readonly IModuleRepository _moduleRepository = moduleRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<ResponseDTO<ModuleDTO>> AddModuleAsync(ModuleCreateDTO module)
        {
            try
            {
                var moduleEntity = _mapper.Map<ModuleEntity>(module);

                var createdModuleType = await _moduleRepository.AddAsync(moduleEntity);

                var moduleTypeDTO = _mapper.Map<ModuleDTO>(createdModuleType);

                return ResponseBuilder<ModuleDTO>.Ok(moduleTypeDTO).WithCode(201).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<ModuleDTO>
                    .Fail(new ErrorDTO { Message = ex.Message }).WithException(ex).WithCode(500).Build();
            }
        }

        public Task<ResponseDTO<bool>> DeleteModuleAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseDTO<IEnumerable<ModuleDTO>>> GetAllModuleAsync()
        {
            try
            {
                var moduleTypes = await _moduleRepository.GetAllAsync();

                if(!moduleTypes.Any())
                    return ResponseBuilder<IEnumerable<ModuleDTO>>.Ok([]).Build();

                var moduleTypesDTO = _mapper.Map<IEnumerable<ModuleDTO>>(moduleTypes);

                return ResponseBuilder<IEnumerable<ModuleDTO>>.Ok(moduleTypesDTO).Build();

            }
            catch (Exception ex)
            {
                return ResponseBuilder<IEnumerable<ModuleDTO>>
                    .Fail(new ErrorDTO{Message = ex.Message})
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        public async Task<ResponseDTO<ModuleDTO>> GetModuleByIdAsync(Guid id)
        {
            try
            {
                var moduleType = await _moduleRepository.GetByIdAsync(id);
                if (moduleType == null)
                    return ResponseBuilder<ModuleDTO>
                        .Fail(new ErrorDTO { Message = "Module type not found." })
                        .WithCode(404)
                        .Build();

                var moduleTypeDTO = _mapper.Map<ModuleDTO>(moduleType);
                return ResponseBuilder<ModuleDTO>.Ok(moduleTypeDTO).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<ModuleDTO>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        public Task<ResponseDTO<ModuleDTO>> UpdateModuleAsync(Guid id, ModuleUpdateDTO moduleType)
        {
            throw new NotImplementedException();
        }
    }
}
