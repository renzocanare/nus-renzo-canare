<h1 align="center">📄 EE4211 - Data Science for the Internet of Things 📄</h1>

Code for all parts of the project can be found in *FinalProject.ipynb*, which you can preview by clicking the file in GitHub or <a href="https://nbviewer.org/github/renzocanare/nus-renzocanare/blob/main/EE4211/FinalProject.ipynb" target="_blank" rel="noreferrer noopener">here</a>.

## 🚀 Summary 
<img src="./ee4211-banner.gif" alt="p1screen"/>

Working as a team of 4, we explored how various Machine Learning models could be applied to real world datasets.

Our final project was aimed at helping users to identify which public carparks near Hawker Centres were likely to be full at a certain hour during the day. This involved collecting about 5 months of carpark data through API calls and using it to train a Machine Learning model that could predict parking availability.

As a proof of concept, we visualized the predictions using a heatmap for a particular set of carparks that are within 200 metres of a hawker centre (located in the Northern region of Singapore).


## :flashlight: Project Analysis
> The following was taken from *FinalProject.ipynb*.

In our project, we are examining car park spaces in relation to the time of day and their proximity to popular hawker centers. We start by analyzing a dataset from data.gov.sg, which shows the availability of car parks. This data reveals a general trend: early in the morning, around 5 AM, the availability of car parks is at about 45%. As the day progresses, this number slowly increases in a pattern that looks like a U-shaped curve. By 4 PM, the availability usually reaches its peak at 65%, and then it starts decreasing again, following a similar curve, until it goes back to 45% at around 4 AM the next day.

Since we cannot know for sure how car park availability changes every minute of the day, we use Machine Learning as a tool to predict these changes. Machine Learning helps us estimate the availability based on the hour, using models designed for predicting numbers, like Decision Trees, Support Vector Machines, Random Forests, and more advanced techniques like Ensembling and Adaptive Boosting (AdaBoostRegressor).

To add another layer to our analysis, we cross-reference the car park codes with the DataMall API to find their exact locations. We then gather data on hawker centers from a GEOJSON file on data.gov.sg and pinpoint their locations too.

Our focus then shifts to identifying car parks that are within 200 meters of these hawker centers. We create a new dataset, 'hawker_nearby.json', which links car parks to nearby hawker centers. For example, we found that the Bukit Merah Central Food Centre has several car parks in its vicinity, each with its own ID, address, and coordinates.

~~~
"Bukit Merah Central Blk 163 (Bukit Merah Central Food Centre)": [
        {
            "carpark_id": "HE12",
            "carpark_add": "BLK 78/81 REDHILL LANE",
            "carpark_coord": "1.2882100231309097 103.81865062406243"
        },
        {
            "carpark_id": "RHM",
            "carpark_add": "BLK 88A REDHILL CLOSE",
            "carpark_coord": "1.2864639274816887 103.81857760519375"
        },
        {
            "carpark_id": "BM29",
            "carpark_add": "BLK 163 BUKIT MERAH CENTRAL",
            "carpark_coord": "1.283634706091155 103.81709921259974"
        }]
~~~

By combining these datasets, we can understand how car park availability correlates with the proximity to hawker centers, particularly during meal times. This analysis is valuable in the context of a data science project because it provides insights into urban planning and public convenience. It helps to answer questions like: How does the demand for car parks change throughout the day? Are there enough parking spaces near popular dining areas? This understanding could guide city planners in improving parking solutions and managing traffic better, especially during peak dining hours.

In conclusion, our project expands beyond the initial proposal by integrating different datasets and applying Machine Learning to draw meaningful conclusions about car park availability in relation to hawker centers. This approach demonstrates the power of data science in solving real-world problems and improving everyday life.