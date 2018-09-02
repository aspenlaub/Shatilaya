using System.Collections.Generic;
using System.Xml.Serialization;
using Aspenlaub.Net.GitHub.CSharp.PeghStandard.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Entities {
    [XmlRoot("PersonalAccessTokens", Namespace = "http://www.aspenlaub.net")]
    public class PersonalAccessTokens : List<PersonalAccessToken>, ISecretResult<PersonalAccessTokens> {
        public PersonalAccessTokens Clone() {
            var clone = new PersonalAccessTokens();
            clone.AddRange(this);
            return clone;
        }
    }
}
