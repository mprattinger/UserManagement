using FlintSoft.Endpoints;
using UserManagement.Api.Features.BusinessPartner.Models;

namespace UserManagement.Api.Features.BusinessPartner.Endpoints;

public class BusinessPartnerEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/bp", () =>
        {
            var bps = new List<BusinessPartnerModel>()
            {
                new BusinessPartnerModel { Id = Guid.NewGuid(), Name1 = "ACME", Name2 = "Industries", Search = "ACME"},
                new BusinessPartnerModel { Id = Guid.NewGuid(), Name1 = "Start Industries", Name2 = "Tony Stark", Search = "STARK"}
            };

            return Results.Ok(bps);
        })
            .RequireAuthorization("MultiScheme", "BP.Read");
    }
}
