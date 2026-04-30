# ProjectUsingMicroServices

## Configuration

Each app now supports service-specific environment variables in addition to `appsettings.json`.

This avoids collisions between shared keys like `ConnectionStrings__DefaultConnection` when you run multiple services on the same machine.

Use these prefixes:

- `SERVICEMESH_WEB__`
- `SERVICEMESH_GATEWAY__`
- `SERVICEMESH_AUTHAPI__`
- `SERVICEMESH_PRODUCTAPI__`
- `SERVICEMESH_COUPONAPI__`
- `SERVICEMESH_SHOPPINGCARTAPI__`
- `SERVICEMESH_ORDERAPI__`
- `SERVICEMESH_EMAILAPI__`
- `SERVICEMESH_REWARDAPI__`

Nested configuration keys use double underscores. Example:

```powershell
$env:SERVICEMESH_WEB__ServiceUrls__ProductAPI = "https://localhost:7000"
$env:SERVICEMESH_AUTHAPI__ConnectionStrings__DefaultConnection = "Server=YOUR_SERVER;Database=ServiceMesh_Auth;Trusted_Connection=True;TrustServerCertificate=True"
$env:SERVICEMESH_AUTHAPI__ApiSettings__JwtOptions__Secret = "replace-with-jwt-signing-secret"
```

Required keys are listed in [.env.example](./.env.example).

## Required Values By Service

`ServiceMesh.MessageBus`
- `SERVICEMESH_SERVICEBUS__ConnectionString`

`ServiceMesh.Web`
- `ServiceUrls__ProductAPI`
- `ServiceUrls__CouponAPI`
- `ServiceUrls__AuthAPI`
- `ServiceUrls__ShoppingCartAPI`
- `ServiceUrls__OrderAPI`

`ServiceMesh.GatewaySolution`
- `ApiSettings__Secret`
- `ApiSettings__Issuer`
- `ApiSettings__Audience`

`ServiceMesh.Services.AuthAPI`
- `ConnectionStrings__DefaultConnection`
- `ApiSettings__JwtOptions__Secret`
- `ApiSettings__JwtOptions__Issuer`
- `ApiSettings__JwtOptions__Audience`

`ServiceMesh.Services.ProductAPI`
- `ConnectionStrings__DefaultConnection`
- `ApiSettings__Secret`
- `ApiSettings__Issuer`
- `ApiSettings__Audience`

`ServiceMesh.Services.CouponAPI`
- `ConnectionStrings__DefaultConnection`
- `Stripe__SecretKey`
- `ApiSettings__Secret`
- `ApiSettings__Issuer`
- `ApiSettings__Audience`

`ServiceMesh.Services.ShoppingCartAPI`
- `ConnectionStrings__DefaultConnection`
- `ApiSettings__Secret`
- `ApiSettings__Issuer`
- `ApiSettings__Audience`
- `ServiceUrls__ProductAPI`
- `ServiceUrls__CouponAPI`

`ServiceMesh.Services.OrderAPI`
- `ConnectionStrings__DefaultConnection`
- `ApiSettings__Secret`
- `ApiSettings__Issuer`
- `ApiSettings__Audience`
- `ServiceUrls__ProductAPI`
- `Stripe__SecretKey`

`ServiceMesh.Services.EmailAPI`
- `ConnectionStrings__DefaultConnection`
- `ServiceBusConnectionString`

`ServiceMesh.Services.RewardAPI`
- `ConnectionStrings__DefaultConnection`
- `ServiceBusConnectionString`

## Notes

- Environment variables override values from `appsettings.json`.
- `.env.example` is a reference file only. ASP.NET Core does not load `.env` automatically.
- If you use Visual Studio or launch profiles, set these values in each project’s debug environment settings or in your shell before starting the service.
