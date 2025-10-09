# Database Migration Instructions

## Changes Made

1. **Added Many-to-Many Navigation Properties**:
   - `BarberModel.BarberServices` - Collection of BarberServiceModel
   - `ServiceModel.BarberServices` - Collection of BarberServiceModel

2. **Updated DbContext Configuration**:
   - Added unique composite index on (BarberId, ServiceId) to prevent duplicates
   - Configured proper many-to-many relationships with cascade delete
   - Both sides now reference the join entity properly

3. **Fixed Critical Bug**:
   - Changed `FirstAsync()` to `FirstOrDefaultAsync()` in `BarberServiceRepository.GetBarberServiceByBarberIdServiceId()`
   - This prevents "Sequence contains no elements" exception when no matching record exists

## Steps to Apply Migration

### 1. Create a new migration
```bash
cd /Users/charlest/Documents/Revature/Dev/TheNightOwls/api
dotnet ef migrations add AddManyToManyBarberService
```

### 2. Review the migration
Check the generated migration file in the `Migrations` folder to ensure it:
- Adds a unique index on `IX_barberServiceTable_BarberId_ServiceId`
- Updates foreign key relationships

### 3. Apply the migration to the database
```bash
dotnet ef database update
```

### 4. Run the application
```bash
dotnet run
```

## What This Fixes

- **CORS Error**: Already fixed in Program.cs
- **"Sequence contains no elements" Error**: Fixed by using `FirstOrDefaultAsync()` instead of `FirstAsync()`
- **Many-to-Many Relationship**: Properly configured with navigation properties on both sides
- **Duplicate Prevention**: Unique index ensures a barber can't be assigned the same service twice

## Testing

After migration, test:
1. GET `/api/barber` - Should return all barbers
2. PUT `/api/barber/{id}/services` - Should update barber services without errors
3. GET `/api/customerappointment/barbers-by-service/{serviceId}` - Should return barbers for a service
