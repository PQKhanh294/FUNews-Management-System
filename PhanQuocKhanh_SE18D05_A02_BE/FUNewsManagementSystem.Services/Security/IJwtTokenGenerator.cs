using FUNewsManagementSystem.Services.Dtos;
using FUNewsManagementSystem.BusinessObjects.Entities;

namespace FUNewsManagementSystem.Services.Security;

public interface IJwtTokenGenerator
{
    AuthResponseDto Generate(SystemAccount account);
}

