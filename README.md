Cars API:

-> Database: Azure Cosmos DB
-> Language: Azure Function App with C#

-> when create an id, the id will be the random id like: b0dfea81-b9ef-48c3-b5f8-ee4891dcd718. I have already put some data in database for the test.
 
-> I tried to cerate Search Brand url as [GET] http://localhost:7071/api/car/search?brand=BMW, but azure cosmos db does not work with this url, so i made it like [GET] http://localhost:7071/api/car/search/brand/{brand}.
