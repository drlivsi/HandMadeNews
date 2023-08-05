<h1 align="center" id="title">HandMadeNews</h1>

<p align="center"><img src="/res/sheep.jpg" alt="project-image" height="200" ></p>

<p align="center" id="description">Needlework News Parser</p>

<h2>Description</h2>

<p>HandMadeNews is a web parser for embroidery manufacturers. As soon as the manufacturer releases something new, the service writes this information to the database and sends it to 2 telegram channels in different languages. The service sends exactly the same information to both channels, but it is always possible to manually add ads to the desired channel :)</p>

<p>It looks like this: </p>

- Periodically checking specific URLs
- Saving information about new products to the database
- Sending news to the Telegram channel

<h2>🚀 Demo</h2>

[https://t.me/handmade\_news\_ua](https://t.me/handmade_news_ua) <br>
[https://t.me/handmade\_news\_ru](https://t.me/handmade_news)

<h2>💻 Built with</h2>

Technologies used in the project:

*   Azure Functions
*   MySQL
*   .NET 6

<h2>Project Screenshots:</h2>

<p>List of Telegram channels:</p>
<img src="/res/channels-list.png" alt="project-screenshot" width="540" height="121" />

<p>An example of information sent to a telegram channel:</p>
<img src="/res/channels-content.jpg" alt="project-screenshot" height="700" />

<p>Channel subscribers can go to the manufacturer's website and buy an embroidery kit.</p>

<h2>🛠️ Run on local machine:</h2>

<h3>1. Clone or download repository</h3>

<h3>2. Create file src\HandmadeNews.AzureFunc\local.settings.json</h3>

```
 {
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet"
  },
  "ConnectionStrings": {
    "DefaultConnection": "server=db;uid=my_user;pwd=my_password;database=my_database",
    "MigrationsConnection": "server=127.0.0.1;uid=my_user;pwd=my_password;database=my_database"
  },
  "ProducersOptions": {
    "LanarteUrl": "https://webshop.verachtert.be/en-us/lan-arte/embroidery-kits/?sort=PfsSeason_desc&count=13&viewMode=list",
    "BucillaUrl": "https://plaidonline.com/products?category=Shop_Brand_Bucilla&sort=originalproductdate|desc&count=7",
    "KoolerDesignUrl": "https://www.koolerdesign.com/whatsnew"
  },
  "TelegramOptions": {
    "Enabled": false,
    "ApiKey": "<YOUR API KEY>",
    "ChatIdRu": -<First Channel Id>,
    "ChatIdUa": -<Second Channel Id>
  }
}
```
Note:
* Both connection strings differ only by the server (we need 127.0.0.1 for applying EF-migrations)
* Sending images to Telegram is disabled by default. To enable it, you need to specify the Telegram ApiKey and manually create 2 Telegram channels

<h3>3. Create file src\.env</h3>

```
MYSQL_ROOT_PASSWORD=my_root_password
MYSQL_DATABASE=my_database
MYSQL_USER=my_user
MYSQL_PASSWORD=my_password
```

You need this file only for running on a local machine using docker-compose

<h3>3. Run docker-compose </h3>  

```
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d
```
At the moment we have an empty database, in the next step we will create tables.

<h3>4. Run Entity Framework migration script</h3>

```
dotnet ef database update --project HandmadeNews.Infrastructure --startup-project Handmadenews.AzureFunc  
```

<h3>5. And the latest step - run Azure Function :)</h3>

```
http://localhost:34895/api/Scrap
```

As a result, the HandmadeNews parser will grab information from 3 websites and save it to the database. Information can be sent to Telegram channels.

<h2>🛠️ Run on Dev/Prod environments</h2>
<p></p>docker-compose and .env files are only needed for debugging on the local machine.</p>
<p>Azure Function is deploying using Azure DevOps, you can find the pipeline here https://github.com/drlivsi/HandMadeNews/blob/main/azure-pipelines.yml</p>
<p>On Azure Portal, you need to create all environment variables from local.settings.json and specify the correct database connection string (I use Hetzner Cloud, but you can create the database on Azure).</p>

<h2>🛠️ How to add a new website for parsing?</h2>

- Modify producer options in file src\HandmadeNews.AzureFunc\local.settings.json
- Add new Parsing Strategy https://github.com/drlivsi/HandMadeNews/tree/main/src/HandmadeNews.Infrastructure/Parsing/Strategies
  
<br>
<p align="center"><img src="/res/StandWithUkraine.jpg" /></p>
