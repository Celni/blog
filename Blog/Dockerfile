FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY ["Blog/*.csproj", "./Blog/"]
RUN dotnet restore

# copy everything else and build app
COPY . .
WORKDIR /app/Blog
RUN dotnet build -r linux-arm

FROM build AS publish
WORKDIR /app/Blog
RUN dotnet publish -c Release -o out -r linux-arm


FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-disco-arm32v7 AS runtime
WORKDIR /app
COPY --from=publish /app/Blog/out ./
ENTRYPOINT ["dotnet", "Blog.dll"]