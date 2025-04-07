namespace LeaveService.Model
{
    public class JsonModel
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public object UserPermission { get; set; }
        public object AppConfigurations { get; set; }
        public object UserLocations { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public object data { get; set; }
        public object? data1 { get; set; }
        public object SpareObject { get; set; }

        public string AppError { get; set; }

        public string UserRole { get; set; }
        public string RoleName { get; set; }
        public string UserName { get; set; }
        public string PhotoBase64 { get; set; }
        public string EncryptedId { get; set; }
        public int Id { get; set; }
        public string UserId { get; set; }
        public bool CheckFlag { get; set; }
        public string SpareData { get; set; }
        public string Base64printdata { get; set; }
        public bool IsSwitched { get; set; }
        public bool isPaymentForMonthDone { get; set; }
        public string TermsAndConditionsSign { get; set; }
        public string IsConsent { get; set; }
        public bool isNewIp { get; set; }
    }
}
