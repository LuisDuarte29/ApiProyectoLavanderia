FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# WORKDIR establece la carpeta de trabajo dentro del contenedor.
# Sin esto: los siguientes comandos se ejecutarían en una ubicación incorrecta.
WORKDIR /src
# COPY copia primero el archivo del proyecto.
# ./ representa la carpeta /src configurada arriba.
#Necesito el .csproj solamente para restaurar las dependencias y hago separado del codigo, 
#para que no se vuelva a restaurar cada vez que cambie el codigo entonces si cambio el codigo, no va a reconstruir la imagen desde cero
COPY PracticaJWTcore.csproj ./
# RUN ejecuta un comando durante la construcción.
# dotnet restore descarga las dependencias NuGet.
# Sin esto: podrían faltar paquetes al compilar.
RUN dotnet restore PracticaJWTcore.csproj
# Copia todo el código restante del backend.
# El primer punto es la carpeta local y el segundo es /src.
# Sin esto: solamente existiría el archivo csproj, pero no el código.
#Ahora si cambio el codigo, separo de .csproj, entonces no se vuelve a restaurar las dependencias, solo se compila el codigo
COPY . ./
# dotnet publish compila y prepara la aplicación para ejecutarse.
# -c Release utiliza la configuración optimizada.
# -o indica la carpeta de salida.
# UseAppHost=false evita generar un ejecutable dependiente del sistema anfitrión.
# Sin esto: no existiría una aplicación publicada para ejecutar.
RUN dotnet publish PracticaJWTcore.csproj -c Release -o /app/publish /p:UseAppHost=false
# Inicia una segunda etapa con el runtime de ASP.NET Core.
# Esta imagen es más pequeña porque no incluye el compilador.
# AS final identifica la etapa definitiva.
# Sin esto: el contenedor final tendría que usar el SDK completo.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Define /app como carpeta de ejecución.
# Sin esto: la aplicación podría buscar sus archivos en otra ubicación.
WORKDIR /app

# --from=build obtiene archivos de la etapa llamada build.
# Copia la publicación compilada dentro de /app.
# Sin esto: el contenedor final estaría vacío.
#Copiá desde la etapa build la carpeta /app/publish
#hacia la carpeta actual de esta nueva etapa final por eso el punto final al final de la instrucción COPY
COPY --from=build /app/publish .

# Documenta que la aplicación escucha internamente en el puerto 8080.
# Sin esto: Compose todavía podría publicar el puerto, pero quedaría menos claro.
EXPOSE 8080

# ENTRYPOINT define el proceso principal del contenedor.
# Ejecuta PracticaJWTcore.dll utilizando .NET.
# Sin esto: el contenedor se cerraría porque no tendría un proceso que ejecutar.
ENTRYPOINT ["dotnet", "PracticaJWTcore.dll"]

