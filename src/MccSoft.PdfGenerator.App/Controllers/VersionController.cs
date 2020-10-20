using System;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MccSoft.PdfGenerator.App.Controllers
{
    /// <summary>
    /// Shows the info about the service.
    /// </summary>
    [ApiController]
    public class VersionController
    {
        /// <summary>
        /// Gets the version of the service.
        /// </summary>
        /// <returns>A string representing the version.</returns>
        [AllowAnonymous]
        [HttpGet("api")]
        public string Version()
        {
            var attribute = typeof(VersionController).GetTypeInfo()
                .Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

            if (attribute == null)
            {
                throw new Exception("Can not retrieve api version");
            }

            return attribute.InformationalVersion;
            ;
        }

        /// <summary>
        /// Demonstrates an error response.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("error-test")]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public string ThrowError()
        {
            throw new ArgumentException("Invalid arg.", "argName");
        }
    }
}