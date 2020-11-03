<?php
  $db = mysql_connect('localhost', 'capstoneUser', 'nSKjbUJvaSSqjysz') or die('Could not connect: ' . mysql_error());
  mysql_select_db('qa_poc') or die('Could not select database');

  $questionQuery = 'SELECT * from question';
  $questionResult = mysql_query($questionQuery) or die('Query failed: ' . mysql_error());
  $questionResultCount = mysql_num_rows($questionResult);

  for($i = 0; $i < $questionResultCount; $i++)
  {
       $row = mysql_fetch_array($questionResult);

       $question->questionId = $row['questionId'];
       $question->questionContent = $row['questionContent'];
       $question->correctAnswerId = $row['correctAnswerId'];

       $answerQuery = 'select * from answer where answer.questionId = ' . $row['questionId'];
       $answerResult = mysql_query($answerQuery) or die('Query failed: ' . mysql_error());
       $answerResultCount = mysql_num_rows($answerResult);

       for($j = 0; $j < $answerResultCount; $j++)
       {
          $answerRow = mysql_fetch_array($answerResult);
          $question->answers[] =  $answerRow['answerId'] . '.)' . $answerRow['answerContent'];
       }
       $questions[] = $question;
  }

  $jsonQuestions = json_encode($questions);

  echo $jsonQuestions;
?>
