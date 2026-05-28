# Payment System Repository

## Dewacloud Deploy

Goal: Access API at: https://evangelion.user.cloudjkt02.com/api/...

Architecture:
Jelastic Proxy → .NET Server (port 5241)

### 1. Create Environment

Environment must include:

- .NET Server Node
- (Optional) MongoDB Node
- Ignore Nginx — don’t need it for this setup.

### 2. Deploy .NET API

From Dewacloud:

- Open .NET node
- Deploy from GitHub (Auto deploy)
- Build the project

### 3. Configure Kestrel to Listen on All Interfaces

In Program.cs:

```csharp
builder.WebHost.ConfigureKestrel(o =>
{
    o.ListenAnyIP(5241);
});
```

Or via appsettings.json:

```json
{
  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://0.0.0.0:5241"
      }
    }
  }
}
```

This ensures the app is reachable internally.

### 4. Start the API

SSH into the .NET node:

curl http://localhost:5241/api/payment

If we get a response → good.

### 5. Create a Public Endpoint for Port 5241

In Dewacloud dashboard:

Environment → Settings → Endpoints → Add Endpoint

```
Internal Port: 5241
Protocol: HTTP
Public: Yes
```

Example:

<img src="Resources/figure.png"/>

This exposes API to the external Jelastic proxy.

### 6. Bind Domain to the .NET Node

Environment → Settings → Custom Domains

Bind: ```evangelion.user.cloudjkt02.com```

To:
.NET Server Node
Port 5241 (via endpoint above)

HTTPS is automatically handled by Dewacloud (Let’s Encrypt).

### 8. Test Public URL

Open: ```https://evangelion.user.cloudjkt02.com/api/payment```

If it works → deployment is correct.

### COMPLETE CHECKLIST (PRINT THIS)

This is the complete minimal setup:

1. Create environment with .NET node
2. Deploy .NET API from GitHub
3. Configure Kestrel → ListenAnyIP(5241)
4. Run API → test on localhost:5241
5. Create Endpoint → internal 5241 → public
6. Bind domain → evangelion.user.cloudjkt02.com → .NET node
7. Access API over HTTPS

No need to:

- configure Nginx
- reload Nginx
- open firewall manually
- use systemctl
- manage certs
