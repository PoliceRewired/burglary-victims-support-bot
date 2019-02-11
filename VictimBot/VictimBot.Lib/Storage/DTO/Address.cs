using System;
using System.Collections.Generic;
using System.Text;

namespace VictimBot.Lib.Storage.DTO
{
    /// <summary>
    /// Represents a normalised location address.
    /// Likely to be based on a service such as: https://getaddress.io/
    /// </summary>
    public class Address
    {
        public string Raw { get; set; }
    }
}
