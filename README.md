# Web scraper 
Web scraper is created as Azure Function, which collect data from web site (scrape it), parse it to model and store in database. In this project we will scrape data about beaches in Croatia.  

## Azure functions  
For testing purposes, functions are implemented as HttpTriggered functions but for production will be used as Time triggered functions (every month).  

### Database  
Microsoft SQL Database is used for database storage with ORM (objet-orijented mapping) principle (Entity Framework, Code - first).  

## ScraperFunction
Main project which contains:
 * Containers - container builders
 * Functions - Azure functions
   * GetMarkers - get all markers and store it to DB  
   * GetQuality - get quality marker / markers and store it to DB  
   * GetDetails - get details of marker (type, vegetation, wind...)  
 * Modules - module entities for dependency injection  
 * json settings  

## ScraperLib
All Database manipulation is implemented inside ScraperLib. Servicelib contains:
  * DAL - Database access layer 
  * Models - Database models 
  * DomainServices - services which manipulate with database (using Entity Framework)  
    * Interfaces - interfaces of domain services 
  * DomainModels - models for domain services
    * ParseModels - models in process of scraping, for parsing Xml
