WebApi => ASP.NET Core Web API

BusinessLogic => Class library

Core => Class library

Dependencia de proyetos

WebApi => BusinessLogic => Core


Instalar dotnet-ef
Descargar de https://www.nuget.org/

Obtener versión del SDK de NET.CORE
  dotnet --list-runtimes

Buscar versión igual a la del SDK de NET.CORE instalado localmente
dotnet tool install --global dotnet-ef --version 5.0.11


Crar archivo de migración
	1. Migración inicial
		dotnet ef migrations add MigracionInicial -p BusinessLogic -s WebApi -o Data/Migrations
	2. Crear archivo de migración para la base de datos de seguridad
		dotnet ef migrations add SeguridadInicial -p BusinessLogic -s WebApi -o Identity/Migrations -c SeguridadDbContext
	3. Agregar imagen a la tabla usuario
		dotnet ef migrations add SeguridadImagn -p BusinessLogic -s WebApi -o Identity/Migrations -c SeguridadDbContext
		ejecutar archivo de migracion
			cd .\WebApi\
			dotnet watch run
	4. Migración de ordenes de compra
		dotnet ef migrations add OrdenDeCompra -p BusinessLogic -s WebApi -o Data/Migrations -c MarketDbContext
