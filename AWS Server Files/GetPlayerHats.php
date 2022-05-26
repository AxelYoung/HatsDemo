<?php

$playerID = $_POST["playerID"];
$dir = "Hats/" . $playerID;
$premadeDir = "Hats/Premade";

$premadeImages = glob($premadeDir . "/*.png");
foreach($premadeImages as $image)
{
    echo $image;
    echo "-";
}

if (file_exists($dir)) {
    $images = glob($dir . "/*.png");
    foreach($images as $image)
    {
        echo $image;
        echo "-";
    }
}

?>