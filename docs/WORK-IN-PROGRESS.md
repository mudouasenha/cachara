# TODO:

- Enrich Domain;
    - Notifications;
    - Friendships;
    - Posts Interations;
    - User configuration;
    - Admin features;
- Improve Repository;
- Make use of Results;
- Error Handling and Validations;
- Bulk Add and Admin features;
    - Increment Hangfire with recurringJobs and BackgroundJobs;
- Implement RabbitMQ for user Poking

## Dream:

- Find a use case for SignalR;
- Implement MessagePack serialization;

## Infrastructure

- Repository needs rework;
- Create a PostManagerRepository that can use an IApplicationDbConnection.
- Make use of Dapper, and find a way of using it with Hangfire.

## Notes

### 16/08

feat: added hangfire services, disabled Disposable dbConnection due to hangfire not accepting idisposables;

### 18/12/2024

Authentication and authorization:
User Authentication: JWT token
APIs internal communication: Api Keys

Find a way to store the claims into the apiKey and pass it so other services can make use of it?