namespace IdentityManagement.API.Dtos
{
    public class ExternalServiceDto
    {
        public class GetAllBanksResponse
        {
            public BankModel[] result { get; set; }
            public object errorMessage { get; set; }
            public object errorMessages { get; set; }
            public bool hasError { get; set; }
            public DateTime timeGenerated { get; set; }
        }

        public class BankModel
        {
            public string bankName { get; set; }
            public string bankCode { get; set; }
            public string bankLogo { get; set; }
        }
    }
}
