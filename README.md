
# Ethereum Transaction Fetcher API (.NET Core)

A simple, scalable .NET core Web API that fetches Ethereum wallet transactions using Alchemy API and generates CSV output.

---

# Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [VS Code](https://code.visualstudio.com/)
- Alchemy API Key
- Git

---

# Setup Instructions for macOS and VS Code

# 1. Clone the Repository

```bash
https://github.com/gaurav55/EthereumTransactionsExporter.git

```

# 2. Set Environment Variable

Update the AlchemyApiKey Api key in appsettings.json file 
```
apiky = youAlchemyApiKey

```

# 3. Restore Dependencies
```bash
dotnet restore
```

# 4. Run the API

```
dotnet run --project EthereumTransactionApi

```

# API can be accessed on swagger at:
```
http://localhost:5150/swagger/index.html
```

# Curl to make a request

```
curl -X 'GET' \
  'http://localhost:5150/api/transacations/0xa39b189482F984388A34460636Fea9Eb181ad1a6/csv' \
  -H 'accept: */*'
```

# How to Run the Tests

Navigate to the test project folder 

```
cd EthereumTransactionApi.Tests
```

# Run the tests using dotnet test:

```
dotnet test
```
