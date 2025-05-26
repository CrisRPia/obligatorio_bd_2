### Aclaraciones

Para poder utilizar sintaxis monoespaciada de markdown, las claves primarias se
marcan con PK, y las foráneas con FK.

### Diseño Lógico

Decisión de Diseño: Por temas de seguridad, asumimos que el gobierno envía sus
contraseñas a cada ciudadano por un canal privado.

```
ciudadano(
    id_ciudadano PK,
    CI,
    CC,
    nombre,
    apellido,
    fecha_nacimiento,
    contraseña
)
```
```
policía(id_ciudadano PK FK)
```

Decisión de Diseño: Creamos esta tabla para que sea más
extensible el sistema, en caso de que en un futuro se necesitará guardar más
información de los policías o añadir nuevas relaciones.

```
comisaria(
    id_comisaría PK,
    id_zona FK,
    nombre,
    dirección
)
```
```
policía_asignado_comisaría(
    id_policia PK FK,
    id_comisaría PK FK,
    fecha_asignado PK,
    fecha_baja
)
```
```
establecimiento(
    id_establecimiento PK,
    nombre,
    dirección,
    id_zona FK
)
```
```
policía_asignado_establecimiento(
id_policia PK FK,
id_establecimiento PK FK,
fecha_asignado PK FK
)
```
```
zona(
    id_zona PK,
    nombre,
    codigo_postal,
    id_localidad FK
)
```
```
localidad(
    id_localidad PK,
    nombre,
    tipo,
    id_departamento FK
)
```
```
departamento(
    id_departamento PK,
    nombre
)
```
```
presidente(
    id_ciudadano PK FK,
    organismo
)
```
```
secretario(
    id_ciudadano PK FK,
    organismo
)
```
```
vocal(
    id_ciudadano PK FK,
    organismo
)
```
```
eleccion(
    id_eleccion PK,
    descripcion,
    fecha,
    esta_abierta
)
```
```
circuito(
    numero_circuito PK,
    esta_abierto,
    id_establecimiento FK
)
```
```
eleccion_tiene_circuito(
    id_eleccion PK FK,
    numero_circuito PK FK
)
```
```
circuito_eleccion_tiene_mesa(
    id_presidente PK FK,
    numero_circuito PK FK,
    id_eleccion PK FK,
    id_secretario FK,
    id_vocal FK
)
```
```
ciudadano_vota_circuito_eleccion(
    id_ciudadano PK FK,
    id_eleccion PK FK,
    numero_circuito FK
)
```
```
ciudadano_asignado_circuito_eleccion(
    id_ciudadano PK FK,
    id_eleccion PK FK,
    numero_circuito FK
)
```
```
voto(
    id_voto PK,
    estado
)
```
```
papeleta(id_papeleta PK)
```
```
voto_tiene_papeleta(
    id_voto PK FK,
    id_papeleta PK FK
)
```
```
booleana(
    id_papeleta PK FK,
    valor
)
```
```
lista(
    id_papeleta PK FK,
    numero_lista
)
```
```
candidato(
    id_ciudadano PK FK
)
```
```
lista_tiene_candidato(
    id_lista PK FK,
    id_candidato PK FK,
    indice,
    organo
)
```
```
partido(
    id_partido PK,
    direccion_sede
)
```
```
partido_tiene_ciudadano(
    id_ciudadano PK FK,
    id_partido PK FK,
    fecha_ingreso PK,
    rol,
    fecha_salida
)
```
```
lista_pertenece_departamento(
    id_lista PK FK,
    id_departamento FK
)
```
```
plebiscito(id_eleccion PK FK)
```
```
referendum(id_eleccion PK FK)
```
```
presidencial(id_eleccion PK FK)
```
```
municipal(
    id_eleccion PK FK,
    id_localidad FK
)
```
```
ballotage(id_eleccion PK FK)
```

## Foreign Keys

- `policía.CI_ciudadano` → `ciudadano.CI`
- `policía_asignado_comisaría.CI_policía` → `policía.CI_ciudadano`
- `policía_asignado_comisaría.ID_comisaría` → `comisaría.ID_comisaría`
- `policía_asignado_establecimiento.CI_policía` → `policía.CI_ciudadano`
- `policía_asignado_establecimiento.ID_establecimiento` → `establecimiento.ID_establecimiento`
- `establecimiento.ID_zona` → `zona.ID_zona`
- `zona.ID_localidad` → `localidad.ID_localidad`
- `localidad.ID_departamento` → `departamento.ID_departamento`
- `presidente.ID_ciudadano` → `ciudadano.ID_ciudadano`
- `secretario.ID_ciudadano` → `ciudadano.ID_ciudadano`
- `vocal.ID_ciudadano` → `ciudadano.ID_ciudadano`
- `circuito.ID_establecimiento` → `establecimiento.ID_establecimiento`
- `eleccion_tiene_circuito.ID_eleccion` → `eleccion.ID_eleccion`
- `eleccion_tiene_circuito.numero_circuito` → `circuito.numero_circuito`
- `circuito_eleccion_tiene_mesa.ID_presidente` → `presidente.ID_ciudadano`
- `circuito_eleccion_tiene_mesa.numero_circuito` → `circuito.numero_circuito`
- `circuito_eleccion_tiene_mesa.ID_eleccion` → `eleccion.ID_eleccion`
- `circuito_eleccion_tiene_mesa.ID_secretario` → `secretario.ID_ciudadano`
- `circuito_eleccion_tiene_mesa.ID_vocal` → `vocal.ID_ciudadano`
- `ciudadano_vota_circuito_eleccion.ID_ciudadano` → `ciudadano.ID_ciudadano`
- `ciudadano_vota_circuito_eleccion.ID_eleccion` → `eleccion.ID_eleccion`
- `ciudadano_vota_circuito_eleccion.numero_circuito` → `circuito.numero_circuito`
- `ciudadano_asignado_circuito_eleccion.ID_ciudadano` → `ciudadano.ID_ciudadano`
- `ciudadano_asignado_circuito_eleccion.ID_eleccion` → `eleccion.ID_eleccion`
- `ciudadano_asignado_circuito_eleccion.numero_circuito` → `circuito.numero_circuito`
- `voto_tiene_papeleta.ID_voto` → `voto.ID_voto`
- `voto_tiene_papeleta.ID_papeleta` → `papeleta.ID_papeleta`
- `booleana.ID_papeleta` → `papeleta.ID_papeleta`
- `lista.ID_papeleta` → `papeleta.ID_papeleta`
- `candidato.ID_ciudadano` → `ciudadano.ID_ciudadano`
- `lista_tiene_candidato.ID_lista` → `lista.ID_papeleta`
- `lista_tiene_candidato.ID_candidato` → `candidato.ID_ciudadano`
- `partido_tiene_ciudadano.ID_ciudadano` → `ciudadano.ID_ciudadano`
- `partido_tiene_ciudadano.ID_partido` → `partido.ID_partido`
- `lista_pertenece_departamento.ID_lista` → `lista.ID_papeleta`
- `lista_pertenece_departamento.ID_departamento` → `departamento.ID_departamento`
- `plebiscito.ID_eleccion` → `eleccion.ID_eleccion`
- `referendum.ID_eleccion` → `eleccion.ID_eleccion`
- `presidencial.ID_eleccion` → `eleccion.ID_eleccion`
- `municipal.ID_eleccion` → `eleccion.ID_eleccion`
- `municipal.ID_localidad` → `localidad.ID_localidad`
- `ballotage.ID_eleccion` → `eleccion.ID_eleccion`
