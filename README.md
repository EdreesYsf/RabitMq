# RabitMq Library (`Company.Shared.Messaging` & `Company.Shared.Contracts`)

A shared `.NET` library solution for RabbitMQ messaging and shared contracts across services, configured with automated builds and publishing via **GitHub Actions** and **NuGet Trusted Publishing**.

## Solution Projects

* **`Company.Shared.Contracts`**: Shared contracts, models, and message definitions used across services.
* **`Company.Shared.Messaging`**: RabbitMQ connection handling, publishing, and consuming messages.

## Installation

Install the packages into your `.NET` project:

```bash
dotnet add package Company.Shared.Contracts
dotnet add package Company.Shared.Messaging