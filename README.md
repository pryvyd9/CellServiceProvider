# CellServiceProvider

## Description

Uni project
Variant 7
ASP.NET Core 2.2 MVC

## Overview
This application provides basic CRUD operations against a postgress database (any really). 
The UI presents a table and a drop-down list of available tables.
Each row of table represents a record from the db. To the right of it Delete and Update buttons are located.
At the end of the table there is an empty row shown for the purpose of inserting new rows
After applying any CRUD operation the page and the table are updated.

## Deployment
Docker is not required but recommended.
1. Start database. Dump file is located in a root of the solution and called provider.sql;
2. Edit appsettings.json to match DBConnection string;
3. Build the project and run it.



## Tutorial for creating the project

###### Environtment
- Windows
- Docker
- Visual Studio 2017/2019
- asp.net core 2.2 sdk [link](https://dotnet.microsoft.com/download/dotnet-core/2.2)
- NuGet package Npgsql (Postgress connector)

###### Docker
1. Pull Postgres image in docker [link](https://docs.docker.com/engine/reference/commandline/image_pull/)
2. Run image with params 
	`$ docker run -d -p PORT:5432 --name my-postgres -e POSTGRES_PASSWORD=PSWD postgres`
	[link](https://medium.com/@lvthillo/connect-from-local-machine-to-postgresql-docker-container-f785f00461a7)
3. Connect to db from host to configure it [link](https://medium.com/@lvthillo/connect-from-local-machine-to-postgresql-docker-container-f785f00461a7)
	- use PSWD to connect from host
	- use PORT to connect from host
4. Export db to push it on github 
	`pg_dump -U USERNAME -O DBNAME -f FILENAME.sql` [link](http://www.postgresqltutorial.com/postgresql-copy-database/)
	- Copy file to host
5. Import db
	- Copy file from host
	- First create db
	`psql -U USERNAME -d DBNAME -f FILENAME.sql` [link](http://www.postgresqltutorial.com/postgresql-copy-database/)

###### Visual Studio 
1. Create asp.net core project
	- asp.net core 2.2
	- Enable Docker integration
	- Mvc project
2. Add package Npgsql [tutorial](https://www.npgsql.org/doc/index.html)
3. Connect to Db 
	- If deployed on docker
		- Get ip of container with db
			`docker inspect CONTAINER`
			- search for `"IPAddress": "FOUND_IP",`
		- set connection string
			`Server=FOUND_IP;Port=CONFIGURED_PORT;Database=postgres;User Id=postgres;Password=PSWD;`
	- If deployed on host
		- set connection string
			`Server=127.0.0.1;Port=CONFIGURED_PORT;Database=postgres;User Id=postgres;Password=PSWD;`
	[connection string link](https://www.connectionstrings.com/postgresql/)

###### Other
- Connect to db via terminal `psql -h localhost -p 5432 -U USERNAME -W`[link](https://medium.com/@lvthillo/connect-from-local-machine-to-postgresql-docker-container-f785f00461a7)
- Copy file to container [link](https://stackoverflow.com/questions/22907231/copying-files-from-host-to-docker-container)
	- to container `docker cp foo.txt CONTAINER:/foo.txt` 
	- to host `docker cp CONTAINER:/foo.txt foo.txt` 
- Enable container bash `docker exec -it CONTAINER bash` [link](https://medium.com/@lvthillo/connect-from-local-machine-to-postgresql-docker-container-f785f00461a7)

## Database
- All operations go through DbContext
- new User(dbContext){ assing params }
- user.Commit()
- user.Delete()
- dbContext.SelectAll<T>()
- dbContext.DeleteAll<T>()

###### Attribute list
- [Table] - table name
- [Field] - field name
- [Key] - field name but used in key selection
- [Default] - if value was not assigned then default value from db is assigned
- [DefaultOverride] - if value was not assigned then value from attribute is used
- [Nullable] - having null as value is not considered error