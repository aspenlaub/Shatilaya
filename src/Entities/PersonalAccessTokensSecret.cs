using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Entities {
    public class PersonalAccessTokensSecret : ISecret<PersonalAccessTokens> {
        private PersonalAccessTokens vPersonalAccessTokens;
        public PersonalAccessTokens DefaultValue {
            get {
                return vPersonalAccessTokens ?? (vPersonalAccessTokens = new PersonalAccessTokens());
            }
        }

        public string Guid { get { return "A7EDA2D5-4207-46D1-A1F5-B7F831C0269B"; } }
    }
}
