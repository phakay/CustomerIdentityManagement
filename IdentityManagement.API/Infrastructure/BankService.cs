using static IdentityManagement.API.Dtos.ExternalServiceDto;

namespace IdentityManagement.API.Infrastructure
{
    public class BankService : BaseApiClient
    {
        public BankService(HttpClient httpClient) : base(httpClient)
        {  }

        public async Task<GetAllBanksResponse> GetAllBanksAsync()
        {
            return await GetAsync<GetAllBanksResponse>("https://wema-alatdev-apimgt.azure-api.net/alat-test/api/Shared/GetAllBanks");
        }
    }
}
