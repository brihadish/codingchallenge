using CodingChallenge.Lib.Domain;
using Functional.Maybe;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Web.Http;

namespace CodingChallenge.WebApi
{
    public sealed class ZipCodeIndexController : ApiController
    {
        private readonly IZipCodeIndexer _zipCodeIndexer;

        public ZipCodeIndexController(IZipCodeIndexer zipCodeIndexer)
        {
            _zipCodeIndexer = zipCodeIndexer;
        }

        [HttpPost]
        [Route("api/zipcode/search/{searchTerm}")]
        public async Task<IHttpActionResult> SearchAsync(int searchTerm, [FromBody] ZipCodeSearchOptions options)
        {
            var take = options?.Take;
            var token = options?.ContinuationToken;
            var result = await _zipCodeIndexer.SearchAsync(
                new ZipCodeIndexSearchInput(searchTerm, take.HasValue ? take.GetValueOrDefault() : -1,
                    string.IsNullOrWhiteSpace(token) == false ? token : null));
            if (result.IsFailure)
            {
                ZipCodeIndexerErrorType errorType;
                if (Enum.TryParse(result.Error.ErrorType, out errorType))
                {
                    switch (errorType)
                    {
                        case ZipCodeIndexerErrorType.ZipCodeNotFound:
                            return NotFound();

                        case ZipCodeIndexerErrorType.InvalidZipCode:
                            return BadRequest();
                    }
                }
                return Content(HttpStatusCode.InternalServerError, new ZipCodeSearchResult
                {
                    ErrorType = result.Error.ErrorType
                });
            }
            var output = result.ResultValue;
            return Content(HttpStatusCode.OK, new ZipCodeSearchResult
            {
                ZipCodes = output.SearchResults,
                ContinuationToken = output.ContinuationToken
            });
        }
    }

    [DataContract]
    public class ZipCodeSearchOptions
    {
        [DataMember]
        public int Take { get; set; }

        [DataMember]
        public string ContinuationToken { get; set; }
    }

    [DataContract]
    public class ZipCodeSearchResult
    {
        [DataMember]
        public IEnumerable<int> ZipCodes { get; set; }

        [DataMember]
        public string ContinuationToken { get; set; }

        [DataMember]
        public string ErrorType { get; set; }
    }
}
