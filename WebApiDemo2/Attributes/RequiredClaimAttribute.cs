namespace WebApiDemo2.Attributes
{

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class RequiredClaimAttribute : Attribute
    {
        public string ClaimType {get;} //to claim einai ena key value pair

        public string ClaimValue {get;}

        public RequiredClaimAttribute(string ClaimType, string ClaimValue)
        {
            this.ClaimType = ClaimType;
            this.ClaimValue = ClaimValue;
            
        }

    }
}
