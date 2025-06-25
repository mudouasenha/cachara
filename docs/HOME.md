## How to Run:

#### Redis in docker

docker run -d -p 6379:6379 -v redis-data:/data --name redis redis:latest

#### How to watch Redis Data?

Using Redis Insight:
https://redis.io/insight/
  

#### Run  GitHub Actions Locally:
✅ How to run GitHub Actions locally with act
1. Install act
  
Windows: via Scoop
```bash 
scoop install act
```

Linux:
```bash 
curl https://raw.githubusercontent.com/nektos/act/master/install.sh | sudo bash
```

2. Run your workflow In the root of your repo (where .github/workflows/ exists):

```bash
act
```
To run a specific event:
```bash
act push       # or pull_request, workflow_dispatch, etc.
```

To list available jobs:
```bash
act -l
```

3. Use secrets locally
   Create a .secrets file:
```bash
MY_SECRET=value
```

Then run:
```bash
act -s MY_SECRET=value
```

#### Migrations

dotnet ef migrations add Initial --context CacharaUsersDbContext --startup-project Cachara.Users.API -o
Infrastructure/Data/Migrations

dotnet ef database update --context CacharaUsersDbContext --startup-project Cachara.Users.API  


#### Redis

Start redis container:

docker run -d --name redis -p 6379:6379 -v redis-data:/data redis

#### Seq

docker run -d --name seq -e ACCEPT_EULA=Y -p 5341:80 -v seq-data:/data datalust/seq

### TODO: 
MANAGE:
- UserAccountService;
- SessionStoreService;
- SessionValidationMiddleware;
- UserAuthenticationService;
- Controller Authentication;
- Redis healthCheck;
- Cache Provider;
- Login and Logout;

- Use CloudEvents
  https://cloudevents.io/