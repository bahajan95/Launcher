<?php

class App {

    public static $key;
    private static $_conf = false;
    protected static $_init;
    const PATH = ROOT;

    public static function __Init(){
        $key = isset($_GET['_key']) ? $_GET['_key'] : false;

        if(self::$key !== $key){
            header('HTTP/1.1 404 Not Found');
            header('Status: 404 Not Found');
            exit();
        }

        if(self::$_init === null){
            self::$_init = new self();
        }
        return self::$_init;
    }

    protected function __construct(){
        $file = self::PATH.'api'.DS.'config.php';
        if(is_file($file)){
            self::$_conf = require_once $file;
        }else{
            $this->error('Not Found File config!',E_USER_ERROR);
        }
    }

    private function error($msg,$type){
        if(DISPLAY_ERROR === true){
            trigger_error($msg,$type);
        }
        exit();
    }

    private function TestConnect($host,$port){
        $fp = @fsockopen($host, $port, $errno, $errstr, 10);
        if($fp){
            return true;
        }else{
            return false;
        }
    }

    protected function Connect($db){
       if($this->TestConnect(self::$_conf['host'],self::$_conf['port'])) {
           return new \medoo(array(
                   'database_type' => 'mysql',
                   'database_name' => $db,
                   'server' => self::$_conf['host'],
                   'username' => self::$_conf['user'],
                   'password' => self::$_conf['pass'],
                   'charset' => 'utf8',
                   'port' => self::$_conf['port'],
                   'option' => array(
                       \PDO::ATTR_CASE => \PDO::CASE_NATURAL
                   ))
           );
       }else{
           exit();
       }
    }

    protected function XMLRender($keys,$data,$multi = false){
        if(is_array($keys) and is_array($data)) {

            //header("Content-Type: text/xml");

            $xml = new XMLWriter();
            $xml->openMemory();
            $xml->startDocument();

            if($multi === false) {
                foreach ($keys as $key) {
                    $xml->startElement($key);
                }

                foreach ($data as $key => $val) {
                    $xml->writeElement($key, $val);
                }

                foreach ($keys as $key) {
                    $xml->endElement();
                }
            }else{
                $elem = array_pop($keys);

                foreach ($keys as $key) {
                    $xml->startElement($key);
                }

                foreach($data as $line){
                    $xml->startElement($elem);
                    foreach($line as $k=>$l){
                        $xml->writeElement($k, $l);
                    }
                    $xml->endElement();
                }

                foreach ($keys as $key) {
                    $xml->endElement();
                }
            }

           return $xml->outputMemory();
        }else{
            $this->error('XML Data ERROR',E_USER_ERROR);
        }
    }

    private function Fraction($race){
        if ($race === 1 || $race === 3 || $race === 4 || $race === 7 || $race === 11 || $race === 22) {
            return 'Alliance';
        } elseif($race == 24) {
            return 'Neutral';
        }else{
            return 'Horde';
        }
    }

    private function OnlineIcon($race,$gender){
        switch ($race) {
            case 1:
                ($gender === 0) ? $race = 'launcher/race/Ui-charactercreate-races_human-male.png' : $race = 'launcher/race/Ui-charactercreate-races_human-female.png';
                break;
            case 2:
                ($gender === 0) ? $race = 'launcher/race/Ui-charactercreate-races_orc-male.png' : $race = 'launcher/race/Ui-charactercreate-races_orc-female.png';
                break;
            case 3:
                ($gender === 0) ? $race = 'launcher/race/Ui-charactercreate-races_dwarf-male.png' : $race = 'launcher/race/Ui-charactercreate-races_dwarf-female.png';
                break;
            case 4:
                ($gender === 0) ? $race = 'launcher/race/Ui-charactercreate-races_nightelf-male.png' : $race = 'launcher/race/Ui-charactercreate-races_nightelf-female.png';
                break;
            case 5:
                ($gender === 0) ? $race = 'launcher/race/Ui-charactercreate-races_undead-male.png' : $race = 'launcher/race/Ui-charactercreate-races_undead-female.png';
                break;
            case 6:
                ($gender === 0) ? $race = 'launcher/race/Ui-charactercreate-races_tauren-male.png' : $race = 'launcher/race/Ui-charactercreate-races_tauren-female.png';
                break;
            case 7:
                ($gender === 0) ? $race = 'launcher/race/Ui-charactercreate-races_gnome-male.png' : $race = 'launcher/race/Ui-charactercreate-races_gnome-female.png';
                break;
            case 8:
                ($gender === 0) ? $race = 'launcher/race/Ui-charactercreate-races_troll-male.png' : $race = 'launcher/race/Ui-charactercreate-races_troll-female.png';
                break;
            case 10:
                ($gender === 0) ? $race = 'launcher/race/Ui-charactercreate-races_bloodelf-male.png' : $race = 'launcher/race/Ui-charactercreate-races_bloodelf-female.png';
                break;
            case 11:
                ($gender === 0) ? $race = 'launcher/race/Ui-charactercreate-races_draenei-male.png' : $race = 'launcher/race/Ui-charactercreate-races_draenei-female.png';
                break;
            case 22:
                ($gender === 0) ? $race = 'launcher/race/Ui-charactercreate-races_worgen-male.png' : $race = 'launcher/race/Ui-charactercreate-races_worgen-female.png';
                break;
            case 9:
                ($gender === 0) ? $race = 'launcher/race/Ui-charactercreate-races_goblin-male.png' : $race = 'launcher/race/Ui-charactercreate-races_goblin-female.png';
                break;
            case 24:
                ($gender === 0) ? $race = 'launcher/race/Ui-charactercreate-races_pandaren-male.png' : $race = 'launcher/race/Ui-charactercreate-races_pandaren-female.png';
                break;
        }
        return $race;
    }

    private function RaceIcon($race,$gender){
        switch ($race) {
            case 1:
                ($gender === 0) ? $race = 'Resources/charBg/1-0.jpg' : $race = 'Resources/charBg/1-1.jpg';
                break;  //humans
            case 2:
                ($gender === 0) ? $race = 'Resources/charBg/2-0.jpg' : $race = 'Resources/charBg/2-1.jpg';
                break;  //orks
            case 3:
                ($gender === 0) ? $race = 'Resources/charBg/3-0.jpg' : $race = 'launcher/character/charBg/3-1.jpg';
                break;  //dwarfs
            case 4:
                ($gender === 0) ? $race = 'Resources/charBg/4-0.jpg' : $race = 'launcher/character/charBg/4-1.jpg';
                break; //night elfs
            case 5:
                ($gender === 0) ? $race = 'Resources/charBg/5-0.jpg' : $race = 'Resources/charBg/5-1.jpg';
                break; // undead
            case 6:
                ($gender === 0) ? $race = 'Resources/charBg/6-0.jpg' : $race = 'Resources/charBg/6-1.jpg';
                break;  //tauren
            case 7:
                ($gender === 0) ? $race = 'Resources/charBg/7-0.jpg' : $race = 'Resources/charBg/7-1.jpg';
                break;  //gnomes
            case 8:
                ($gender === 0) ? $race = 'Resources/charBg/8-0.jpg' : $race = 'Resources/charBg/8-1.jpg';
                break; //troll
            case 10:
                ($gender === 0) ? $race = 'Resources/charBg/10-0.jpg' : $race = 'Resources/charBg/10-1.jpg';
                break; //blood elf
            case 11:
                ($gender === 0) ? $race = 'Resources/charBg/11-0.jpg' : $race = 'Resources/charBg/11-1.jpg';
                break; // drenei
            case 9:
                ($gender === 0) ? $race = 'Resources/charBg/9-0.jpg' : $race = 'Resources/charBg/9-1.jpg';
                break; // goblin
            case 22:
                ($gender === 0) ? $race = 'Resources/charBg/22-0.jpg' : $race = 'Resources/charBg/22-1.jpg';
                break; // worgen
            case 24:
                ($gender === 0) ? $race = 'Resources/charBg/24-0.jpg' : $race = 'Resources/charBg/24-1.jpg';
                break; // pandaren
        }
        return $race;
    }

    private function ClassState($class,$mana = 0,$rage = 0,$energy = 0,$powerRune = 0){
        $classes = array(
            1=>array('class'=>'Warrior','powerName'=>'Rage'),
            2=>array('class'=>'Paladin','powerName'=>'Mana'),
            3=>array('class'=>'Hunter','powerName'=>'Mana'),
            4=>array('class'=>'Rogue','powerName'=>'Energy'),
            5=>array('class'=>'Priest','powerName'=>'Mana'),
            6=>array('class'=>'Death Knight','powerName'=>'Runic Power'),
            7=>array('class'=>'Shaman','powerName'=>'Mana'),
            8=>array('class'=>'Mage','powerName'=>'Mana'),
            9=>array('class'=>'Warlock','powerName'=>'Mana'),
            11=>array('class'=>'Druid','powerName'=>'Mana'),
            10=>array('class'=>'Monk','powerName'=>'Energy')
        );

        if(isset($classes[$class])){
            $cls = $classes[$class];
            if($cls['powerName'] === 'Mana'){
                $cls['power'] = $mana;
            }elseif($cls['powerName'] === 'Energy'){
                $cls['power'] = $energy;
            }elseif($cls['powerName'] === 'Runic Power'){
                $cls['power'] = $powerRune;
            }elseif($cls['powerName'] === 'Rage') {
                $cls['power'] = $rage;
            }
            return $cls;
        }else{
            $this->error('Not Found Class '.$class,E_USER_ERROR);
        }
    }

    private function HonorRang($honor = 0,$race = 0){
        $rank_a = array(
            0 => 'No rank',
            1 => 'Private',
            2 => 'Corporal',
            3 => 'Sergeant',
            4 => 'Master Sergeant',
            5 => 'Sergeant Major',
            6 => 'Knight',
            7 => 'Knight-Lieutenant',
            8 => 'Knight-Captain',
            9 => 'Knight-Champion',
            10 => 'Lieutenant Commander',
            11 => 'Commander',
            12 => 'Marshal',
            13 => 'Field Marshal',
            14 => 'Grand Marshal',
            15 => 'City Defender'
        );
        $rank_h = array(
            0 => 'No rank',
            1 => 'Scout',
            2 => 'Grunt',
            3 => 'Sergeant',
            4 => 'Senior Sergeant',
            5 => 'First Sergeant',
            6 => 'Stone Guard',
            7 => 'Blood Guard',
            8 => 'Legionnaire',
            9 => 'Centurion',
            10 => 'Champion',
            11 => 'Lieutenant General',
            12 => 'General',
            13 => 'Warlord',
            14 => 'High Warlord',
            15 => 'City Defender'
        );
        if ($honor <= 0)
            $rank = 0;
        elseif ($honor < 500)
            $rank = 1;
        elseif ($honor < 1500)
            $rank = 2;
        elseif ($honor < 3000)
            $rank = 3;
        elseif ($honor < 5000)
            $rank = 4;
        elseif ($honor < 7500)
            $rank = 5;
        elseif ($honor < 10000)
            $rank = 6;
        elseif ($honor < 15000)
            $rank = 7;
        elseif ($honor < 20000)
            $rank = 8;
        elseif ($honor < 30000)
            $rank = 9;
        elseif ($honor < 40000)
            $rank = 10;
        elseif ($honor < 50000)
            $rank = 11;
        elseif ($honor < 75000)
            $rank = 12;
        elseif ($honor < 100000)
            $rank = 13;
        elseif ($honor < 150000)
            $rank = 14;
        else
            $rank = 15;
        if($race === 1 || $race === 3 || $race === 4 || $race === 7 || $race === 11)
            return $rank_a[$rank];
        else
            return $rank_h[$rank];
    }

    private function totalTime($time){
        $totaltime = $time;
        $sec = $totaltime%60;
        $totaltime = intval ($totaltime/60);
        $min = $totaltime%60;
        $totaltime = intval ($totaltime/60);
        $hours = $totaltime%24;
        $totaltime = intval($totaltime/24);
        $days = $totaltime;
        return "Total time in game: $days d. $hours h. $min m.";
    }

    private function Inventory(){
        return array(
            0 => 'HeadChar',
            1 => 'Neck',
            2 => 'Shoulders',
            3 => 'BodyChar',
            4 => 'ChestChar',
            5 => 'Waist',
            6 => 'Legs',
            7 => 'Feet',
            8 => 'Wrists',
            9 => 'Hands',
            10 => 'Finger1',
            11 => 'Finger2',
            12 => 'Trinket1',
            13 => 'Trinket2',
            14 => 'Back',
            15 => 'MainHand',
            16 => 'OffHand',
            17 => 'Ranget',
            18 => 'Tabard'
        );
    }

    private function is_item($item){
        if (is_numeric($item)) {
            return self::$_conf['site_url'] . 'launcher/race/noteImage.png';
        } else {
            return self::$_conf['wowhead_url_icons'].$item.'.jpg';
        }
    }

    public function getCharInfo($name){
        $db = $this->Connect(self::$_conf['characters']);
        $char = $db->select('characters',array('guid','power1','power2','power4','power7','class','name','level','race','gender','health','totaltime','totalHonorPoints','arenaPoints','totalKills'),array('name'=>$name));
        if($char !== false and count($char) > 0){
            $character = $char[0];

            $mana = $character['power1'];  //mp
            $rage = $character['power2'];  //Rage
            $energy = $character['power4'];  //Energy
            $powerRune = $character['power7'];  //runic power
            $character_stat = $this->ClassState($character['class'],$mana,$rage,$energy,$powerRune);

            $stats = '';
            $stats['Name'] = $character['name'];
            $stats['Level'] = $character['level'];
            $stats['Race'] = $this->RaceIcon((int) $character['race'],(int) $character['gender']);
            $stats['Class'] = $character_stat['class'];
            $stats['Side'] = $this->Fraction((int) $character['race']);
            $stats['Health'] = 'Health: '.$character['health'];  //hp
            $stats['Power'] = $character_stat['powerName'].':'.$character_stat['power'];
            $stats['TotalTime'] = $this->totalTime($character['totaltime']);
            $stats['Rank'] = 'Rank: '.$this->HonorRang((int) $character['totalHonorPoints'],(int) $character['race']);
            $stats['Honor'] = 'Honor Points: '.$character['totalHonorPoints'];
            $stats['Arena'] = 'Arena Points: '.$character['arenaPoints'];
            $stats['Kills'] = 'Total kills: '.$character['totalKills'];
            $stats['Quest'] = 'Done Quests: '.$db->count('character_queststatus_rewarded', array(
                    'guid' => $character['guid']
                ));
            $stats['Achiev'] = 'Obtain achievements: '.$db->count('character_achievement', array(
                    'guid' => $character['guid']
                ));

            echo $this->XMLRender(array('Characters','Stat','CharBlock'),$stats);
        }else{
            $this->error('Not Found character '.$name,E_USER_ERROR);
        }
    }

    public function getCharItem($name){
        $slots = $this->Inventory();
        $db_char = $this->Connect(self::$_conf['characters']);
        $db_site = $this->Connect(self::$_conf['icon_db']);
        $db_world = $this->Connect(self::$_conf['world']);
        $character = $db_char->select('characters',array('guid'),array('name'=>$name));

        if($character !== false and count($character) > 0) {

            $inventory = $db_char->select('character_inventory', '*', array('AND'=>array('guid' => $character[0]['guid'],'slot[<>]'=>array(0,19),'bag'=>0)));

            if($inventory !== false and count($inventory) > 0){

                $itemIcon = '';
                $itemsDisplayID = '';
                $charaterInventory = '';
                $guid = '';

                foreach($inventory as $invert){
                    if(isset($slots[$invert['slot']])){
                        $charaterInventory[$slots[$invert['slot']]] = $invert['item'];
                        if($guid === '') {
                            $guid = $invert['guid'];
                        }
                    }
                }

                if(is_array($charaterInventory)) {
                    foreach ($charaterInventory as $key => $charInf) {
                        $instance = $db_char->select('item_instance',array('itemEntry'),array('AND'=>array('owner_guid'=>$guid,'guid'=>$charInf)));

                        if($instance !== false and count($instance) > 0){
                            foreach($instance as $inst) {
                                $itemID = $db_world->select('item_template', array('displayid'), array('entry' =>$inst['itemEntry']));
                                if($itemID !== false and count($itemID) > 0) {
                                    $itemsDisplayID[$key] = $itemID[0]['displayid'];
                                }
                            }
                        }
                    }
                }

                if (is_array($itemsDisplayID)) {
                    foreach($itemsDisplayID as $name=>$display){
                        $itemIconName = $db_site->select('tab_icon',array('icon'),array('guid'=>$display));
                        if($itemIconName !== false and count($itemIconName) > 0) {
                            $itemIcon[$name] = strtolower($itemIconName[0]['icon']);
                        }
                    }
                }

                if (is_array($itemIcon)) {
                    $inventorySlots = array_flip($slots);
                    $itemIcon = array_merge($inventorySlots, $itemIcon);
                    $itemIcon = array_map(array(__CLASS__,'is_item'), $itemIcon);

                    echo $this->XMLRender(array('Characters','Stat','CharBlock'),$itemIcon);
                }
            }
        }else{
            $this->error('Not Found Character:'.$name,E_USER_ERROR);
        }
    }

    public function getOnline(){
        $db = $this->Connect(self::$_conf['characters']);
        $online = '';
        $online_list = $db->query("SELECT name,race,class,level,gender FROM `characters` WHERE `online` = 1 AND NOT `extra_flags` & 16 ORDER BY `name`")->fetchAll();

        if($online_list !== false and count($online_list) > 0){

            foreach($online_list as $key=>$onl){
                $class = $this->ClassState($onl['class']);
                $online[$key] = array(
                    'Name'=>$onl['name'],
                    'Level'=>$onl['level'],
                    'Race'=>self::$_conf['site_url'].$this->OnlineIcon((int) $onl['race'],(int) $onl['gender']),
                    'Class'=>$class['class'],
                    'Side'=>$this->Fraction((int) $onl['race']),
                    'TotalTime'=>$this->totalTime($onl['totaltime'])
                );
            }

            if(is_array($online)){
                echo $this->XMLRender(array('Characters','Stat','CharBlock'),$online,true);
            }else{
                echo 0;
            }

        }else{
            echo 0;
        }
    }

    public function getHotNews(){
        $db = $this->Connect(self::$_conf['icon_db']);
        $hot_news = $db->select('hot_news','*');
        if($hot_news !== false and count($hot_news) > 0 and $hot_news[0]['hot_news'] != ''){
            echo $hot_news[0]['hot_news'];
        }else{
            echo 'note';
        }
    }

    public function getNews(){
        $db = $this->Connect(self::$_conf['icon_db']);
        $new = '';
        $news = $db->select('news_launcher','*',array('ORDER'=>'id DESC','LIMIT'=>4));

        if($news != false and count($news) > 0){
            foreach($news as $key=>$new_post){
                $new[$key] = array(
                    'NewsTitle'=>$new_post['news_head'],
                    'Text'=>$new_post['news_body'],
                    //'ImageLink'=>$new_post['image'],
                    'NewsLink'=>$new_post['news_link'],
                );
            }

            if(is_array($new)){
                echo $this->XMLRender(array('NewsRoot','ExpressNews','NewsItem'),$new,true);
            }else{
                echo $this->XMLRender(array('NewsRoot','ExpressNews','NewsItem'),array('NewsTitle'=>'No news !'));
            }
        }else{
            echo $this->XMLRender(array('NewsRoot','ExpressNews','NewsItem'),array('NewsTitle'=>'No news !'));
        }
    }

    public function CountOnline(){
        $db = $this->Connect(self::$_conf['characters']);
        $online = $db->count('characters',array('online'=>1));
        if(is_numeric($online) and $online !== 0){
            echo $online;
        }else{
            echo 0;
        }
    }
}