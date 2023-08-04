<h1 align="center" id="title">-- HandMadeNews --</h1>

<p align="center"><img src="https://forumsmile.net/u/f/8/7/f87c2aeb7c529b31fda475bc6b3bfa63.jpg" alt="project-image" height="200" ></p>

<p align="center" id="description">Needlework News Parser</p>

<h2>Description</h2>

<p>I have a lot of pet-projects, with varying degrees of success/profitability. All of them are in private repositories. I do not develop this project further, but it still works. So I decided to make the repository public.</p> 

<p>The project HandMadeNews is a web parser for embroidery manufacturers. As soon as the manufacturer releases something new, my service writes it to the database and sends it to 2 telegram channels (in Ukrainian and Russian). The service sends exactly the same information to both channels, but it is always possible to manually add ads to the desired channel.</p> 

<h2>🚀 Demo</h2>

[https://t.me/handmade\_news\_ua](https://t.me/handmade_news_ua) and [https://t.me/handmade\_news\_ru](https://t.me/handmade_news)

<h2>💻 Built with</h2>

Technologies used in the project:

*   Azure Functions
*   MySQL
*   .NET 6

<h2>Project Screenshots:</h2>

<img src="https://s3.moifotki.org/5c806a3751724151a0f17d525a11b20b.png" alt="project-screenshot" width="540" height="121" />

<h2>🛠️ How to run on local machine:</h2>

<p>1. Clone or download repository</p>

<p>2. Create file src\HandmadeNews.AzureFunc\local.settings.json</p>

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
    "ApiKey": "*****",
    "ChatIdRu": -****,
    "ChatIdUa": -****
  }
}
```
Note:
* Both connection strings are the same, they differ only by the server
* Sending images to Telegram is disabled by default. To enable it, you need to specify the Telegram ApiKey and create 2 Telegram groups


<p>3. Create file src\.env</p>

```
MYSQL_ROOT_PASSWORD=my_root_password
MYSQL_DATABASE=my_database
MYSQL_USER=my_user
MYSQL_PASSWORD=my_password
```

Note:
* You need this file only for running on a local machine

<p>3. Run docker-compose </p>  

```
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d
```

<p>4. Run Entity Framework migration script</p>

```
dotnet ef database update --project HandmadeNews.Infrastructure --startup-project Handmadenews.AzureFunc  
```

<p>5. And the latest step - run Azure Function :)</p>

```
http://localhost:34895/api/Scrap
```

As a result, our parser will grab information from 3 websites and save it to the database. Optionally, images will be sent to Telegram channels.

<h2>🛠️ How to run on Dev/Prod environments</h2>
<p></p>As I mentioned before, docker-compose and .env file are only needed for debugging on the local machine.</p>
<p>Azure Function is deploying using Azure DevOps, you can find the pipeline here https://github.com/drlivsi/HandMadeNews/blob/main/azure-pipelines.yml</p>
<p>On Azure Portal, you need to create all environment variables from local.settings.json and specify the correct database connection string (I use Hetzner Cloud, but you can create the database on Azure).</p>


