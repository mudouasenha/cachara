## How to Run:

#### Redis in docker
docker run -d -p 6379:6379 -v redis-data:/data --name redis redis:latest

#### How to watch Redis Data?
Using Redis Insight:
https://redis.io/insight/


#### Migrations
dotnet ef migrations add Initial --context CacharaUsersDbContext --startup-project Cachara.Users.API -o Infrastructure/Data/Migrations

