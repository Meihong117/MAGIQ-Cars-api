Cars API:

-> Database: Azure Cosmos DB
-> Language: Azure Function App with C# .net core
 
-> I tried to cerate Search Brand url as [GET] http://localhost:7071/api/car/search?brand=BMW, but azure cosmos db did not work with this url, so I made it via [GET] http://localhost:7071/api/car/search/brand/{brand}.

