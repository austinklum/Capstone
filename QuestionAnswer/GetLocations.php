<?php
  $db = mysqli_connect('localhost', 'capstoneUser', 'nSKjbUJvaSSqjysz') or die('Could not connect: ' . mysql_error());
  mysqli_select_db($db, 'qa_poc') or die('Could not select database');

  $locationQuery = 'SELECT * from location';
  $locationResult = mysqli_query($db, $locationQuery) or die('Query failed: ' . mysql_error());
  $locationResultCount = mysqli_num_rows($locationResult);

  for($i = 0; $i < $locationResultCount; $i++)
  {
       $row = mysqli_fetch_array($locationResult);

       $location = new stdClass();
       $location->locationId = $row['locationId'];
       $location->name = $row['name'];
       $location->url = $row['url'];

       $locations[] = $location;
  }

  $jsonLocations = json_encode($locations);

  echo $jsonLocations;
?>
