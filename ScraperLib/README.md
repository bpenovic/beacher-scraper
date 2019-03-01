# Web scraper 
Web scraper is created as Azure Function, which collect data from web site (scrape it), parse it to model and store in database.  

## Azure functions  
For testing purposes, functions are implemented as HttpTriggered functions but for production will be used as Time triggered functions (every month).  

### Database  
Microsoft SQL Database is used for database storage.  
