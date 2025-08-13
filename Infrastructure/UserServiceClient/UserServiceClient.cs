using System.Net.Http.Json;
using Application.Abstractions.UserService;
using Application.Dtos;
using Application.Dtos.Settings;
using Application.Dtos.Users;
using Application.Responses;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

namespace Infrastructure.UserServiceClient;

public class UserServiceClient : IUserServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ResiliencePipeline _retryPipeline;

    public UserServiceClient(HttpClient httpClient, IOptions<UserProfileClientSettings> settingsOptions)
    {
        var settings = settingsOptions.Value;
        
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(settings.BaseUrl);

        _retryPipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = settings.RetryCount,
                Delay = TimeSpan.FromSeconds(settings.RetryDelaySeconds),
                BackoffType = DelayBackoffType.Constant,
                ShouldHandle = new PredicateBuilder()
                    .Handle<HttpRequestException>()
                    .Handle<TaskCanceledException>(),
                })
            .Build();
    }

    public async Task<UserServiceResult> FindByIdAsync(Guid id, CancellationToken ct) =>
        await _retryPipeline.ExecuteAsync(async token =>
        {
            var response = await _httpClient.GetAsync($"{id}", token);

            if (!response.IsSuccessStatusCode)
            {
                return new UserServiceResult
                {
                    IsSuccess = false,
                    ErrorMessage = response.ReasonPhrase
                };
            }

            var user = await response.Content.ReadFromJsonAsync<UserDto>(cancellationToken: token);
            return new UserServiceResult
            {
                IsSuccess = true,
                User = user
            };
        }, ct);
    
    public async Task<UserServiceResult> FindByEmailAsync(string email, CancellationToken ct) =>
        await _retryPipeline.ExecuteAsync(async token =>
        {
            var response = await _httpClient.GetAsync($"by-email?email={Uri.EscapeDataString(email)}", token);

            if (!response.IsSuccessStatusCode)
            {
                return new UserServiceResult
                {
                    IsSuccess = false,
                    ErrorMessage = response.ReasonPhrase
                };
            }

            var user = await response.Content.ReadFromJsonAsync<UserDto>(cancellationToken: token);
            return new UserServiceResult
            {
                IsSuccess = true,
                User = user
            };
        }, ct);

    public async Task<UserServiceResult> AddUserAsync(UserDto dto, CancellationToken ct) =>
        await _retryPipeline.ExecuteAsync(async token =>
        {
            var request = new AddUserProfileRequest
            {
                Email = dto.Email,
                Id = dto.Id,
                LastName = dto.LastName,
                Name = dto.Name
            };  
            
            var response = await _httpClient.PostAsJsonAsync(string.Empty, request, token);

            if (!response.IsSuccessStatusCode)
            {
                return new UserServiceResult
                {
                    IsSuccess = false,
                    ErrorMessage = response.ReasonPhrase
                };
            }

            var user = await response.Content.ReadFromJsonAsync<UserDto>(cancellationToken: token);
            return new UserServiceResult
            {
                IsSuccess = true,
                User = user
            };
        }, ct);
}