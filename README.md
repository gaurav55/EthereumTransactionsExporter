
# Ethereum Transaction Fetcher API (.NET Core 8.0)

A simple, scalable .NET core Web API that fetches Ethereum wallet transactions using Alchemy API and generates CSV output.

---

# Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [VS Code](https://code.visualstudio.com/)
- Alchemy API Key
- Git

---
# Project structure 

- EthereumTransactionsExporter.API            ->  Web API project - main entry point
- EthereumTransactionsExporter.Application    ->  Application logic
- EthereumTransactionsExporter.Domain         ->  Domain models and interfaces
- EthereumTransactionsExporter.Infrastructure  ->  Implementation of services (e.g. API calls, CSV generation)
- EthereumTransactionsExporter.UnitTests      ->  Unit tests for application
----
# Few design highlights

- Clean architecture design principle used.
- Used dependency injection to provide lose coupling between services and module .
- Unit testing controller and services. For now just 1 running test is written. Added few dummy tests without any implementation.
- Logging exceptions and information messages.

# Areas of improvement 
### Scaling async with the use of Queue
- Use queue to store addresses 
- Worker service fetches and processes transactions
- Use Hosted Service for background jobs
- Retry mechanism 
- Rate limiting


# Setup Instructions for macOS and VS Code

### 1. Clone the Repository

```bash
https://github.com/gaurav55/EthereumTransactionsExporter.git

```

### 2. Set Environment Variable

Update the AlchemyApiKey Api key in appsettings.json file of EthereumTransactionsExporter.API project. 
```
apiky = youAlchemyApiKey

```

### 3. Restore Dependencies
```bash
dotnet restore
```

### 4. Run the API

```
dotnet run --project EthereumTransactionApi

```

### API can be accessed on swagger at:
```
http://localhost:5150/swagger/index.html
```

### Curl to make a request

```
curl -X 'GET' \
  'http://localhost:5150/api/transacations/0xa39b189482F984388A34460636Fea9Eb181ad1a6/csv' \
  -H 'accept: */*'
```

### How to Run the Tests

Navigate to the test project folder 

```
cd EthereumTransactionApi.Tests
```

### Run the tests using dotnet test:

```
dotnet test
```
