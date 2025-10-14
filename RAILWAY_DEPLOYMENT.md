# Railway Deployment Guide

This guide will help you deploy the Fadebook application to Railway with Azure SQL Database.

## Prerequisites

1. **Railway Account**: Sign up at [railway.app](https://railway.app)
2. **Azure SQL Database**: Set up Azure SQL Database
3. **GitHub Repository**: Your code should be in a GitHub repository

## Project Structure

The project consists of two services:
- **Backend (API)**: .NET 9.0 API (`Fadebook.Api/`)
- **Frontend**: Next.js application (`Fadebook.Frontend/`)

## Step 1: Set Up Azure SQL Database

1. Go to [Azure Portal](https://portal.azure.com)
2. Create a new SQL Database
3. Configure firewall rules to allow connections from Railway
   - Add Azure services: Yes
   - Add Railway IP addresses (you'll get these from Railway logs)
4. Get your connection string from Azure Portal:
   ```
   Server=tcp:your-server.database.windows.net,1433;Database=YourDatabase;User ID=youradmin;Password=yourpassword;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
   ```

## Step 2: Deploy Backend API to Railway

1. **Create New Project in Railway**
   - Go to Railway dashboard
   - Click "New Project"
   - Select "Deploy from GitHub repo"
   - Choose your repository

2. **Configure Backend Service**
   - Railway will detect the `Dockerfile` in `Fadebook.Api/`
   - Set the root directory to `Fadebook.Api` (if needed)

3. **Set Environment Variables for Backend**

   Go to your backend service settings and add these variables:

   ```bash
   # Database Connection
   CONNECTION_STRING=Server=tcp:your-server.database.windows.net,1433;Database=YourDatabase;User ID=youradmin;Password=yourpassword;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;

   # JWT Configuration
   JWT_SECRET_KEY=your-super-secret-key-at-least-32-chars-long
   JWT_ISSUER=FadebookAPI
   JWT_AUDIENCE=FadebookClient
   JWT_EXPIRATION_MINUTES=60

   # ASP.NET Core Configuration
   ASPNETCORE_URLS=http://0.0.0.0:8080
   ASPNETCORE_ENVIRONMENT=Production

   # CORS - Will be set after frontend deployment
   FRONTEND_URL=https://your-frontend-url.railway.app
   ```

4. **Generate Domain**
   - Railway will automatically generate a public domain
   - Copy this URL (e.g., `https://your-api.railway.app`)

## Step 3: Deploy Frontend to Railway

1. **Add New Service to Same Project**
   - In your Railway project, click "New Service"
   - Select "GitHub Repo" (same repository)
   - Set root directory to `Fadebook.Frontend`

2. **Set Environment Variables for Frontend**

   ```bash
   # API Configuration
   NEXT_PUBLIC_API_URL=https://your-api.railway.app

   # Node Environment
   NODE_ENV=production
   ```

3. **Generate Domain**
   - Railway will generate a public domain for frontend
   - Copy this URL

## Step 4: Update Backend CORS

1. Go back to your Backend service
2. Update the `FRONTEND_URL` environment variable with your frontend URL:
   ```bash
   FRONTEND_URL=https://your-frontend.railway.app
   ```
3. Railway will automatically redeploy the backend

## Step 5: Run Database Migrations

After the backend is deployed, you need to run migrations:

### Option A: Using Railway CLI (Recommended)

1. Install Railway CLI:
   ```bash
   npm i -g @railway/cli
   ```

2. Login and link to your project:
   ```bash
   railway login
   railway link
   ```

3. Run migrations:
   ```bash
   railway run dotnet ef database update --project Fadebook.Api
   ```

### Option B: Using Local Development

1. Temporarily update your local `.env` file with Azure SQL connection string
2. Run migrations locally:
   ```bash
   cd Fadebook.Api
   dotnet ef database update
   ```

## Step 6: Verify Deployment

1. **Check Backend Health**
   - Visit `https://your-api.railway.app/swagger`
   - You should see the API documentation

2. **Check Frontend**
   - Visit `https://your-frontend.railway.app`
   - Try logging in with seed data credentials

3. **Monitor Logs**
   - Check Railway logs for both services
   - Look for any errors or warnings

## Environment Variables Reference

### Backend (.NET API)

| Variable | Description | Example |
|----------|-------------|---------|
| `CONNECTION_STRING` | Azure SQL connection string | `Server=tcp:...` |
| `JWT_SECRET_KEY` | Secret key for JWT signing | At least 32 characters |
| `JWT_ISSUER` | JWT token issuer | `FadebookAPI` |
| `JWT_AUDIENCE` | JWT token audience | `FadebookClient` |
| `JWT_EXPIRATION_MINUTES` | Token expiration time | `60` |
| `FRONTEND_URL` | Frontend URL for CORS | `https://frontend.railway.app` |
| `ASPNETCORE_URLS` | Server listening URL | `http://0.0.0.0:8080` |
| `ASPNETCORE_ENVIRONMENT` | Environment name | `Production` |

### Frontend (Next.js)

| Variable | Description | Example |
|----------|-------------|---------|
| `NEXT_PUBLIC_API_URL` | Backend API URL | `https://api.railway.app` |
| `NODE_ENV` | Node environment | `production` |

## Seed Data Credentials

After deployment, you can login with these default accounts:

- **Admin**: `admin` / `admin123`
- **Barber**: `dean.barber` / `barber123`
- **Customer**: `john.customer` / `customer123`

## Troubleshooting

### Backend Won't Start

1. Check Railway logs for error messages
2. Verify all environment variables are set
3. Ensure Azure SQL firewall allows Railway connections

### Database Connection Fails

1. Test connection string locally
2. Add Railway IP to Azure SQL firewall rules
3. Check if SQL Server is running
4. Verify credentials in connection string

### CORS Errors

1. Verify `FRONTEND_URL` matches your frontend domain exactly
2. Ensure protocol is included (https://)
3. Check Railway logs for CORS-related errors

### Frontend Can't Connect to Backend

1. Verify `NEXT_PUBLIC_API_URL` is correct
2. Check if backend is responding at `/swagger`
3. Look for network errors in browser console

## Updating Your Application

Railway automatically deploys when you push to your connected GitHub branch:

1. Make changes locally
2. Commit and push to GitHub
3. Railway will automatically rebuild and deploy

## Cost Considerations

- **Railway**: Free tier includes $5 credit/month, then pay-as-you-go
- **Azure SQL Database**: Choose appropriate tier (Basic, Standard, Premium)
- Monitor usage to avoid unexpected costs

## Security Best Points

1. **Never commit `.env` files** to GitHub
2. **Use strong passwords** for SQL Server
3. **Rotate JWT secrets** regularly
4. **Enable HTTPS** (Railway does this by default)
5. **Review firewall rules** regularly
6. **Monitor logs** for suspicious activity

## Additional Resources

- [Railway Documentation](https://docs.railway.app)
- [Azure SQL Documentation](https://docs.microsoft.com/azure/sql-database/)
- [ASP.NET Core Deployment](https://docs.microsoft.com/aspnet/core/host-and-deploy/)
- [Next.js Deployment](https://nextjs.org/docs/deployment)

## Support

If you encounter issues:
1. Check Railway logs
2. Review Azure SQL metrics
3. Consult the documentation links above
4. Check GitHub repository issues
