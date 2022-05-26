<?php

$playerID = $_POST["playerID"];
$dir = "Hats/" . $playerID;


if (!file_exists($dir)) {
    mkdir($dir, 0777, true);
}

$files = scandir($dir);
$fileCount = count($files)-2;

$hatSprite = $_FILES["hatSprite"];

move_uploaded_file($hatSprite['tmp_name'], $dir . "/" . $fileCount .".png");

?>