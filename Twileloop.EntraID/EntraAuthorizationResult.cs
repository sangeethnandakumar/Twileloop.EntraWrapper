namespace Twileloop.EntraID
{
    public record EntraAuthorizationResult
    {
        public bool IsAuthorised { get; set; } = false;
        public string? OverrideAuthorizationFailureResponse { get; set; }

        public EntraAuthorizationResult(bool isAuthorised)
        {
            IsAuthorised = isAuthorised;
        }

        public EntraAuthorizationResult(bool isAuthorised, string failureResponseContent)
        {
            IsAuthorised = isAuthorised;
            OverrideAuthorizationFailureResponse = failureResponseContent;
        }
    }
}
