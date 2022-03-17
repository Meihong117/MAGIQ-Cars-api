Cars API:

-> Database: Azure Cosmos DB
-> Language: Azure Function App with C#

-> when an id is to be created, it will be formed randomly an example like: b0dfea81-b9ef-48c3-b5f8-ee4891dcd718. I have already put some data in database for testing purpose.
 
-> I tried to cerate Search Brand url as [GET] http://localhost:7071/api/car/search?brand=BMW, but azure cosmos db did not work with this url, so I made it via [GET] http://localhost:7071/api/car/search/brand/{brand}.
