# CODING CHALLENGE #

* You have an existing storage of a large entries (> 1 million) of Zip Code entries (Database / Files system). For the exercise, choose the sample data from http://download.geonames.org/export/dump/EE.zip (ee.txt)

* Create a web application which will provide an incremental search on this.

* Example - When you type '4' all numbers under 4 appear, then you say "5"...all numbers under "45" appear; So on and so forth.

* Apply caching and pagination where applicable

* Make the data available to the front end using any technique (either a web api / mvc)

* The zip code input file can drastically increase in size

* Bonus Question: Make the search work on the Zip code AND location. For example, if i type numbers or if i type text, i should get the data. Consider only ASCII Characters. However, you get further credits if you build index based on Unicode