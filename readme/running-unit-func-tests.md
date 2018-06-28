# Running UnitTests, Functional and Laod Tests on eshopOnContainers

## Unit and Functional tests for each microservice

En cada uno de los diferentes micro-servicios creados hay disponibles el conjunto de tests creados para validar su funcionamiento. Con el fin de mantener lo máximo posible la autonomía de cada micro-servicio los diferentes proyectos de tests están físicamente juntos con el resto de código. Así, por ejemplo, si desplegamos en nuestro explorador de soluciones el codigo del servicio de *Ordering* podrá ver los proyectos de *Ordering.FunctionalTests* y *Ordering.UnitTests*.

<p>
<img src="../img/unitandfunctests/orderingservicetests.PNG">
<p>

Para ejecutar los tests unitarios de los diferentes micro-servicios solamente tendremos que seleccionarlos con nuestro *TestView*, o su herramienta preferida, y ejecutarlos. Estos tests no tienen ninguna dependencia y por lo tanto no se necesita nada especial para que los mismos funcionen.

Al contrario, los tests funcionales si tienen dependencias con ciertos elementos de la infraestructura como por ejemplo, la base de datos de Sql Server, el sistema de colas etc. 

<p>
<img src="../img/unitandfunctests/functionaltestsview.PNG">
<p>

Por ello, para correr estos tests es necesario tener disponible ciertos elementos de infraestructura. Con el fin de facilitar esta construcción en la carpeta de tests se dispone de un compose que nos permitirá crear estos elementos.

> docker-compose -f .\docker-compose-tests.yml -f .\docker-compose-tests.override.yml up

Cada proyecto de test funcionales usa un TestServer configurado con la infraestructura levantada en el compose anterior que nos permitirá correr sin ningun problema los diferentes tests.

> Puede obtener más información sobre *TestServer* en [este enlace](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-2.1).


## Integration Tests

Hasta aquí hemos hablado sobre como cada micro-servicio contiene los tests, unitarios y funcionales, que le ocupan, no obstante también son necesarios diferentes tests de integración para verificar el trabajo entre los diferentes elementos. Estos proyectos de pruebas están situados en un lugar común, la carpeta tests y contiene las diferentes pruebas de integración del sistema.

<p>
<img src="../img/unitandfunctests/integrationtestsview.PNG">
<p>

Para poder ejecutarlos, al igual que antes es necesario disponer de la infraestructura necesaria con lo que podemos utilizar el *compose* mencionado anteriormente.

## LoadTests project

El trabajo con los tests de carga está descrito en [este fichero de documentación](../test/ServicesTests/LoadTest/readme.md) que puede leer.