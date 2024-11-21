# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.
# Use the .NET SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the project file and restore dependencies
COPY backend_devops_rejsekort_v2.csproj ./
RUN dotnet restore

# Copy the entire project and build it
COPY . ./ 
RUN dotnet publish -c Release -o out

# Use the ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy the published output from the build stage
COPY --from=build /app/out ./

# Expose port 80 for the application
EXPOSE 80

# Set the entry point to run your application
ENTRYPOINT ["dotnet", "backend_devops_rejsekort_v2.dll"]
