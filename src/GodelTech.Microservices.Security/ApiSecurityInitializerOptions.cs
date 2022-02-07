using System.Net;

namespace GodelTech.Microservices.Security
{
    /// <summary>
    /// ApiSecurity initializer options.
    /// </summary>
    public class ApiSecurityInitializerOptions
    {
        /// <summary>
        /// Clear DefaultInboundClaimTypeMap.
        /// </summary>
        public bool ClearDefaultInboundClaimTypeMap { get; set; } = true;

        /// <summary>
        /// Clear DefaultOutboundClaimTypeMap.
        /// </summary>
        public bool ClearDefaultOutboundClaimTypeMap { get; set; } = true;

        /// <summary>
        /// SecurityProtocol.
        /// </summary>
        public SecurityProtocolType SecurityProtocol { get; set; } = SecurityProtocolType.SystemDefault;
    }
}