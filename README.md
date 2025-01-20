# Project Cachara

This project aims to be a social media platform based on microservices architecture.

This project was initiated to practice Domain-Driven Design (DDD) Concepts and experiment with various
tools for data storage, asynchronous communication and enterprise patterns on top of a Microservices Architecture.

---

## Technologies

* .NET Core 8;
* REST Standards;
* MS SQL;
* Hangfire;
* RabbitMQ;

---

## Dashboard

* API Gateway: https://localhost:5000;
* Users API:
    * API: https://localhost:5200/users-api/swagger;
    * --health--;
    * --healthchecks-ui--;
* Content API:
    * API: https://localhost:5202/content-api/swagger;
    * --health--;
    * --healthchecks-ui--;
* GraphQL:
    * API: https://localhost:5202/content-api/swagger;
    * --health--;

* --rabbitmq-management--;
* Seq Log visualization: http://localhost:5341/#/events;

---

# Features / Architecture:

- [X] Onion Architecture for each microservice;
- [ ] Result Pattern;

### 1.1 Data Access:

- [ ] CQRS Pattern;
- [ ] Dapper for data extraction queries;
- [x] EF Core for general use;
- [x] UnitOfWork Pattern;
- [ ] Data Caching;

### 1.2 Asynchronous Communication:

- [x] Azure Service Bus;
    - [x] Queues;
    - [ ] Events;
- [ ] RabbitMQ;

## 2. Presentation:

- [ ] GraphQL for UI/UX use;
- [X] RESTFul APIs;
- [x] API Versioning;
- [ ] Scalar with OpenAPI 3.0 schema; (WIP)
- [ ] Swagger option;
- [ ] Exception Handling Middleware;

## 3. Security:

- [ ] API Authentication and Authorization;
- [x] Password encryption and decryption;
- [ ] Admin and Public APIs;

## 4. Tests:

- [X] XUnit;
- [ ] Integration Tests;
- [ ] Unit Tests;

## 5. Infrastructure & CI/CD:

- [ ] SonarCloud static code analysis
- [x] Consistent coding style with .editorconfig file
- [x] Terraform IaC Scripts;
- [x] Docker-compose scripts;
- [ ] GitHub Actions;
- [ ] Kubernetes;
- [ ] Containers:
    - [ ] SqlServer;
    - [ ] Seq (logging);
    - [ ] RabbitMQ;

## 6. Logging:

- [X] OpenTelemetry Logging:
    - [X] Seq visualization;

## 7. Background Jobs

- [ ] Hangfire;