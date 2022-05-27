<?php

$serverName = "54.219.56.114";
$username = "root";
$password = "";
$dbname = "hatsdb";

$conn = new mysqli($serverName, $username, $password, $dbname);

if($conn->connect_error) {
    die("Connection failed: " . $conn->connect_error);
}

$sql = "INSERT INTO `users` (`id`) VALUES (NULL)";

$conn->query($sql);

echo $conn->insert_id;

$conn->close();

?>