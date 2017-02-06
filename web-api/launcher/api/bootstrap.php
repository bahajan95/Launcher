<?php

if (version_compare(PHP_VERSION, '5.4.0') < 0) {
    die('[My PHP version('.PHP_VERSION.')] << [APP PHP version(5.4.0)]');
}

require_once ROOT.'api'.DS.'Autoload.php';

Autoload::register(ROOT);

$get = isset($_GET['_url']) ? $_GET['_url'] : false;

\App::$key = 'ffc312f882fbeeb51504483ee8c691a2';

$api = \App::__Init();

if(strpos($get,'CharInfo/') !== false) {
    $char = explode('/', $get);
    $api->getCharInfo($char[1]);
}elseif(strpos($get,'CharItem/') !== false) {
    $char = explode('/', $get);
    $api->getCharItem($char[1]);
}elseif($get === 'online'){
    $api->getOnline();
}elseif($get === 'news'){
    $api->getNews();
}elseif($get === 'hot_news') {
    $api->getHotNews();
}elseif($get === 'count_online'){
    $api->CountOnline();
}else{
    header('HTTP/1.1 404 Not Found');
    header('Status: 404 Not Found');
    exit();
}



