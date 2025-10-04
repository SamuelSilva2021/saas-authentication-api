using Authenticator.API.Core.Application.Interfaces.AccessControl.Operation;
using Authenticator.API.Core.Domain.AccessControl.Operations;
using Authenticator.API.Core.Domain.AccessControl.Operations.DTOs;
using Authenticator.API.Core.Domain.Api;
using AutoMapper;

namespace Authenticator.API.Core.Application.Implementation.AccessControl.Operation
{
    public class OperationService(IOperationRepository operationRepository, IMapper mapper) : IOperationService
    {
        private readonly IOperationRepository _operationRepository = operationRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<ResponseDTO<OperationDTO>> AddOperationAsync(OperationCreateDTO operation)
        {
            try
            {
                var entity = _mapper.Map<OperationEntity>(operation);
                var createdEntity = await _operationRepository.AddAsync(entity);
                var dto = _mapper.Map<OperationDTO>(createdEntity);
                return ResponseBuilder<OperationDTO>.Ok(dto).WithCode(201).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<OperationDTO>
                    .Fail(new ErrorDTO { Message = ex.Message }).WithException(ex).WithCode(500).Build();
            }
            
        }

        public async Task<ResponseDTO<bool>> DeleteOperationAsync(Guid id)
        {
            try
            {
                var existingEntity = await _operationRepository.GetByIdAsync(id);
                if (existingEntity == null)
                    return ResponseBuilder<bool>
                        .Fail(new ErrorDTO { Message = "Operação não encontrada" })
                        .WithCode(404)
                        .Build();
                await _operationRepository.DeleteAsync(existingEntity);
                return ResponseBuilder<bool>.Ok(true).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<bool>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        public async Task<ResponseDTO<IEnumerable<OperationDTO>>> GetAllOperationAsync()
        {
            try
            {
                var entities = await _operationRepository.GetAllAsync();
                var dtos = _mapper.Map<IEnumerable<OperationDTO>>(entities);
                return ResponseBuilder<IEnumerable<OperationDTO>>.Ok(dtos).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<IEnumerable<OperationDTO>>
                    .Fail(new ErrorDTO { Message = ex.Message }).WithException(ex).WithCode(500).Build();
            }
        }

        public async Task<ResponseDTO<PagedResponseDTO<OperationDTO>>> GetAllOperationPagedAsync(int page, int limit)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseDTO<OperationDTO>> GetOperationByIdAsync(Guid id)
        {
            try
            {
                var operationEntity = await _operationRepository.GetByIdAsync(id);
                if (operationEntity == null)
                    return ResponseBuilder<OperationDTO>
                        .Fail(new ErrorDTO { Message = "Operação não encontrada" })
                        .WithCode(404)
                        .Build();
                var operationDTO = _mapper.Map<OperationDTO>(operationEntity);
                return ResponseBuilder<OperationDTO>.Ok(operationDTO).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<OperationDTO>
                    .Fail(new ErrorDTO { Message = ex.Message }).WithException(ex).WithCode(500).Build();
            }
        }

        public async Task<ResponseDTO<OperationDTO>> UpdateOperationAsync(Guid id, OperationUpdateDTO operation)
        {
            try
            {
                var existingEntity = await _operationRepository.GetByIdAsync(id);
                if (existingEntity == null)
                    return ResponseBuilder<OperationDTO>
                        .Fail(new ErrorDTO { Message = "Operação não encontrada" })
                        .WithCode(404)
                        .Build();

                existingEntity = _mapper.Map(operation, existingEntity);

                await _operationRepository.UpdateAsync(existingEntity);

                var dto = _mapper.Map<OperationDTO>(existingEntity);
                return ResponseBuilder<OperationDTO>.Ok(dto).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<OperationDTO>
                    .Fail(new ErrorDTO { Message = ex.Message }).WithException(ex).WithCode(500).Build();
            }
        }
    }
}
