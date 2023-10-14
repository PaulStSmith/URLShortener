# URL Shortener

This was created as a reply to a challenge issued by Mottu (https://mottu.com.br/).

Here's the original challenge (in Portuguese):

<hr/>

# Desafio Backend - Encurtador de URL

## _se divirta!

Imagine um cenário de uma startup que precisa criar um novo serviço para resolver um problema da Mottu. Por aqui, trabalhamos com aplicações em REST API que disponibilizam informações para outros sistemas. Você será avaliado em vários conceitos importantes de desenvolvimento.

## Definição do projeto

* O desafio proposto consiste no desenvolvimento de um encurtador de URL que deve ser implementado como back-end juntamente com um validador capaz de indicar se o link gerado está funcionando. Esse desenvolvimento é baseado em API`s REST.

* É esperado, através da implementação, que seja possível além de encurtar a URL e ao acessá-la, o número de acessos incremente-se para contagem de acessos.

* Para cada criação de uma URL será necessário disparar um evento que futuramente poderá ser integrado em algum sistema de mensageria, inicialmente você pode disparar esse evento para um cache local ou para alguma fila, se a URL já existir não há necessidade de disparo no evento.

* Todas as informações devem ser salvas em um banco de dados PostGres e desenvolvido em C#

## Entrega

Toda entrega será realizada através de uma PR para um GIT da Mottu ou envio de um link do git pessoal contendo que deve estar público para facilitar o acesso

Critérios de aceite

Alguns critérios de aceitação serão validados para conclusão do projeto.

* [Fluxo] - Execução completa de um fluxo considerando as API’s de cadastro e consulta de URL’s encurtadas

* [Código] - O código compartilhado deverá funcionar (rodar) e representar os itens listados

* [Docker] imagens devem estar acessíveis para validação.

* Garanta um README para rodar seu projeto, importante que ele funcione ao ser executado

* Não temos um prazo estipulado para a solução desse desafio, mas acreditamos que você não deveria gastar mais do que 10 horas nele. É importante que você alinhe o prazo de entrega com seu contato na Mottu.

* **SE NOTARMOS QUE O CÓDIGO FOI COPIADO E COLADO, SERÁ DESQUALIFICADO AUTOMATICAMENTE.**

## Diferenciais

* Boa documentação (como rodar o projeto e outras observações)

* Estrutura do código, facilidade de leitura, boas práticas de desenvolvimento

* Consumir o JSON urls.json no back-end para a seção TOP 5 como o estado inicial da aplicação;

* Criar outros endpoints importantes para o encurtador que você julgar necessário

**Boa sorte! =]**

## Anexos

## urls.json
```
[
  { 
   "id": "23094", 
   "hits": 5, 
   "url": "http://globo.com", 
   "shortUrl": "http://chr.dc/9dtr4" 
  }, 
  { 
    "id": "76291", 
    "hits": 4, 
    "url": "http://google.com", 
    "shortUrl": 
    "http://chr.dc/aUx71" 
  }, 
  {
    "id": "66761", 
    "hits": 7, 
    "url": 
    "http://terra.com.br", 
    "shortUrl": "http://chr.dc/u9jh3" 
  }, 
  { 
    "id": "70001", 
    "hits": 1, 
    "url": "http://facebook.com", 
    "shortUrl": "http://chr.dc/qy61p" 
  }, 
  { 
    "id": "21220", 
    "hits": 2, 
    "url": "http://diariocatarinense.com.br", 
    "shortUrl": "http://chr.dc/87itr" 
  }, 
  { 
    "id": "10743", 
    "hits": 0, 
    "url": 
    "http://uol.com.br", 
    "shortUrl": "http://chr.dc/y81xc" 
  }, 
  { 
    "id": "19122", 
    "hits": 2, 
    "url": "http://chaordic.com.br", 
    "shortUrl": "http://chr.dc/qy5k9" }, 
  { 
    "id": "55324", 
    "hits": 4, 
    "url": "http://youtube.com", 
    "shortUrl": "http://chr.dc/1w5tg" 
  }, 
  { 
    "id": "70931", 
    "hits": 5, 
    "url": "http://twitter.com", 
    "shortUrl": "http://chr.dc/7tmv1" 
  }, 
  { 
    "id": "87112", 
    "hits": 2, 
    "url": "http://bing.com", 
    "shortUrl": "http://chr.dc/9opw2" 
  }
]
```

<hr/>

# The development environment

This URL shortener was developed using _Visual Studio 2022 Community Edition_, configured to run in _Docker Desktop_ (configured as Linux).

The _Docker_ configuration is the default for a WebAPI project. No change was made to the ```Dockerfile```.

# Preparing the database

In the folder named ```Data``` there are four files:

* ```Database Schema.dbml```
* ```ShortURLs.MySQL.sql```
* ```ShortURLs.PostgreSQL.sql```
* ```ShortURLs.SQL Server.sql```

The first file is writen in the Database Markup Language, used by dbdiagrams.io, which describes a database schema without biding it to a single DBMS.

The next three files describe the same database schema for different DBMS.

Choose the appropriated one for your DBMS (or create a new one using the DBML file) and execute in your favorite SQL management program.

# Configuring the Project for execution

## Configuring the Connection String

A **Connection String** is a text containing parameters and values that specifies how the project will connect to the database, including details like the server address, authentication credentials, and database name.

You'll need to inform, that is configure, a Connection String for the project to work correctly.

To do this you’ll have to edit a file named ```appsettings.json```, located at the project root folder. This file contains all the configuration for the project to run.

Open this file in your favorite simple text editor (I recommend _Notepad++_ &#x27A8; https://notepad-plus-plus.org), locate the section named ```ConnectionStrings```, and within it locate the name-value pair named ```db```.

Once you find this, change its value to the appropriated Connection String for your database.

## Configuring the NHibernate driver and dialect

In this project I used _NHibernate_ (https://nhibernate.info/) for database connection. It's a personal preference due the fact that I've been using it for a long time now.

In the ```appsettings.json``` file, locate the section named ```Hibernate```, and within it there are two values that need to be changed, according to the following table:

| DBMS       | Driver                            | Dialect                              |
|------------|-----------------------------------|--------------------------------------|
| MySQL      | NHibernate.Driver.MySqlDataDriver | NHibernate.Dialect.MySQLDialect      |
| PostgreSQL | NHibernate.Driver.NpgsqlDriver    | NHibernate.Dialect.PostgreSQLDialect |
| SQL Server | NHibernate.Driver.SqlClientDriver | NHibernate.Dialect.MsSql2012Dialect  |

Choose the correct one for your DBMS, and change the values accordingly.

## Configuring NHibernate mapping

The file ```NHibernate.map.xml``` contains the mapping configuration for a PostgreSQL database.

If you're using a different database, you will need to change the ```generator``` element accordingly.

**MySQL and SQL Server**
```
<generator class="identity"/>
```

**PostgreSQL**
```
<generator class="sequence">
    <param name="sequence">&quot;ShortUrls_id_seq&quot;</param>
</generator>
```

Further reference can be found [here][A1].

  [A1]: https://nhibernate.info/previous-doc/v5.3/single/index.html#mapping-declaration-id-sequences

# Setting up the ```urls.json``` file

The ```urls.json``` file, which will be used to configure the initial state of the database, if used, MUST be placed in the ```Data``` folder.

At the beginning of the execution, if this file is located at the proper folder, it will be consumed and all valid urls will be added to the database, if possible.

Any exception or impossibility of adding any URL to the database will be logged to the file ```urls_json.log```, in the same directory.

After consuming the JSON file, it will be renamed to ```urls.json.old``` with a sequential number if the ```.old``` file already exists.

# Executing the URL Shortener

This project depends on the _Docker Desktop_, that needs to be installed in the machine that will execute this project.

Once the project has been loaded in the _Visual Studio_, simply start the execution, _Visual Studio_ will take care of the _Docker_ installation, configuration, and execution.

Afterwards your default browser will open with an instance of ```Swagger``` that allows you to try executing each one of the endpoints.

Calling the ```Get `/{shortUrl}` ``` from the ```Swagger``` with or without a valid short URL will result in an error, as this call assumes a user is calling from a browser.
In a browser that call will redirect the user to the proper URL, and add the hits counter for that short URL.

All other end points call be executed from ```Swagger```.

# The API

|             | Verb | Entry Point | Description                |
|-------------|------|-------------|----------------------------|
|[![][i1]][I1]| GET  | /{shortUrl} | Redirects to the long URL. |
|[![][i1]][I1]| POST | /Add        | Adds a long URL to the repository \
                                     Parameters:
                                        ```url``` : _string_ : Any Uniform Resource Locator \
                                     \
                                     Returns:
                                        ```ShortUrlDTO``` : _JSON_ : A Data Transfer Object that represents the added URL |


# Known problems

I could not make the NHibernate connect to my ```SQL Server Express``` instnace from inside the ```Docker``` (see my question at [StackOverflow][A2]).

  [A2]: https://stackoverflow.com/questions/77288308/
  [I1]: https://paulstsmith.github.io/images/class/method.gif