#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
#Para que funcione DinkTOPDF
RUN apt-get update && apt-get install -y --no-install-recommends apt-utils libgdiplus libc6-dev && ln -s /usr/lib/libgdiplus.so /usr/lib/gdiplus.dll

WORKDIR /app
EXPOSE 80
EXPOSE 443
ENV TZ=America/Santiago
#ENV LC_ALL=es_CL.UTF-8
#ENV LC_ALL=C.UTF-8
#ENV LC_NUMERIC=C.UTF-8
#ENV LC_MONETARY=es_CL.UTF-8
ENV LC_TIME=es_CL.UTF-8
#ENV LC_ALL=es_CL.UTF-8
#ENV LANG=es_CL.UTF-8
#ENV LANGUAGE=es_CL.UTF-8
#ENV LC_MONETARY=es_CL.UTF-8
#ENV LC_NUMERIC=C.UTF-8

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WebApp/WebApp.csproj", "WebApp/"]
RUN dotnet restore "WebApp/WebApp.csproj"
COPY . .
WORKDIR "/src/WebApp"
RUN dotnet build "WebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApp.dll"]