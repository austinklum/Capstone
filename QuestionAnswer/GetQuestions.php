<?php
  $db = mysqli_connect('localhost', 'capstoneUser', 'nSKjbUJvaSSqjysz') or die('Could not connect: ' . mysql_error());
  mysqli_select_db($db, 'qa_poc') or die('Could not select database');

  $questionQuery = 'SELECT * from question';
  $questionResult = mysqli_query($db, $questionQuery) or die('Query failed: ' . mysql_error());
  $questionResultCount = mysqli_num_rows($questionResult);

  for($i = 0; $i < $questionResultCount; $i++)
  {
       $row = mysqli_fetch_array($questionResult);

       $question = new stdClass();
       $question->questionId = $row['questionId'];
       $question->questionContent = $row['questionContent'];
       $question->correctAnswerId = $row['correctAnswerId'];
       $question->locationId = $row['locationId'];

       $answerQuery = 'SELECT * from answer where answer.questionId = ' . $row['questionId'];
       $answerResult = mysqli_query($db, $answerQuery) or die('Query failed: ' . mysql_error());
       $answerResultCount = mysqli_num_rows($answerResult);

       for($j = 0; $j < $answerResultCount; $j++)
       {
          $answerRow = mysqli_fetch_array($answerResult);
          $question->answers[] =  $answerRow['answerId'] . '.)' . $answerRow['answerContent'];
       }
       $questions[] = $question;
  }

  $jsonQuestions = json_encode($questions);

  echo $jsonQuestions;
?>
