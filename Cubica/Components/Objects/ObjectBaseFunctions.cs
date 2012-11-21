using System.Linq;
using Cubica.Managers;
using System.Globalization;

namespace Cubica.Components.Objects
{
    partial class ObjectBase
    {
        [RegisterFunction]
        public string GetCustParam(string key)
        {
            var result = Parameters.FirstOrDefault(o => o.Name.ToLower(CultureInfo.InvariantCulture).Trim().Equals(key.ToLower(CultureInfo.InvariantCulture).Trim()));
            return result == null ? string.Empty : result.Value;
        }
    }
}
