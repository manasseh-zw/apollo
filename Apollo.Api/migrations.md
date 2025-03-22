For adding migrations:

```bash
dotnet ef migrations add InitialCreate --context ApolloDbContext --project Apollo.Data --startup-project Apollo.Api
```

For updating the database:

```bash
dotnet ef database update --context ApolloDbContext --project Apollo.Data --startup-project Apollo.Api
```
