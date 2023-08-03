<h1 align="center" id="title">-- HandMadeNews --</h1>

<p align="center"><img src="https://forumsmile.net/u/f/8/7/f87c2aeb7c529b31fda475bc6b3bfa63.jpg" alt="project-image"></p>

<p align="center" id="description">Needlework News Parser for Telegram Channels</p>

<h2>🚀 Demo</h2>

[https://t.me/handmade\_news\_ua](https://t.me/handmade_news_ua)

<h2>Project Screenshots:</h2>

<img src="https://s3.moifotki.org/5c806a3751724151a0f17d525a11b20b.png" alt="project-screenshot" width="540" height="121/">

<h2>🛠️ Installation Steps:</h2>

<p>1. Create a MySQL database. You can set it up locally, use Azure, Hetzner Cloud, or any suitable hosting provider. I use Hetzner in production because I already have a server there that I use for other projects.</p>

<p>2. Get Telegram ApiKey, create 2 telegram channels, and get ID for each channel</p>

<p>3. Clone or download repository</p>

<p>4. Create file local.settings.json with required parameters (existing MySQL database Telegram ApiKey and Id's of telegram channels</p>

```
 {
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet"
  },
  "ConnectionStrings": {
    "DefaultConnection": "*****"
  },
  "ProducersOptions": {
    "LanarteUrl": "https://webshop.verachtert.be/en-us/lan-arte/embroidery-kits/?sort=PfsSeason_desc&count=13&viewMode=list",
    "BucillaUrl": "https://plaidonline.com/products?category=Shop_Brand_Bucilla&sort=originalproductdate|desc&count=7",
    "KoolerDesignUrl": "https://www.koolerdesign.com/whatsnew"
  },
  "TelegramOptions": {
    "ApiKey": "*****",
    "ChatIdRu": -****,
    "ChatIdUa": -****
  }
}
```

<p>5. Run from Visual Studio by F5</p>  
  
<h2>💻 Built with</h2>

Technologies used in the project:

*   Azure Functions
*   MySQL
*   .NET 6
