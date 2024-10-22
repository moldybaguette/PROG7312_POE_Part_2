# Municipality Service App - Part 2 Submission for PROG7312
---
This is the **Part 2** submission for **PROG7312**. It is a municipal service app. This Submission added the Local Events Page

## Features:
### Web Scraped Local Events
See the EventScraper class for the implementation. 
### Real World Distance Info Using Google Maps Platform
See the EventDistanceAPI class for the implementation. 

(I was having trouble with it breaking things because the Venue information i scraped was not always enough to determine an address, so a lot of the error handling of that method was done by chat. )
### Highly Configurable Filtering and Sorting Options
See the FilterSortControl UserControl and the FilterSortLogic for the implementation details.
### Search Events by Title, Venue or Date
See the SearchLogic class for implementation.
### Personalised Recommendation Algorithm Based On User Activity
See the CalculatePersonalisedEventPriorities method in the FilterSortLogic class for the implementation.
## Running the Program

To run the program:

1. Open the `PROG7321_POE.sln` file in **Visual Studio**.
2. Once the project has loaded, click the green **Start** icon at the top to run the application.

> **Note**: The first time you run the application, it WILL load slower than on subsequent attempts.
