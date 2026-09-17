# ABC Pharmacy - .NET 8 Web API + Angular SPA

A sample pharmacy management application implementing:
- Medicine list with Name, Expiry Date, Quantity, Price and Brand
- Notes stored but not displayed in the grid
- Red row when expiry is within 30 days (or already expired)
- Yellow row when quantity is below 10
- Search by medicine name
- Add medicine
- Record medicine sales; sale reduces stock and creates a sale record
- JSON files used as server-side persistence

## Prerequisites
- .NET 8 SDK
- Node.js 20.19+ (or a compatible current Node version)
- Angular CLI: `npm install -g @angular/cli`

## 1. Run the API
```bash
cd server/ABCPharmacy.Api
dotnet restore
dotnet run
```

The API uses `http://localhost:5000` and Swagger is available at:
`http://localhost:5000/swagger`

## 2. Run Angular
Open another terminal:
```bash
cd client/abc-pharmacy
npm install
npm start
```

Open:
`http://localhost:4200`

## API endpoints

### Medicines
- `GET /api/medicines`
- `GET /api/medicines?search=paracetamol`
- `GET /api/medicines/{id}`
- `POST /api/medicines`

### Sales
- `GET /api/sales`
- `POST /api/sales`

Example sale:
```json
{
  "medicineId": 1,
  "quantity": 2,
  "soldBy": "admin"
}
```

JSON data is stored under:
`server/ABCPharmacy.Api/Data/medicines.json`
`server/ABCPharmacy.Api/Data/sales.json`

## Important implementation notes

This sample intentionally uses JSON files rather than a database, as required. For production use, add authentication/authorization, validation, concurrency control, audit logging, transactional persistence and a database such as SQL Server/PostgreSQL.
