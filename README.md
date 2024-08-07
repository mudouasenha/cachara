# Project Cachara

This project aims to be a social media platform.

This project was initiated to practice Domain-Driven Design (DDD) Concepts and experiment with various 
tools for data storage, asynchronous communication and enterprise patterns. 


# Architecture:
- Onion Architecture

## 1. Infrastructure:

### 1.1 SQL:
- Dapper for data extraction queries (WIP);
- EF Core for general use (WIP);

### 1.2 Asynchronous Communication:
- Azure Service Bus;
- RabbitMQ;

## 2. Presentation:
- GraphQL for UI/UX use;
- RESTFul APIs;
- API Versioning;
- Scalar with OpenAPI 3.0 schema;
- Swagger option;

## 3. Security:
- API Authentication and Authorization;
- Password encryption and decryption;
- Admin and Public APIs;

## 4. Tests:
- XUnit;
- Integration Tests;
- Unit Tests;

## 5. Deployment:
- Kubernetes;
- Containers:
  - SqlServer;
  - Seq (logging);
  - RabbitMQ;


## 6. Logging:
- OpenTelemetry Logging (WIP):
  - Seq visualization