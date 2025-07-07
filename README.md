### Variables de entorno.

Para ejecutar localmente, cree el archivo `./code/.env` y asignele los datos de
[el archivo de ejemplo](./code/.env.example).

Para ejecutar en la base de datos dada, cree el archivo `./code/.env` y asignele
los datos de [el archivo ejemplo](./code/.env.example.ucu).

### Dependencias

- docker
- docker compose

### Cómo ejecutar

1. Ir a la carpeta de código:

```bash
cd code
```

2. Levantar el entorno de docker:

```bash
docker compose down -v && docker compose up --build # Un unix
docker compose down -v; docker compose up --build # Un powershell
```

3. Levantar el frontend:

En otra terminal, ir a [el directorio frontend](./code/frontend/).

```bash
npm i && npm run dev # En unix
npm i; npm run dev # En powershell
```

### Cómo utilizar

Para verificar que su entorno funcione correctamente, puede usar el
[script de testing](./code/endpoint_testing/test.sh). Es posible que falle
si su computadora es lenta, ya que se usan timeouts hardcodeados.

### Enlaces útiles

- [Documentación del backend](http://localhost:8080/swagger/index.html).
- [Frontend](http://localhost:5173)

### Creación de datos

Para crear datos y probar, ejecutar la ruta `/debug/fake/init` en
[la interfaz de swagger](http://localhost:8080/swagger/index.html) una vez el
backend haya iniciado.

La ruta le retornará datos importantes para iniciar sesión y poder votar. Puede
que le sea útil guardar esta información para utilizar durante sus tests.
Particularmente, deberá iniciar sesión con el presidente de mesa de la primera
tabla. (Haga búsqueda textual de `president`)

Por último, la contrasña de todos los usuarios falsos es `pato1234`.
