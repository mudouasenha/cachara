# Project Cachara

This project aims to be a social media platform.

This project was initiated to practice Domain-Driven Design (DDD) Concepts and experiment with various 
tools for data storage, asynchronous communication and enterprise patterns. 

---
## Technologies
* .NET Core 8;
* REST Standards;
* MS SQL;
* Hangfire;

---

## Dashboard
* --index--;
* --hangfire--;
* --health--;
* --healthchecks-ui--;
* --rabbitmq-management--;
* --graphql--;
* Seq Log visualization: http://localhost:5341/#/events;

---

# Features / Architecture:
- [ ] Onion Architecture;
- [ ] Result Pattern;

## 1. Infrastructure:
### 1.1 Data Access:
- [ ] CQRS Pattern;
- [ ] Dapper for data extraction queries;
- [x] EF Core for general use;
- [x] UnitOfWork Pattern;

### 1.2 Asynchronous Communication:
- [ ] Azure Service Bus;
- [ ] RabbitMQ;

## 2. Presentation:
- [ ] GraphQL for UI/UX use;
- [ ] RESTFul APIs;
- [x] API Versioning;
- [ ] Scalar with OpenAPI 3.0 schema;
- [ ] Swagger option;
- [ ] Exception Handling Middleware;

## 3. Security:
- [ ] API Authentication and Authorization;
- [x] Password encryption and decryption;
- [ ] Admin and Public APIs;

## 4. Tests:
- [ ] XUnit;
- [ ] Integration Tests;
- [ ] Unit Tests;

## 5. Deployment:
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