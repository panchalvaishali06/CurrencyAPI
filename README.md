This project provides an API for managing and retrieving exchange rate data. The `ExchangeRatesController` exposes endpoints for fetching the latest exchange rates, converting currencies, and retrieving historical exchange rate data.

## Endpoints

### 1. Get Latest Exchange Rates
**URL:** `GET /api/ExchangeRates/latestExRate/v1`  
**Authorization:** Required  
**Query Parameters:**
- `baseCurrency` (string): The base currency for which to fetch exchange rates.

**Response:**  
Returns the latest exchange rates for the specified base currency.

---

### 2. Convert Currency
**URL:** `GET /api/ExchangeRates/convert/v1`  
**Authorization:** Required  
**Query Parameters:**
- `from` (string): The source currency.
- `to` (string): The target currency.
- `amount` (decimal): The amount to convert.

**Response:**  
Returns the converted amount.

**Error Handling:**  
Returns a `400 Bad Request` if the conversion fails due to invalid input.

---

### 3. Get Historical Exchange Rates
**URL:** `GET /api/ExchangeRates/history/v1`  
**Authorization:** Required  
**Query Parameters:**
- `baseCurrency` (string): The base currency for which to fetch historical rates.
- `start` (DateTime): The start date of the historical range.
- `end` (DateTime): The end date of the historical range.
- `page` (int, optional): The page number for pagination (default is 1).
- `pageSize` (int, optional): The number of records per page (default is 10).

**Response:**  
Returns a paginated list of historical exchange rates.

**Error Handling:**  
Returns a `400 Bad Request` if the `start` date is after the `end` date.

---

## Dependencies
- **Microsoft.AspNetCore.Authorization**
- **Microsoft.AspNetCore.Mvc**

## How to Use
1. Inject the `CurrencyService` into the `ExchangeRatesController`.
2. Ensure proper authentication is configured for the API.
3. Use the provided endpoints to interact with exchange rate data.

## Error Handling
- Invalid input or operations will return appropriate HTTP status codes and error messages.
