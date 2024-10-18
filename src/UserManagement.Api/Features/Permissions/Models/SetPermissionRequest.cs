namespace UserManagement.Api.Features.Permissions.Models;

public class SetPermissionRequest
{
    public Guid UserId { get; set; }

    public PermissionDetailRequest? Permission { get; set; }
}

public class SetPermissionsRequest
{
    public Guid UserId { get; set; }


    public List<PermissionDetailRequest>? Permissions { get; set; }
}

public class PermissionDetailRequest
{
    public string Group { get; set; } = "";

    public string Permission { get; set; } = "";
}