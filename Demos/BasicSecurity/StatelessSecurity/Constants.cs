namespace StatelessSecurity
{
    public static class Constants
    {
        // The issuer and the audience are the same server in this case.

        public const string Issuer = Audience;
        public const string Audience = "https://localhost:5001";
        public const string Secret = "S0ME_V3RY_S3CR3T_4ND_L0NG_K3Y_!@#$";
    }
}
