/// <reference path="phaser.js" />
/// <reference path="view.js" />
var game;
window.onload = function () {
	var elm = document.getElementById("content");
	makeMonsterMap();
    game = new Phaser.Game(elm.offsetWidth, elm.offsetWidth, Phaser.AUTO, "content", { preload: preload, create: create, update: update, render: render }, false);
}

var shop;

// The screen that is being rendered by the game
// View.current

// The server object used to pull data from the server
var server;

var monsterMap;

function makeMonsterMap() {
	monsterMap = {};
	var actions = ["WALKING_", "ATTACKING_", "DYING"];
	var directions = ["DOWN", "DOWN_RIGHT", "RIGHT", "UP_RIGHT", "UP", "UP_LEFT", "LEFT", "DOWN_LEFT"];
	var monsters = ["BASAL_GOLEM", "BANSHEE", "BRIGAND", "CLERIC", "PEASANT", "DRUID", "FURY", "GARGOYLE",
		"GHOUL", "GNOME", "STONE_GOLEM", "GORGON", "GRIFFEN", "RANGER", "WARRIOR", "WIZARD", "WOLF", "ZOMBIE"];
	for (var m = 0; m < monsters.length; m++) {
		var monster = monsters[m];
		for (var a = 0; a < 2; a++) {
			for (var d = 0; d < 8; d++) {
				var animationArray = [];
				for (var i = 0; i < 4; i++) {
					animationArray[i] = monster + "/" + actions[a] + directions[d] + "_" + i;
				}
				monsterMap[monster + "/" + actions[a] + directions[d]] = animationArray;
			}
		}
		var animationArray = [];
		for (var i = 0; i < 8; i++) {
			animationArray[i] = monster + "/" + actions[2] + i;
		}
		monsterMap[monster + "/" + actions[2]] = animationArray;
	}
}

function preload () {
    // download all sprites
    game.load.atlasJSONHash("objects", "/Content/Images/spritesheet3.png", "/Content/Images/spritesheet3.json");
    //game.load.atlasJSONHash("stone_golem", "/Content/Images/stone_golem.png", "/Content/Images/stone_golem.json");
    game.load.atlasJSONHash("monsters", "/Content/Images/monsters.png", "/Content/Images/monsters.json");
	//server = new Server("", onerror, onclose);
}

function create () {
    // add all objects to game to be displayed
    //grass = new game.add.sprite(game.world.centerX, 500, 'grass', 'objects');
    //grass = game.add.tileSprite(0, 0, game.world.width, game.world.height, "objects", "grass");
    //View.current = new View.MainGameScreen(game, Server, 300, 30, game.width - 60, game.height - 60);
    //shop = new View.BuildingShop(game, Server, 40, 40, game.world.width - 80, game.world.height - 80);
    //mgw = new View.MainGameScreen(game, Server, 30, 20 + 32 * 2, game.width - 60, game.height - 30 - 20 - 32 * 2);
    var nav = new View.TileNavigator(game, Server, 30, 20 + 32 * 2, game.width - 60, game.height - 30 - 20 - 32 * 2);
    var shop = new View.ButtonScreen(game, 20, 10, 64, 64, "objects", "ShopButton", openShop, UpdateShopCosts);
    var views = [
        nav,
        shop,
        new View.InventoryBar(game, 84, 10, game.width - 20 - 84, 32*2)
    ];
    mgw = nav.mainGameScreen;
    View.current = new View.ScreenContainer(views, 0, 0, game.width, game.height);
    View.current.create();
    Server.register(View.current);
    Server.register(shop);
    Server.register(nav);
    View.current.screens.forEach(function (val) {
        Server.register(val);
    });
    Server.open();

    game.stage.backgroundColor = 0xbbddff;
}

function update () {
    // called every frame
    View.current.update();
}

function render () {
    // I don't know what this is for
    // Docs say it's not used often, so...
    View.current.render();
}

function openShop() {
    $("#shop-window").show();
}

Server = (function () {
    var socket = null;
    var listeners = new Array();
    function open() {
        socket = new WebSocket("wss://" + window.location.host + "/api/WebSocket/" + WorldId);
        socket.onmessage = onmessage;
        socket.onclose = onclose;
        socket.onerror = onerror;
    }

    function register(listener) {
        listeners.push(listener);
    }

    function unregister(listener){
        throw new Error("Not yet implemented.");
    }
	
    var first = true;
    function onmessage(message) {
        var obj = JSON.parse(message.data);
        for (var x = 0; x < listeners.length; ++x) {
            for (var y = 0; y < listeners[x].subscribesTo.length; y++) {
                var subs = listeners[x].subscribesTo[y].split('.');
                var prop = obj;
                var worked = true;
                for (var z = 0; z < subs.length; z++) {
                    if (prop[subs[z]]) {
                        prop = prop[subs[z]];
                    } else {
                        worked = false;
                        break;
                    }
                }
                if (worked) {
                    listeners[x].onmessage(prop, listeners[x].subscribesTo[y]);
                }
            }
        }
    }

    window.onbeforeunload = function () {
        socket.close();
    }

    function close() {
        socket.close();
    }

    function send(msg) {
        socket.send(msg);
    }

    function onclose() {
        View.current.destroy();
        View.current = new View.TextScreen(game, 0, 0, game.world.width, game.world.height, "Disconnected", false);
    }

    function onerror() {
        View.current.destroy();
        View.current = new View.TextScreen(game, 0, 0, game.world.width, game.world.height, "Disconnected", false);
    }

    return {
        open: open,
        register: register,
        close: close,
        send: send
    }
})();