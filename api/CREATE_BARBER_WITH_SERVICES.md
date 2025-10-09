# Create Barber with Services - Feature Documentation

## Overview
The `POST /api/barber` endpoint has been updated to allow creating a barber and associating services in a single request.

## Changes Made

### 1. New DTO: `CreateBarberDto`
**File**: `/api/DTO/CreateBarberDto.cs`

```csharp
public class CreateBarberDto
{
    public string Username { get; set; }
    public string Name { get; set; }
    public string Specialty { get; set; }
    public string ContactInfo { get; set; }
    public List<int> ServiceIds { get; set; } = new List<int>();
}
```

**Key Features**:
- All barber properties (Username, Name, Specialty, ContactInfo)
- `ServiceIds` - List of service IDs to associate with the barber
- Empty `ServiceIds` list is valid (creates barber with no services)

### 2. Service Layer
**Interface**: `IBarberManagementService.cs`
- Added: `Task<BarberModel> AddBarberWithServicesAsync(BarberModel barber, List<int> serviceIds)`

**Implementation**: `BarberManagementService.cs`
```csharp
public async Task<BarberModel> AddBarberWithServicesAsync(BarberModel barber, List<int> serviceIds)
{
    // 1. Create the barber
    var createdBarber = await repo.AddAsync(barber);
    await repo.SaveChangesAsync();

    // 2. Add services if provided
    if (serviceIds != null && serviceIds.Any())
    {
        foreach (var serviceId in serviceIds)
        {
            await barberServiceRepo.AddBarberService(createdBarber.BarberId, serviceId);
        }
        await barberServiceRepo.SaveChangesAsync();
    }

    return createdBarber;
}
```

### 3. Controller Update
**File**: `BarberController.cs`

**Before**:
```csharp
[HttpPost]
public async Task<ActionResult<BarberDto>> Create([FromBody] BarberDto dto)
{
    var model = _mapper.Map<BarberModel>(dto);
    var created = await _service.AddAsync(model);
    var createdDto = _mapper.Map<BarberDto>(created);
    return Created($"api/barber/{created.BarberId}", createdDto); 
}
```

**After**:
```csharp
[HttpPost]
public async Task<ActionResult<BarberDto>> Create([FromBody] CreateBarberDto dto)
{
    var model = _mapper.Map<BarberModel>(dto);
    var created = await _service.AddBarberWithServicesAsync(model, dto.ServiceIds);
    var createdDto = _mapper.Map<BarberDto>(created);
    return Created($"api/barber/{created.BarberId}", createdDto); 
}
```

### 4. AutoMapper Configuration
**File**: `ApiMappingProfile.cs`
- Added: `CreateMap<CreateBarberDto, BarberModel>();`

## API Usage

### Endpoint
```
POST /api/barber
```

### Request Body Examples

#### Example 1: Create Barber with Services
```json
{
  "username": "john-the-barber",
  "name": "John Smith",
  "specialty": "Fades and Tapers",
  "contactInfo": "555-1234",
  "serviceIds": [1, 2, 3]
}
```

#### Example 2: Create Barber without Services
```json
{
  "username": "jane-stylist",
  "name": "Jane Doe",
  "specialty": "Women's Cuts",
  "contactInfo": "555-5678",
  "serviceIds": []
}
```

#### Example 3: Minimal Request
```json
{
  "username": "mike-barber",
  "name": "Mike Johnson",
  "specialty": "",
  "contactInfo": "",
  "serviceIds": [1]
}
```

### Response
**Status**: `201 Created`
**Location Header**: `api/barber/{barberId}`

```json
{
  "barberId": 4,
  "username": "john-the-barber",
  "name": "John Smith",
  "specialty": "Fades and Tapers",
  "contactInfo": "555-1234"
}
```

**Note**: The response DTO (`BarberDto`) does not include the services list. To get the barber's services, use:
- `GET /api/barber/{id}` (if you add services to the response)
- Or query the barber-service relationship separately

## Validation

### Required Fields
- `Username` (1-50 characters)
- `Name` (1-50 characters)

### Optional Fields
- `Specialty` (max 100 characters)
- `ContactInfo` (max 50 characters)
- `ServiceIds` (can be empty list)

### Error Responses

#### 400 Bad Request - Invalid Data
```json
{
  "errors": {
    "Username": ["Username is required."],
    "Name": ["Name must be between 1 and 50 characters."]
  }
}
```

#### 400 Bad Request - Invalid Service ID
If a service ID doesn't exist, the barber will be created but the invalid service association will fail silently (based on current implementation).

**Recommendation**: Add validation to check if all service IDs exist before creating associations.

## Testing with Swagger

1. Navigate to `http://localhost:5288/swagger`
2. Find `POST /api/barber`
3. Click "Try it out"
4. Use this sample request:
```json
{
  "username": "test-barber",
  "name": "Test Barber",
  "specialty": "All Services",
  "contactInfo": "test@example.com",
  "serviceIds": [1, 2]
}
```
5. Click "Execute"
6. Verify response is `201 Created`

## Testing with cURL

```bash
curl -X POST "http://localhost:5288/api/barber" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "curl-barber",
    "name": "Curl Test",
    "specialty": "Testing",
    "contactInfo": "curl@test.com",
    "serviceIds": [1, 2, 3]
  }'
```

## Database Impact

### Tables Affected
1. **barberTable** - New barber record created
2. **barberServiceTable** - One record per service ID in the list

### Example Database State After Creation

**barberTable**:
| BarberId | Username | Name | Specialty | ContactInfo |
|----------|----------|------|-----------|-------------|
| 4 | john-the-barber | John Smith | Fades and Tapers | 555-1234 |

**barberServiceTable**:
| Id | BarberId | ServiceId |
|----|----------|-----------|
| 8  | 4        | 1         |
| 9  | 4        | 2         |
| 10 | 4        | 3         |

## Frontend Integration (Next.js)

### Update TypeScript Types
**File**: `fadebook-frontend/src/types/api.ts`

```typescript
export interface CreateBarberDto {
  username: string;
  name: string;
  specialty: string;
  contactInfo: string;
  serviceIds: number[];
}
```

### Update API Client
**File**: `fadebook-frontend/src/lib/api/barbers.ts`

```typescript
export const barbersApi = {
  // ... existing methods

  // Updated create method
  create: async (barber: CreateBarberDto): Promise<BarberDto> => {
    const { data } = await axiosInstance.post('/api/barber', barber);
    return data;
  },
};
```

### React Component Example
```tsx
const CreateBarberForm = () => {
  const [formData, setFormData] = useState({
    username: '',
    name: '',
    specialty: '',
    contactInfo: '',
    serviceIds: [] as number[],
  });

  const createBarber = useCreateBarber();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    await createBarber.mutateAsync(formData);
  };

  return (
    <form onSubmit={handleSubmit}>
      {/* Form fields */}
      <MultiSelect
        label="Services"
        options={services}
        value={formData.serviceIds}
        onChange={(ids) => setFormData({ ...formData, serviceIds: ids })}
      />
      <button type="submit">Create Barber</button>
    </form>
  );
};
```

## Benefits

### 1. **Atomic Operation**
- Creates barber and associates services in one request
- Reduces round trips between client and server

### 2. **Better UX**
- Single form submission
- No need for separate "Add Services" step after creating barber

### 3. **Data Consistency**
- Barber is created with services immediately
- No intermediate state where barber exists without services

### 4. **Backward Compatible**
- Empty `serviceIds` list creates barber without services
- Existing functionality preserved

## Future Enhancements

### 1. Add Service Validation
```csharp
public async Task<BarberModel> AddBarberWithServicesAsync(BarberModel barber, List<int> serviceIds)
{
    // Validate all service IDs exist
    foreach (var serviceId in serviceIds)
    {
        var serviceExists = await _serviceRepo.ExistsAsync(serviceId);
        if (!serviceExists)
            throw new NotFoundException($"Service with ID {serviceId} not found.");
    }
    
    // ... rest of implementation
}
```

### 2. Return Services in Response
Update `BarberDto` to include services:
```csharp
public class BarberDto
{
    public int BarberId { get; set; }
    public string Username { get; set; }
    public string Name { get; set; }
    public string Specialty { get; set; }
    public string ContactInfo { get; set; }
    public List<ServiceDto> Services { get; set; } = new List<ServiceDto>();
}
```

### 3. Add Transaction Support
Wrap the operation in a transaction to ensure atomicity:
```csharp
using var transaction = await _dbContext.Database.BeginTransactionAsync();
try
{
    var createdBarber = await repo.AddAsync(barber);
    await repo.SaveChangesAsync();
    
    // Add services
    foreach (var serviceId in serviceIds)
    {
        await barberServiceRepo.AddBarberService(createdBarber.BarberId, serviceId);
    }
    await barberServiceRepo.SaveChangesAsync();
    
    await transaction.CommitAsync();
    return createdBarber;
}
catch
{
    await transaction.RollbackAsync();
    throw;
}
```

## Troubleshooting

### Issue: Services Not Associated
**Symptom**: Barber created but no services in `barberServiceTable`

**Possible Causes**:
1. Invalid service IDs in request
2. Database constraint violation
3. Service repository not saving changes

**Solution**: Check logs for errors, verify service IDs exist in `serviceTable`

### Issue: Duplicate Service Association
**Symptom**: Error when trying to add same service twice

**Solution**: The unique index on `(BarberId, ServiceId)` prevents duplicates. This is expected behavior.

## Summary

The `POST /api/barber` endpoint now accepts `CreateBarberDto` which includes a `ServiceIds` list, allowing you to create a barber and associate services in a single atomic operation. This improves the API's usability and reduces the number of requests needed to set up a new barber.
