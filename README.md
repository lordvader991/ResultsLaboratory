# CassandraJwtAuth

Este es el microservicio de autenticacion para los usuarios que serian nuestros doctores, los doctores incian sesion con correo y contrase.

## Database

Se tiene que crear una instancia de cassandra en docker con el nombre: cassandrajwtauth

```
docker run -d --name cassandrajwtauth -p 9042:9042 cassandra:latest
```

Luego se tiene que crear las siguientes tablas:

```
CREATE TABLE users_by_email (
    email text PRIMARY KEY,
    id int,
    passwordhash text
);
```

Por ultimo cargas los usuarios del sistemas:

```
INSERT INTO users_by_email (email, id, passwordhash)
VALUES ('daniel@email.com', 1, 'contraseñaSegura123');

INSERT INTO users_by_email (email, id, passwordhash)
VALUES ('maria@email.com', 2, 'claveFuerte456');

INSERT INTO users_by_email (email, id, passwordhash)
VALUES ('juan@email.com', 3, 'passSegura789');
```

## Start

Para correr el proyecto ejecuta el comando:

```
dotnet run
```

Asegurate de estar en la ruta raiz del proyecto.

## API

Login:
POST http://localhost:3000/api/auth/login

```
{
  "email": "daniel@email.com",
  "passwordHash": "contraseñaSegura123"
}
```
