using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<UserManagement_Api>("api");

builder.Build().Run();
