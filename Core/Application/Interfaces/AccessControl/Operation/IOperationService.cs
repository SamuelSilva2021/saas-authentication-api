using Authenticator.API.Core.Domain.AccessControl.Operations.DTOs;
using Authenticator.API.Core.Domain.Api;

namespace Authenticator.API.Core.Application.Interfaces.AccessControl.Operation
{
    public interface IOperationService
    {
        Task<ResponseDTO<IEnumerable<OperationDTO>>> GetAllOperationAsync();
        Task<ResponseDTO<PagedResponseDTO<OperationDTO>>> GetAllOperationPagedAsync(int page, int limit);
        Task<ResponseDTO<OperationDTO>> GetOperationByIdAsync(Guid id);
        Task<ResponseDTO<OperationDTO>> AddOperationAsync(OperationCreateDTO operation);
        Task<ResponseDTO<OperationDTO>> UpdateOperationAsync(Guid id, OperationUpdateDTO operation);
        Task<ResponseDTO<bool>> DeleteOperationAsync(Guid id);
    }
}
