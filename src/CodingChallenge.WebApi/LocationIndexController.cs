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
    public sealed class LocationIndexController : ApiController
    {
        private readonly IStringIndexer _stringIndexer;

        public LocationIndexController(IStringIndexer stringIndexer)
        {
            _stringIndexer = stringIndexer;
        }

        [HttpPost]
        [Route("api/location/search/{searchTerm}")]
        public async Task<IHttpActionResult> SearchAsync(string searchTerm, [FromBody] ZipCodeSearchOptions options)
        {
            var take = options?.Take;
            var token = options?.ContinuationToken;
            var result = await _stringIndexer.SearchAsync(new StringIndexerSearchInput(
                searchTerm, take.HasValue ? take.Value.ToMaybe() : Maybe<int>.Nothing,
                string.IsNullOrWhiteSpace(token) == false ? token.ToMaybe() : Maybe<string>.Nothing));
            if (result.IsFailure)
            {
                StringIndexerErrorType errorType;
                if (Enum.TryParse(result.Error.ErrorType, out errorType))
                {
                    switch (errorType)
                    {
                        case StringIndexerErrorType.IndexNotFound:
                            return NotFound();

                        case StringIndexerErrorType.EmptySearchValue:
                            return BadRequest();
                    }
                }
                return Content(HttpStatusCode.InternalServerError, new LocationSearchResult
                {
                    ErrorType = result.Error.ErrorType
                });
            }
            var output = result.ResultValue;
            return Content(HttpStatusCode.OK, new LocationSearchResult
            {
                Locations = output.SearchResults,
                ContinuationToken = output.ContinuationToken
            });
        }
    }

    [DataContract]
    public class LocationSearchOptions
    {
        [DataMember]
        public int Take { get; set; }

        [DataMember]
        public string ContinuationToken { get; set; }
    }

    [DataContract]
    public class LocationSearchResult
    {
        [DataMember]
        public IEnumerable<string> Locations { get; set; }

        [DataMember]
        public string ContinuationToken { get; set; }

        [DataMember]
        public string ErrorType { get; set; }
    }
}
