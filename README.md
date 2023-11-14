# Varausjärjestelmä C#

Tämä C#:lla toteutettu varausjärjestelmä tarjoaa API-rajapinnan, jonka avulla voit hallinnoida varauksia, kohteita ja käyttäjiä. Järjestelmä koostuu kolmesta pääkomponentista: `ItemsController`, `ReservationsController` ja `UsersController`, sekä kolmesta repositorysta: `ItemRepository`, `ReservationRepository` ja `UserRepository`.

## Controllers

### ItemsController

`ItemsController` vastaa kohteisiin liittyvistä HTTP-pyyynnöistä, kuten haku, päivitys, luonti ja poisto. Se toimii välittäjänä käyttäjän ja tietokantaa käsittelevän `ItemRepositoryn` välillä.

### ReservationsController

`ReservationsController` huolehtii varauksiin liittyvistä toiminnoista, kuten varausten hakeminen, luonti ja poisto. Se käyttää tietokantaa käsittelevää `ReservationRepositorya`.

### UsersController

`UsersController` hallinnoi käyttäjiin liittyviä toimintoja, kuten käyttäjien hakeminen ja poisto. Sen toimintoihin vaikuttaa `UserRepository`.

## Repositories

### ItemRepository

`ItemRepository` käsittelee tietokantaoperaatioita, jotka liittyvät kohteisiin. Se vastaa kohteiden lisäämisestä, päivittämisestä ja poistamisesta tietokannasta.

### ReservationRepository

`ReservationRepository` vastaa varauksiin liittyvien tietokantaoperaatioiden suorittamisesta. Se käsittelee varausten lisäämistä, päivittämistä ja poistamista.

### UserRepository

`UserRepository` huolehtii käyttäjiin liittyvistä tietokantaoperaatioista. Se sisältää toiminnallisuuden käyttäjien lisäämiseen, päivittämiseen ja poistamiseen.

Näitä repositoryja käytetään yhdessä ohjaimien kanssa varmistamaan, että tietokantaan tallennetut tiedot ovat ajan tasalla ja vastaavat järjestelmän liiketoiminnallisia tarpeita. Repositoryt on eriytetty ohjaimista pitämään sovellusarkkitehtuuri selkeänä ja ylläpidettävänä.


