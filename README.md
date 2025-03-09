```markdown
# Project Cachara - Social Media Platform

Project Cachara is a **social media platform** designed using **.NET Core 8** and a **Microservices Architecture** to simulate real-world engineering challenges.

This project was initiated to:

✅ Practice **Domain-Driven Design (DDD)** principles  
✅ Experiment with **asynchronous communication**, **data storage**, and **enterprise patterns**  
✅ Develop a scalable and maintainable application following modern software design practices  

Built with a strong focus on **clean code**, **scalability**, and **observability**, this project leverages tools like **Azure Service Bus**, **RabbitMQ**, and **Hangfire**.

---

## 🌍 Live Demo

🔗 *Deployment link coming soon*

---

## 🛠️ Technologies

---

## 🚀 Getting Started

### Prerequisites

- .NET Core 8  
- Docker & Docker Compose  
- Visual Studio / Visual Studio Code  

### Clone the Repository

```bash
git clone https://github.com/yourusername/project-cachara.git
cd project-cachara
```

### Running the Application

1. Install dependencies
   ```bash
   dotnet restore
   ```
2. Start the application with Docker Compose
   ```bash
   docker-compose up
   ```
3. Access the following services:
    - **API Gateway:** [http://localhost:5000](http://localhost:5000)
    - **Seq Logs:** [http://localhost:5341/#/events](http://localhost:5341/#/events)

---

## 📊 Dashboard Links

### 🟣 Core Services

- **API Gateway:** [https://localhost:5000](https://localhost:5000)
- **Users API:** [https://localhost:5200/users-api/swagger](https://localhost:5200/users-api/swagger)
- **Content API:** [https://localhost:5202/content-api/swagger](https://localhost:5202/content-api/swagger)

### 🔎 Monitoring & Logging

- **Seq (Log Visualization):** [http://localhost:5341/#/events](http://localhost:5341/#/events)
- **RabbitMQ Management UI:** *(Add URL if applicable)*

### 🔒 Health Checks

- **Users API Health:** `/health`
- **Content API Health:** `/health`

---

## 🔹 Features

- **Onion Architecture** — Promotes maintainability by enforcing clear separation of concerns.
- **CQRS Pattern** (In Progress) — Implements distinct models for reads and writes to improve scalability.
- **OpenTelemetry Logging** — Ensures observability through structured logging and performance tracking.
- **API Versioning** for better flexibility when evolving services.

---

## 🎯 What I Learned

- Applying **Domain-Driven Design** concepts in real-world scenarios.
- Building a scalable architecture with **Azure Service Bus** and **RabbitMQ**.
- Managing background jobs effectively with **Hangfire**.
- Improving observability through **OpenTelemetry Logging** and **Seq**.

---

## 🧪 Tests

- ✅ **XUnit** for unit and integration tests.
- 🚧 **Integration Tests** (In Progress).

---

## 📦 Infrastructure & CI/CD

- ✅ **Terraform** for Infrastructure as Code (IaC).
- ✅ **Docker Compose** for simplified environment setup.
- 🚧 **GitHub Actions** (Planned).
- 🚧 **Kubernetes** for container orchestration (Planned).

---

## 📬 Contact

📧 Email: [your.email@example.com](mailto:your.email@example.com)  
💼 LinkedIn: [linkedin.com/in/yourprofile](https://linkedin.com/in/yourprofile)  
🐙 GitHub: [github.com/yourusername](https://github.com/yourusername)

---

💡 *If you find this project helpful or have suggestions for improvements, feel free to open an issue or submit a pull request!*
```

