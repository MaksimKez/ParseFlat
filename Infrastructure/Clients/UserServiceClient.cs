using Application.Abstractions.UserService;
using Application.Dtos;
using Application.Dtos.Users;
using Application.Responses;
using Infrastructure.Clients.Interfaces;
using Polly;
using Refit;

namespace Infrastructure.Clients;

public class UserServiceClient(
    HttpClient httpClient,
    ResiliencePipeline resiliencePipeline,
    IUserServiceApi userServiceApi)
    : BaseHttpService(httpClient, resiliencePipeline), IUserServiceClient
{
    public async Task<UserServiceResult> FindByIdAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var user = await resiliencePipeline.ExecuteAsync(async token => 
                await userServiceApi.GetByIdAsync(id, token), ct);

            return new UserServiceResult
            {
                IsSuccess = true,
                User = user
            };
        }
        catch (ApiException apiEx)
        {
            return new UserServiceResult
            {
                IsSuccess = false,
                ErrorMessage = FormatApiError(apiEx)
            };
        }
        catch (Exception ex)
        {
            return new UserServiceResult
            {
                IsSuccess = false,
                ErrorMessage = $"Unexpected error: {ex.Message}"
            };
        }
    }

    public async Task<UserServiceResult> FindByEmailAsync(string email, CancellationToken ct = default)
    {
        try
        {
            var user = await resiliencePipeline.ExecuteAsync(async token => 
                await userServiceApi.GetByEmailAsync(email, token), ct);

            return new UserServiceResult
            {
                IsSuccess = true,
                User = user
            };
        }
        catch (ApiException apiEx)
        {
            return new UserServiceResult
            {
                IsSuccess = false,
                ErrorMessage = FormatApiError(apiEx)
            };
        }
        catch (Exception ex)
        {
            return new UserServiceResult
            {
                IsSuccess = false,
                ErrorMessage = $"Unexpected error: {ex.Message}"
            };
        }
    }

    public async Task<UserServiceResult> AddUserAsync(UserDto dto, CancellationToken ct = default)
    {
        try
        {
            var request = MapToAddUserRequest(dto);
            var user = await resiliencePipeline.ExecuteAsync(async token => 
                await userServiceApi.AddUserAsync(request, token), ct);

            return new UserServiceResult
            {
                IsSuccess = true,
                User = user
            };
        }
        catch (ApiException apiEx)
        {
            return new UserServiceResult
            {
                IsSuccess = false,
                ErrorMessage = FormatApiError(apiEx)
            };
        }
        catch (Exception ex)
        {
            return new UserServiceResult
            {
                IsSuccess = false,
                ErrorMessage = $"Unexpected error: {ex.Message}"
            };
        }
    }

    private static AddUserProfileRequest MapToAddUserRequest(UserDto dto)
    {
        return new AddUserProfileRequest
        {
            Email = dto.Email,
            Id = dto.Id,
            LastName = dto.LastName,
            Name = dto.Name
        };
    }

    private static string FormatApiError(ApiException apiEx)
    {
        var errorBody = apiEx.Content ?? string.Empty;
        return string.IsNullOrWhiteSpace(errorBody)
            ? $"HTTP {(int)apiEx.StatusCode} {apiEx.ReasonPhrase}"
            : $"HTTP {(int)apiEx.StatusCode} {apiEx.ReasonPhrase}: {errorBody}";
    }
}